using Microsoft.Extensions.Configuration;
using OpenMod.API.Plugins;
using SilK.Unturned.Extras.Plugins;

namespace SilK.Unturned.Extras.Configuration
{
    /// <summary>
    /// A service used to access the configuration of a plugin from outside the plugin scope.
    /// </summary>
    /// <typeparam name="TPlugin">The plugin of the target configuration.</typeparam>
    public interface IConfigurationAccessor<out TPlugin> : IConfiguration where TPlugin : IOpenModPlugin
    {
        /// <summary>
        /// Gets the configuration instance.
        /// </summary>
        /// <returns>Returns the configuration instance or throws <see cref="PluginNotLoadedException"/></returns>
        /// <exception cref="PluginNotLoadedException">Thrown when plugin is not loaded.</exception>
        IConfiguration GetInstance();

        /// <summary>
        /// Gets the configuration instance.
        /// </summary>
        /// <returns>Returns the configuration instance or <b>null</b> if the plugin is not loaded or found.</returns>
        IConfiguration? GetNullableInstance();
    }
}
