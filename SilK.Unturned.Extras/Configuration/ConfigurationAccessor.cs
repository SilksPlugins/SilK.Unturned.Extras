using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using SilK.Unturned.Extras.Accessors;

namespace SilK.Unturned.Extras.Configuration
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    internal class ConfigurationAccessor<TPlugin> : 
        PluginServiceAccessor<TPlugin, IConfiguration>,
        IConfigurationAccessor<TPlugin> where TPlugin : IOpenModPlugin
    {
        protected ConfigurationAccessor(IPluginAccessor<TPlugin> pluginAccessor) : base(pluginAccessor)
        {
        }
    }
}
