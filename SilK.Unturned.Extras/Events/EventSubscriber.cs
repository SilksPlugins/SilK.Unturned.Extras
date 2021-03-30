using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using System.Linq;
using System.Reflection;

namespace SilK.Unturned.Extras.Events
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    internal class EventSubscriber : IEventSubscriber
    {
        private readonly ILogger<EventSubscriber> _logger;
        private readonly IEventBus _eventBus;

        public EventSubscriber(ILogger<EventSubscriber> logger,
            IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }
        
        public void SubscribeEvents(object target, IOpenModComponent component)
        {
            var eventListeners = target.GetType().GetInterfaces().Where(x =>
                x.IsGenericType && (typeof(IExtraEventListener<>).IsAssignableFrom(x.GetGenericTypeDefinition()) ||
                                    typeof(IAsyncExtraEventListener<>).IsAssignableFrom(x.GetGenericTypeDefinition())));

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

                _eventBus.Subscribe(component, eventType, async (_, sender, @event) =>
                {
                    var task = (UniTask) method.Invoke(target, new[] {sender, @event});

                    if (typeof(IAsyncExtraEventListener<>).IsAssignableFrom(listener.GetGenericTypeDefinition()))
                    {
                        task.Forget();
                    }
                    else
                    {
                        await task.AsTask();
                    }
                });
            }
        }
    }
}
