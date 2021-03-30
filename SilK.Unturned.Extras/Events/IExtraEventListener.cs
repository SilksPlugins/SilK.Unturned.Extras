using Cysharp.Threading.Tasks;
using OpenMod.API.Eventing;

namespace SilK.Unturned.Extras.Events
{
    public interface IExtraEventListener<in TEvent> where TEvent : IEvent
    {
        UniTask HandleEventAsync(object sender, TEvent @event);
    }
}
