extern alias JetBrainsAnnotations;
using Cysharp.Threading.Tasks;
using JetBrainsAnnotations::JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Unturned.Users;
using System.Collections.Generic;
using System.Drawing;

namespace SilK.Unturned.Extras.Notifications
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    internal class TextMessageNotificationManager : INotificationManager
    {
        private readonly IUnturnedUserDirectory _userDirectory;

        public TextMessageNotificationManager(IUnturnedUserDirectory userDirectory)
        {
            _userDirectory = userDirectory;
        }

        public UniTask SendNotificationAsync(UnturnedUser user, string message)
        {
            return user.PrintMessageAsync(message).AsUniTask();
        }

        public UniTask SendNotificationAsync(UnturnedUser user, string message, string icon)
        {
            return user.PrintMessageAsync(message).AsUniTask();
        }

        public UniTask SendNotificationWithImageAsync(UnturnedUser user, string message, string imageUrl)
        {
            return user.PrintMessageAsync(message, Color.White, true, imageUrl).AsUniTask();
        }

        public async UniTask SendNotificationAsync(IEnumerable<UnturnedUser> users, string message)
        {
            await UniTask.SwitchToMainThread();

            foreach (var user in users)
            {
                await SendNotificationAsync(user, message);
            }
        }

        public async UniTask SendNotificationAsync(IEnumerable<UnturnedUser> users, string message, string icon)
        {
            await UniTask.SwitchToMainThread();

            foreach (var user in users)
            {
                await SendNotificationAsync(user, message, icon);
            }
        }

        public async UniTask SendNotificationWithImageAsync(IEnumerable<UnturnedUser> users, string message, string imageUrl)
        {
            await UniTask.SwitchToMainThread();

            foreach (var user in users)
            {
                await SendNotificationWithImageAsync(user, message, imageUrl);
            }
        }

        public UniTask SendAllUsersNotificationAsync(string message) =>
            SendNotificationAsync(_userDirectory.GetOnlineUsers(), message);

        public UniTask SendAllUsersNotificationAsync(string message, string icon) =>
            SendNotificationAsync(_userDirectory.GetOnlineUsers(), message, icon);

        public UniTask SendAllUsersNotificationWithImageAsync(string message, string imageUrl) =>
            SendNotificationWithImageAsync(_userDirectory.GetOnlineUsers(), message, imageUrl);
    }
}
