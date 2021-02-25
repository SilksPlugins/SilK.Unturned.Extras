using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.Events
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class EventSubscriber : IEventSubscriber
    {
        private readonly IRuntime _runtime;
        private readonly IEventBus _eventBus;

        private readonly Dictionary<Type, List<(IOpenModPlugin plugin, MethodInfo method)>> _subscribedEvents;

        public EventSubscriber(
            IRuntime runtime,
            IEventBus eventBus)
        {
            _runtime = runtime;
            _eventBus = eventBus;

            _subscribedEvents = new Dictionary<Type, List<(IOpenModPlugin plugin, MethodInfo method)>>();
        }

        public void SetupEvents(IOpenModPlugin plugin)
        {
            var type = plugin.GetType();

            var eventListeners = type.GetInterfaces().Where(x =>
                x.IsGenericType && x.GetGenericTypeDefinition().IsAssignableFrom(typeof(IPluginEventListener<>)));

            foreach (var listener in eventListeners)
            {
                var eventType = listener.GetGenericArguments().Single();

                if (!_subscribedEvents.ContainsKey(eventType))
                {
                    _subscribedEvents.Add(eventType, new List<(IOpenModPlugin addon, MethodInfo method)>());

                    _eventBus.Subscribe(_runtime, eventType, HandleEventAsync);
                }

                _subscribedEvents[eventType].Add((plugin, listener.GetMethod("HandleEventAsync", BindingFlags.Public | BindingFlags.Instance)));
            }
        }

        public async Task HandleEventAsync(IServiceProvider serviceProvider, object? sender, IEvent @event)
        {
            if (_subscribedEvents.TryGetValue(@event.GetType(), out var methods))
            {
                foreach (var (addon, method) in methods)
                {
                    await (Task)method.Invoke(addon, new[] { sender, @event });
                }
            }
        }
    }
}
