using Autofac;
using OpenMod.API.Plugins;
using SilK.Unturned.Extras.Plugins;

namespace SilK.Unturned.Extras.Accessors
{
    public class PluginServiceAccessor<TPlugin, TService>
        where TPlugin : IOpenModPlugin
        where TService : class
    {
        private readonly IPluginAccessor<TPlugin> _pluginAccessor;

        protected PluginServiceAccessor(IPluginAccessor<TPlugin> pluginAccessor)
        {
            _pluginAccessor = pluginAccessor;
        }

        public TService GetInstance()
        {
            var plugin = _pluginAccessor.Instance ?? throw new PluginNotLoadedException(typeof(TPlugin));

            return plugin.LifetimeScope.Resolve<TService>();
        }

        public TService? GetNullableInstance()
        {
            var plugin = _pluginAccessor.Instance;

            return plugin == null ? null : plugin.LifetimeScope.Resolve<TService>();
        }
    }
}
