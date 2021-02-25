using OpenMod.API.Eventing;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.Events
{
    public interface IPluginEventListener<in TEvent> where TEvent : IEvent
    {
        Task HandleEventAsync(object sender, TEvent @event);
    }
}
