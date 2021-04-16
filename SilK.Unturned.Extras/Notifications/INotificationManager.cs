using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.Unturned.Users;
using System.Collections.Generic;

namespace SilK.Unturned.Extras.Notifications
{
    /// <summary>
    /// A service for sending notifications for players.
    /// A basic notification implementation is provided in this plugin,
    /// but a UI-based version is available at https://imperialplugins.com/Unturned/Products/Notifications
    /// </summary>
    [Service]
    public interface INotificationManager
    {
        /// <summary>
        /// Send a user a notification with the given message.
        /// </summary>
        /// <param name="user">The user to send the notification to.</param>
        /// <param name="message">The message to send.</param>
        UniTask SendNotificationAsync(UnturnedUser user, string message);

        /// <summary>
        /// Send a user a notification with the given message and text-based icon.
        /// </summary>
        /// <param name="user">The user to send the notification to.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="icon">The text-based icon to send alongside the message.</param>
        UniTask SendNotificationAsync(UnturnedUser user, string message, string icon);

        /// <summary>
        /// Send a user a notification with the given message and an image icon.
        /// </summary>
        /// <param name="user">The user to send the notification to.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="imageUrl">The url of the image icon to send alongside the message.</param>
        UniTask SendNotificationWithImageAsync(UnturnedUser user, string message, string imageUrl);

        /// <summary>
        /// Send multiple users a notification with the given message.
        /// </summary>
        /// <param name="users">The users to send the notification to.</param>
        /// <param name="message">The message to send.</param>
        UniTask SendNotificationAsync(IEnumerable<UnturnedUser> users, string message);

        /// <summary>
        /// Send multiple users a notification with the given message and text-based icon.
        /// </summary>
        /// <param name="users">The users to send the notification to.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="icon">The text-based icon to send alongside the message (i.e. <c>!</c>, <c>&lt;3</c>)</param>
        UniTask SendNotificationAsync(IEnumerable<UnturnedUser> users, string message, string icon);

        /// <summary>
        /// Send multiple users a notification with the given message and an image icon.
        /// </summary>
        /// <param name="users">The users to send the notification to.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="imageUrl">The url of the image icon to send alongside the message.</param>
        UniTask SendNotificationWithImageAsync(IEnumerable<UnturnedUser> users, string message, string imageUrl);

        /// <summary>
        /// Send all Unturned users a notification with the given message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        UniTask SendAllUsersNotificationAsync(string message);

        /// <summary>
        /// Send all Unturned users a notification with the given message and text-based icon.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="icon">The text-based icon to send alongside the message (i.e. <c>!</c>, <c>&lt;3</c>)</param>
        UniTask SendAllUsersNotificationAsync(string message, string icon);

        /// <summary>
        /// Send all Unturned users a notification with the given message and an image icon.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="imageUrl">The url of the image icon to send alongside the message.</param>
        UniTask SendAllUsersNotificationWithImageAsync(string message, string imageUrl);
    }
}
