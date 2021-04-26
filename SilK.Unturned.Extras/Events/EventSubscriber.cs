using Autofac;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Common.Helpers;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SilK.Unturned.Extras.Events
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    internal class EventSubscriber : IEventSubscriber
    {
        private readonly IRuntime _runtime;
        private readonly ILogger<EventSubscriber> _logger;
        private readonly IEventBus _eventBus;
        private readonly IServiceProvider _serviceProvider;

        public EventSubscriber(
            ILogger<EventSubscriber> logger,
            IEventBus eventBus,
            IServiceProvider serviceProvider, IRuntime runtime)
        {
            _logger = logger;
            _eventBus = eventBus;
            _serviceProvider = serviceProvider;
            _runtime = runtime;
        }

        private IEnumerable<Type> GetEventListenerTypes(Type type) => type.GetInterfaces().Where(x =>
            x.IsGenericType && (typeof(IInstanceEventListener<>).IsAssignableFrom(x.GetGenericTypeDefinition()) ||
                                typeof(IInstanceAsyncEventListener<>).IsAssignableFrom(x.GetGenericTypeDefinition())));

        public IDisposable Subscribe(object target, IOpenModComponent component)
        {
            var eventListeners = GetEventListenerTypes(target.GetType());

            var disposables = new List<IDisposable>();

            foreach (var listener in eventListeners)
            {
                var eventType = listener.GetGenericArguments().Single();

                var method = listener.GetMethod("HandleEventAsync", BindingFlags.Public | BindingFlags.Instance);

                if (method == null)
                {
                    _logger.LogWarning(
                        $"Event callback method for type {eventType.FullName} could not be found. Plugin will not function properly.");
                    continue;
                }

                var disposable = _eventBus.Subscribe(component, eventType, async (_, sender, @event) =>
                {
                    var task = (UniTask) method.Invoke(target, new[] {sender, @event});

                    if (typeof(IInstanceAsyncEventListener<>).IsAssignableFrom(listener.GetGenericTypeDefinition()))
                    {
                        task.Forget();
                    }
                    else
                    {
                        await task.AsTask();
                    }
                });

                disposables.Add(disposable);
            }

            return new DisposeAction(disposables.DisposeAll);
        }

        // todo: Use ServiceRegistrationHelper.FindFromAssembly in next patch
        public static IEnumerable<ServiceRegistration> FindFromAssembly<T>(Assembly assembly, ILogger? logger = null) where T : ServiceImplementationAttribute
        {
            List<Type> types;
            try
            {
                types = AssemblyExtensions.GetLoadableTypes(assembly)
                    .Where(d => d.IsClass && !d.IsInterface && !d.IsAbstract)
                    .ToList();
            }
            catch (ReflectionTypeLoadException ex) //this ignores missing optional dependencies
            {
                logger?.LogTrace(ex, $"Some optional dependencies are missing for \"{assembly}\"");
                if (ex.LoaderExceptions != null && ex.LoaderExceptions.Length > 0)
                {
                    foreach (var loaderException in ex.LoaderExceptions)
                    {
                        logger?.LogTrace(loaderException, "Loader Exception: ");
                    }
                }


                types = ex.Types.Where(tp => tp != null && tp.IsClass && !tp.IsInterface && !tp.IsAbstract)
                    .ToList();
            }

            foreach (var type in types)
            {
                T? attribute;
                Type[] interfaces;

                try
                {
                    attribute = (T?)type.GetCustomAttributes(inherit: false).FirstOrDefault(x => x.GetType() == typeof(T));
                    if (attribute == null)
                    {
                        continue;
                    }

                    interfaces = type.GetInterfaces()
                        .Where(d => d.GetCustomAttribute<ServiceAttribute>() != null)
                        .ToArray();

                    if (interfaces.Length == 0)
                    {
                        logger?.LogWarning(
                            $"Type {type.FullName} in assembly {assembly.FullName} has been marked as ServiceImplementation but does not inherit any services!\nDid you forget to add [Service] to your interfaces?");
                        continue;
                    }

                }
                catch (Exception ex)
                {
                    logger?.LogWarning($"FindFromAssembly has failed for type: {type.FullName} while searching for {typeof(T).FullName}", ex);
                    continue;
                }

                yield return new ServiceRegistration
                {
                    Priority = attribute.Priority,
                    ServiceImplementationType = type,
                    ServiceTypes = interfaces,
                    Lifetime = attribute.Lifetime
                };
            }
        }

        public IDisposable SubscribeServices(Assembly assembly, IOpenModComponent component)
        {
            var disposables = new List<IDisposable>();

            var registrations =
                FindFromAssembly<ServiceImplementationAttribute>(assembly)
                    .Where(x => x.Lifetime == ServiceLifetime.Singleton).ToList();

            var pluginRegistrations =
                FindFromAssembly<PluginServiceImplementationAttribute>(assembly)
                    .Where(x => x.Lifetime == ServiceLifetime.Singleton).ToList();

            foreach (var registration in registrations.Concat(pluginRegistrations))
            {
                if (!GetEventListenerTypes(registration.ServiceImplementationType).Any()) continue;

                foreach (var serviceType in registration.ServiceTypes)
                {
                    var service = component.LifetimeScope.ResolveOptional(serviceType);

                    if (service == null || service.GetType() != registration.ServiceImplementationType) continue;

                    // Service is singleton, exists, has event listeners, and is same as implementation

                    var disposable = Subscribe(service, component);

                    disposables.Add(disposable);
                }
            }

            return new DisposeAction(disposables.DisposeAll);
        }
    }
}
