namespace SilK.Unturned.Extras.Configuration
{
    /// <summary>
    /// This service provides a wrapper for binding a
    /// configuration to a new instance of the given class type.
    /// </summary>
    /// <typeparam name="T">The type of the configuration instance.</typeparam>
    public interface IConfigurationParser<out T> where T : class
    {
        /// <summary>
        /// Attempts to bind the configuration to a new instance of
        /// the given class type. If binding fails, returns a new
        /// instance of the given class type with default values.
        /// </summary>
        T Instance { get; }
    }
}
