using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using OpenMod.API.Plugins;
using SilK.Unturned.Extras.Accessors;
using System.Collections.Generic;

namespace SilK.Unturned.Extras.Configuration
{
    internal class ConfigurationAccessor<TPlugin> : 
        PluginServiceAccessor<TPlugin, IConfiguration>,
        IConfigurationAccessor<TPlugin> where TPlugin : IOpenModPlugin
    {
        public ConfigurationAccessor(IPluginAccessor<TPlugin> pluginAccessor) : base(pluginAccessor)
        {
        }

        /// <inheritdoc cref="IConfiguration"/>
        public IConfigurationSection GetSection(string key) => GetInstance().GetSection(key);

        /// <inheritdoc cref="IConfiguration"/>
        public IEnumerable<IConfigurationSection> GetChildren() => GetInstance().GetChildren();

        /// <inheritdoc cref="IConfiguration"/>
        public IChangeToken GetReloadToken() => GetInstance().GetReloadToken();

        /// <inheritdoc cref="IConfiguration"/>
        public string this[string key]
        {
            get => GetInstance()[key];
            set => GetInstance()[key] = value;
        }
    }
}
