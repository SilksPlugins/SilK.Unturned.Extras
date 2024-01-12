using Autofac;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
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
        private readonly ILogger<EventSubscriber> _logger;
        private readonly IEventBus _eventBus;

        public EventSubscriber(
            ILogger<EventSubscriber> logger,
            IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
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
                    UniTask GetTask() => (UniTask)method.Invoke(target, new[] { sender, @event });

                    if (typeof(IInstanceAsyncEventListener<>).IsAssignableFrom(listener.GetGenericTypeDefinition()))
                    {
                        UniTask.RunOnThreadPool(GetTask).Forget();
                    }
                    else
                    {
                        await GetTask().AsTask();
                    }
                });

                disposables.Add(disposable);
            }

            return new DisposeAction(disposables.DisposeAll);
        }

        public IDisposable SubscribeServices(Assembly assembly, IOpenModComponent component)
        {
            var disposables = new List<IDisposable>();

            var registrations =
                ServiceRegistrationHelper.FindFromAssembly<ServiceImplementationAttribute>(assembly)
                    .Where(x => x.Lifetime == ServiceLifetime.Singleton).ToList();

            foreach (var registration in registrations)
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