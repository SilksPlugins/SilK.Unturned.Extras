using Microsoft.Extensions.Localization;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using SilK.Unturned.Extras.Plugins;

namespace SilK.Unturned.Extras.Localization
{
    /// <summary>
    /// A service used to access the string localizer of a plugin from outside the plugin scope.
    /// </summary>
    /// <typeparam name="TPlugin">The plugin of the target string localizer.</typeparam>
    [Service]
    public interface IStringLocalizerAccessor<out TPlugin> where TPlugin : IOpenModPlugin
    {

        /// <summary>
        /// Gets the string localizer instance.
        /// </summary>
        /// <returns>Returns the string localizer instance or throws <see cref="PluginNotLoadedException"/></returns>
        /// <exception cref="PluginNotLoadedException">Thrown when plugin is not loaded.</exception>
        IStringLocalizer GetInstance();

        /// <summary>
        /// Gets the string localizer instance.
        /// </summary>
        /// <returns>Returns the string localizer instance or <b>null</b> if the plugin is not loaded or found.</returns>
        IStringLocalizer? GetNullableInstance();
    }
}
