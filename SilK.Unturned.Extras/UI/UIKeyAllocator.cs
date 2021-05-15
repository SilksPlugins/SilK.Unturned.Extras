using System;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Unturned.Effects;

namespace SilK.Unturned.Extras.UI
{
    [Obsolete]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    internal class UIKeyAllocator : IUIKeyAllocator
    {
        private readonly IUnturnedUIEffectsKeysProvider _keysProvider;
        private readonly IRuntime _runtime;

        public UIKeyAllocator(
            IUnturnedUIEffectsKeysProvider keysProvider,
            IRuntime runtime)
        {
            _keysProvider = keysProvider;
            _runtime = runtime;
        }

        public short GetEffectKey()
        {
            var key = _keysProvider.BindKey(_runtime);

            return key.Value;
        }

        public short GetEffectKey(string id) => GetEffectKey();

        public short GetEffectKey(ushort id) => GetEffectKey();
    }
}
