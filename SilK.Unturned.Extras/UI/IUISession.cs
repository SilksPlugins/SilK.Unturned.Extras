using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Users;
using System;

namespace SilK.Unturned.Extras.UI
{
    public interface IUISession
    {
        /// <summary>
        /// A unique identifier for this type of UI session.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The user this UI session pertains to.
        /// Implementations should decide how to initialize this property.
        /// </summary>
        UnturnedUser User { get; }

        /// <summary>
        /// A method which is called when this UI session begins.
        /// Called once per session.
        /// </summary>
        UniTask StartAsync();

        /// <summary>
        /// A method which is called when this UI session ends.
        /// Called once per session.
        /// </summary>
        UniTask EndAsync();

        /// <summary>
        /// An event which should be invoked when the UI session has finished ending;
        /// </summary>
        event Action<IUISession> OnUISessionEnded;
    }
}
