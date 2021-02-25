using OpenMod.API.Eventing;
using OpenMod.Core.Plugins.Events;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.Events
{
    internal class PluginLoadedEventListener : IEventListener<PluginLoadedEvent>
    {
        private readonly IEventSubscriber _eventSubscriber;

        public PluginLoadedEventListener(IEventSubscriber eventSubscriber)
        {
            _eventSubscriber = eventSubscriber;
        }

        public Task HandleEventAsync(object? sender, PluginLoadedEvent @event)
        {
            _eventSubscriber.SetupEvents(@event.Plugin);

            return Task.CompletedTask;
        }
    }
}
