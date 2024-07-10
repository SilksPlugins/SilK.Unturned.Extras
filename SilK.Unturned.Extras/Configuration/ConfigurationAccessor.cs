using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using OpenMod.API.Plugins;
using SilK.Unturned.Extras.Accessors;
using System;
using System.Collections.Generic;

namespace SilK.Unturned.Extras.Configuration
{
    internal class ConfigurationAccessor<TPlugin> : 
        PluginServiceAccessor<TPlugin, IConfiguration>,
        IConfigurationAccessor<TPlugin> where TPlugin : IOpenModPlugin
    {
        public ConfigurationAccessor(IServiceProvider serviceProvider) : base(serviceProvider)
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
            get => GetInstance()[key]!;
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
            set => GetInstance()[key!] = value;
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        }
    }
}
