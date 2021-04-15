using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Users;
using SilK.Unturned.Extras.UI;
using System;

namespace UnturnedExtrasTest.UI
{
    public class TestUISession : SingleEffectUISession
    {
        public TestUISession(UnturnedUser user,
            IServiceProvider serviceProvider) : base(user, serviceProvider)
        {
        }

        public override string Id => "SilK.TestUI";

        public override ushort EffectId => 20000;

        protected override async UniTask OnStartAsync()
        {
            SubscribeButtonClick("ExitButton", OnExitClickedAsync);

            await UniTask.SwitchToMainThread();

            SendUIEffect();
        }

        protected override async UniTask OnEndAsync()
        {
            await UniTask.SwitchToMainThread();

            ClearEffect();
        }

        private async UniTask OnExitClickedAsync(string buttonName)
        {
            await EndAsync();
        }
    }
}
