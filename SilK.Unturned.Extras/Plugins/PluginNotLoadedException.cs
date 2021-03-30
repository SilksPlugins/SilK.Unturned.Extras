using System;

namespace SilK.Unturned.Extras.Plugins
{
    /// <summary>
    /// This exception is thrown when an operation
    /// requires a plugin to be loaded, but it is not.
    /// </summary>
    public class PluginNotLoadedException : Exception
    {
        public PluginNotLoadedException(string message) : base(message)
        {
        }

        public PluginNotLoadedException(Type pluginType) : this($"Plugin is not loaded: {pluginType}")
        {
        }
    }
}
