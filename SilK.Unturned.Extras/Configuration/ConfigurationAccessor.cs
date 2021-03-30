using Microsoft.Extensions.Configuration;
using OpenMod.API.Plugins;
using SilK.Unturned.Extras.Accessors;

namespace SilK.Unturned.Extras.Configuration
{
    internal class ConfigurationAccessor<TPlugin> : 
        PluginServiceAccessor<TPlugin, IConfiguration>,
        IConfigurationAccessor<TPlugin> where TPlugin : IOpenModPlugin
    {
        public ConfigurationAccessor(IPluginAccessor<TPlugin> pluginAccessor) : base(pluginAccessor)
        {
        }
    }
}
