using Microsoft.Extensions.Localization;
using OpenMod.API.Plugins;
using SilK.Unturned.Extras.Accessors;

namespace SilK.Unturned.Extras.Localization
{
    internal class StringLocalizerAccessor<TPlugin> :
        PluginServiceAccessor<TPlugin, IStringLocalizer>,
        IStringLocalizerAccessor<TPlugin> where TPlugin : IOpenModPlugin
    {
        public StringLocalizerAccessor(IPluginAccessor<TPlugin> pluginAccessor) : base(pluginAccessor)
        {
        }
    }
}
