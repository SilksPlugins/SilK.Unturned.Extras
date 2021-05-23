using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Users;
using System;

namespace SilK.Unturned.Extras.UI
{
    /// <summary>
    /// A class which represents a UI session for a given player.
    /// UI sessions are used alongside <see cref="IUIManager"/>.
    /// </summary>
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
        /// A method which is called by <see cref="IUIManager"/>
        /// when this UI session begins. Called once per session.
        /// </summary>
        UniTask StartAsync();

        /// <summary>
        /// A method which is called by <see cref="IUIManager"/>
        /// when this UI session ends. Called once per session.
        /// <br/>
        /// <b>This method can also be called manually to end the session.</b>
        /// <br/>
        /// The <see cref="IUIManager"/> service will handle the session
        /// end properly by subscribing to <see cref="OnUISessionEnded"/>.
        /// </summary>
        UniTask EndAsync();

        /// <summary>
        /// An event which should be invoked when the UI session has finished ending.
        /// Used by the <see cref="IUIManager"/> service to determine when the session has ended.
        /// </summary>
        event Action<IUISession> OnUISessionEnded;
    }
}
