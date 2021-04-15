using OpenMod.Unturned.Users;
using System;

namespace SilK.Unturned.Extras.UI
{
    public abstract class SingleEffectUISession : UISessionBase
    {
        public abstract ushort EffectId { get; }

        private short? _effectKey;
        protected short EffectKey => _effectKey ??= KeyAllocator.GetEffectKey(EffectId);

        protected SingleEffectUISession(UnturnedUser user, IServiceProvider serviceProvider)
            : base(user, serviceProvider)
        {
        }

        protected void SendUIEffect() =>
            SendUIEffectWithKey(EffectId, EffectKey);

        protected void SendUIEffect(object arg0) =>
            SendUIEffectWithKey(EffectId, EffectKey, arg0);

        protected void SendUIEffect(object arg0, object arg1) =>
            SendUIEffectWithKey(EffectId, EffectKey, arg0, arg1);

        protected void SendUIEffect(object arg0, object arg1, object arg2) =>
            SendUIEffectWithKey(EffectId, EffectKey, arg0, arg1, arg2);

        protected void SendUIEffect(object arg0, object arg1, object arg2, object arg3) =>
            SendUIEffectWithKey(EffectId, EffectKey, arg0, arg1, arg2, arg3);

        protected void SendText(string childName, string text) =>
            SendTextWithKey(EffectKey, childName, text);

        protected void SendVisibility(string childName, bool visible) =>
            SendVisibilityWithKey(EffectKey, childName, visible);

        protected void SendImage(string childName, string url, bool shouldCache = true, bool forceRefresh = false) =>
            SendImageWithKey(EffectKey, childName, url, shouldCache, forceRefresh);

        protected void ClearEffect() => ClearEffect(EffectId);
    }
}
