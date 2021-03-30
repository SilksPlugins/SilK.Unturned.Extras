using OpenMod.API.Eventing;

namespace SilK.Unturned.Extras.Events
{
    /// <summary>
    /// This event callback interface will generate a true async context for executing the callback method.
    /// </summary>
    /// <typeparam name="TEvent">The event to subscribe to.</typeparam>
    public interface IAsyncExtraEventListener<in TEvent> : IExtraEventListener<TEvent> where TEvent : IEvent
    {
    }
}
