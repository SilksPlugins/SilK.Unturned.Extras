using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using System.Collections.Generic;

namespace SilK.Unturned.Extras.UI
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    internal class UIKeyAllocator : IUIKeyAllocator
    {
        private readonly Dictionary<string, short> _stringIdAllocations;
        private readonly Dictionary<ushort, short> _idAllocations;

        private short _nextKey;


        public UIKeyAllocator()
        {
            _stringIdAllocations = new Dictionary<string, short>();
            _idAllocations = new Dictionary<ushort, short>();

            _nextKey = -20000;
        }

        public short GetEffectKey()
        {
            return _nextKey++;
        }

        public short GetEffectKey(string id)
        {
            if (_stringIdAllocations.TryGetValue(id, out var key))
                return key;

            key = GetEffectKey();
            _stringIdAllocations.Add(id, key);

            return key;
        }

        public short GetEffectKey(ushort id)
        {
            if (_idAllocations.TryGetValue(id, out var key))
                return key;

            key = GetEffectKey();
            _idAllocations.Add(id, key);

            return key;
        }
    }
}
