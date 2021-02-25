using OpenMod.API.Ioc;
using OpenMod.API.Plugins;

namespace SilK.Unturned.Extras.Events
{
    [Service]
    public interface IEventSubscriber
    {
        void SetupEvents(IOpenModPlugin plugin);
    }
}
