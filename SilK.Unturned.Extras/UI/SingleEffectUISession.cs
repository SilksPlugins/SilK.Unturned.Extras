using OpenMod.Unturned.Effects;
using OpenMod.Unturned.Users;
using System;

namespace SilK.Unturned.Extras.UI
{
    /// <summary>
    /// An implementation of <see cref="UISessionBase"/> which
    /// provides wrapper methods to support a UI with only one effect.
    /// </summary>
    public abstract class SingleEffectUISession : UISessionBase
    {
        /// <summary>
        /// The single effect ID to be used by wrapper methods.
        /// </summary>
        public abstract ushort EffectId { get; }

        private UnturnedUIEffectKey? _effectKey;

        /// <summary>
        /// The effect key used alongside the provided <see cref="EffectId"/>.
        /// </summary>
        protected short EffectKey => (_effectKey ??= KeysProvider.BindKey(OpenModComponent)).Value;

        protected SingleEffectUISession(UnturnedUser user, IServiceProvider serviceProvider)
            : base(user, serviceProvider)
        {
            Dispose += OnDispose;
        }

        private void OnDispose()
        {
            Dispose -= OnDispose;

            if (_effectKey != null)
            {
                KeysProvider.ReleaseKey(OpenModComponent, _effectKey.Value);
            }
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
