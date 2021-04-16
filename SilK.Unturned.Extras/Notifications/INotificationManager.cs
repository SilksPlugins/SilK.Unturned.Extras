using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.Unturned.Users;

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
        UniTask SendNotificationAsync(UnturnedUser user, string message);

        UniTask SendNotificationAsync(UnturnedUser user, string message, string icon);

        UniTask SendNotificationWithImageAsync(UnturnedUser user, string message, string imageUrl);
    }
}
