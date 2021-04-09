using OpenMod.Unturned.Users;
using System;

namespace SilK.Unturned.Extras.UI
{
    public abstract class SingleEffectUISession : UISessionBase
    {
        public abstract ushort EffectId { get; }

        protected SingleEffectUISession(UnturnedUser user, IServiceProvider serviceProvider) : base(user, serviceProvider)
        {
        }


    }
}
