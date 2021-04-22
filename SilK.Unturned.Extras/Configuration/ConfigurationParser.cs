using Microsoft.Extensions.Configuration;

namespace SilK.Unturned.Extras.Configuration
{
    internal class ConfigurationParser<T> : IConfigurationParser<T> where T : class, new()
    {
        private readonly IConfiguration _configuration;

        public ConfigurationParser(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T Instance => _configuration.Get<T>() ?? new T();
    }
}
