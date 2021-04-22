using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using SilK.Unturned.Extras.Configuration;
using SilK.Unturned.Extras.Localization;

namespace SilK.Unturned.Extras
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(typeof(IConfigurationAccessor<>), typeof(ConfigurationAccessor<>));
            serviceCollection.AddTransient(typeof(IStringLocalizerAccessor<>), typeof(StringLocalizerAccessor<>));

            serviceCollection.AddTransient(typeof(IConfigurationParser<>), typeof(ConfigurationParser<>));
        }
    }
}
