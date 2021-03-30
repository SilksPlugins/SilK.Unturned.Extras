using OpenMod.API;
using OpenMod.API.Ioc;

namespace SilK.Unturned.Extras.Events
{
    [Service]
    public interface IEventSubscriber
    {
        void SubscribeEvents(object target, IOpenModComponent component);
    }
}
