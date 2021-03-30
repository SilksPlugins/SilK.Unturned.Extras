using System.Reflection;
using OpenMod.API;
using OpenMod.API.Ioc;

namespace SilK.Unturned.Extras.Events
{
    /// <summary>
    /// The event subscriber subscribes events to callback methods within a given class instance
    /// based on implementations of <see cref="IInstanceEventListener{TEvent}"/> and <see cref="IInstanceAsyncEventListener{TEvent}"/>.
    /// </summary>
    [Service]
    public interface IEventSubscriber
    {
        /// <summary>
        /// Subscribe events to callback methods within the given target class instance
        /// which are attached to the specified <paramref name="component"/>.
        /// </summary>
        /// <param name="target">The target class instance to subscribe events to</param>
        /// <param name="component">The component attached to the event subscriptions.</param>
        void Subscribe(object target, IOpenModComponent component);

        /// <summary>
        /// Subscribe events to callback methods within singleton
        /// services in the given target <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to search for singleton services within.</param>
        /// <param name="component">The component attached to the event subscriptions.</param>
        void SubscribeServices(Assembly assembly, IOpenModComponent component);
    }
}
