using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Plugins;
using OpenMod.Core.Plugins.Events;
using SilK.Unturned.Extras.Plugins;
using System;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.Accessors
{
    public class PluginServiceAccessor<TPlugin, TService> : IDisposable
        where TPlugin : IOpenModPlugin
        where TService : class
    {
        private readonly IPluginAccessor<TPlugin> _pluginAccessor;
        private readonly ILogger? _logger;

        private IDisposable? _eventsDisposable;
        private TService? _cachedService;

        protected PluginServiceAccessor(IServiceProvider serviceProvider)
        {
            _pluginAccessor = serviceProvider.GetRequiredService<IPluginAccessor<TPlugin>>();
            _logger = serviceProvider.GetService<ILogger<PluginServiceAccessor<TPlugin, TService>>>();

            var eventBus = serviceProvider.GetService<IEventBus>();
            var runtime = serviceProvider.GetService<IRuntime>();

            _eventsDisposable = eventBus!.Subscribe<PluginLoadedEvent>(runtime!, OnPluginLoaded);
        }

        public void Dispose()
        {
            try
            {
                _eventsDisposable?.Dispose();
                _eventsDisposable = null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Exception occurred when disposing event subscription");
            }
        }

        private Task OnPluginLoaded(IServiceProvider serviceProvider, object? sender, PluginLoadedEvent @event)
        {
            if (@event.Plugin is not TPlugin)
            {
                return Task.CompletedTask;
            }

            try
            {
                _cachedService = @event.Plugin.LifetimeScope.ResolveOptional<TService>();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred when resolving service {Service} from plugin {Plugin}",
                    nameof(TService), nameof(TPlugin));
            }

            return Task.CompletedTask;
        }

        public TService GetInstance()
        {
            if (_cachedService != null)
            {
                return _cachedService;
            }

            var plugin = _pluginAccessor.Instance ?? throw new PluginNotLoadedException(typeof(TPlugin));

            _cachedService = plugin.LifetimeScope.Resolve<TService>();

            return _cachedService;
        }

        public TService? GetNullableInstance()
        {
            if (_cachedService != null)
            {
                return _cachedService;
            }

            var plugin = _pluginAccessor.Instance;
            
            _cachedService = plugin == null ? null : plugin.LifetimeScope.Resolve<TService>();

            return _cachedService;
        }
    }
}
