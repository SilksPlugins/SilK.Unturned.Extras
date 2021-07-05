using Microsoft.Extensions.Configuration;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Core.Plugins.Events;
using System;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.Configuration
{
    internal class ConfigurationParser<T> : IConfigurationParser<T>, IDisposable where T : class, new()
    {
        private readonly IConfiguration _configuration;
        private readonly IDisposable _disposable;

        public ConfigurationParser(IConfiguration configuration,
            IEventBus eventBus,
            IOpenModComponent component)
        {
            _configuration = configuration;

            _disposable = eventBus.Subscribe(component,
                (EventCallback<PluginConfigurationChangedEvent>) OnConfigurationChangedEvent);
        }

        private T? _instance;

        private T CreateInstance()
        {
            return _configuration.Get<T>() ?? new T();
        }

        public T Instance => _instance ??= CreateInstance();

        private Task OnConfigurationChangedEvent(IServiceProvider serviceProvider, object? sender,
            PluginConfigurationChangedEvent @event)
        {
            if (_instance != null)
            {
                _instance = CreateInstance();
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
