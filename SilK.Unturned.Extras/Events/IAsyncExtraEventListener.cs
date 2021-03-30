using Cysharp.Threading.Tasks;
using OpenMod.API.Eventing;

namespace SilK.Unturned.Extras.Events
{
    /// <summary>
    /// When implemented by a plugin class, the inherited callback method will be executed when the given event is emitted.
    /// This callback will be executed separate from the game thread, allowing the callback to execute without blocking the game thread.
    /// </summary>
    /// <typeparam name="TEvent">The event to be subscribed to.</typeparam>
    public interface IAsyncExtraEventListener<in TEvent> where TEvent : IEvent
    {
        UniTask HandleEventAsync(object? sender, TEvent @event);
    }
}
