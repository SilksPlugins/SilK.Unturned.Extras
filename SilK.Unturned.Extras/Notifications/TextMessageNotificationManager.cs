using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Unturned.Users;
using System.Drawing;

namespace SilK.Unturned.Extras.Notifications
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    internal class TextMessageNotificationManager : INotificationManager
    {
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
    }
}
