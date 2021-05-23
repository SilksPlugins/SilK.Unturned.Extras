using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;

[assembly: PluginMetadata("SilK.Unturned.Extras", DisplayName = "SilK's Unturned Extras",
    Author = "SilK", Website = "https://github.com/IAmSilK/SilK.Unturned.Extras/")]

namespace SilK.Unturned.Extras
{
    /// <summary>
    /// The central plugin which manages SilK.Unturned.Extras services.
    /// </summary>
    public class UnturnedExtrasPlugin : OpenModUnturnedPlugin
    {
        /// <summary>
        /// Creates an instance of this plugin. Should not be called except by OpenMod.
        /// </summary>
        public UnturnedExtrasPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
