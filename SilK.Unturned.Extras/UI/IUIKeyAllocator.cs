using OpenMod.API.Ioc;
using System;

namespace SilK.Unturned.Extras.UI
{
    /// <summary>
    /// A service which allocates UI effect keys to help prevent key overlapping.
    /// </summary>
    [Obsolete("Use OpenMod.Unturned.Effects.IUnturnedUIEffectsKeysProvider instead. This will be removed in v2.0.")]
    [Service]
    public interface IUIKeyAllocator
    {
        /// <summary>
        /// Gets an effect key not bound to a given id.
        /// </summary>
        /// <returns>An effect key not bound to a given id.</returns>
        [Obsolete("Use OpenMod.Unturned.Effects.IUnturnedUIEffectsKeysProvider instead. This will be removed in v2.0.")]
        short GetEffectKey();

        /// <summary>
        /// Gets an effect key bound to a given id.
        /// Retrieving an effect key with this id again will return the same key.
        /// </summary>
        /// <param name="id">The id the key will be bound to.</param>
        /// <returns>An effect key bound to the given id.</returns>
        [Obsolete("Use OpenMod.Unturned.Effects.IUnturnedUIEffectsKeysProvider instead. This will be removed in v2.0.")]
        short GetEffectKey(string id);

        /// <summary>
        /// Gets an effect key bound to a given id.
        /// Retrieving an effect key with this id again will return the same key.
        /// </summary>
        /// <param name="id">The id the key will be bound to.</param>
        /// <returns>An effect key bound to the given id.</returns>
        [Obsolete("Use OpenMod.Unturned.Effects.IUnturnedUIEffectsKeysProvider instead. This will be removed in v2.0.")]
        short GetEffectKey(ushort id);
    }
}
