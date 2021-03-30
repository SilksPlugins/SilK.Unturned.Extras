using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using SilK.Unturned.Extras.Accessors;

namespace SilK.Unturned.Extras.Localization
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    internal class StringLocalizerAccessor<TPlugin> :
        PluginServiceAccessor<TPlugin, IStringLocalizer>,
        IStringLocalizerAccessor<TPlugin> where TPlugin : IOpenModPlugin
    {
        protected StringLocalizerAccessor(IPluginAccessor<TPlugin> pluginAccessor) : base(pluginAccessor)
        {
        }
    }
}
