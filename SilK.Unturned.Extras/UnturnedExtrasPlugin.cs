using OpenMod.Unturned.Plugins;
using System;
using OpenMod.API.Plugins;

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
