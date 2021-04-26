using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;

[assembly: PluginMetadata("SilK.Unturned.Extras", DisplayName = "SilK's Unturned Extras",
    Author = "SilK", Website = "https://github.com/IAmSilK/SilK.Unturned.Extras/")]

namespace SilK.Unturned.Extras
{
    public class UnturnedExtrasPlugin : OpenModUnturnedPlugin
    {
        public UnturnedExtrasPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
