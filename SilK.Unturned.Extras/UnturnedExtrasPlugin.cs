using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using SilK.Unturned.Extras.Server;
using System;

[assembly: PluginMetadata("SilK.Unturned.Extras", DisplayName = "Unturned Extras")]
namespace SilK.Unturned.Extras
{
    public class UnturnedExtrasPlugin : OpenModUnturnedPlugin
    {
        private readonly ILogger<UnturnedExtrasPlugin> _logger;
        private readonly IServerHelper _serverHelper;

        public UnturnedExtrasPlugin(
            ILogger<UnturnedExtrasPlugin> logger,
            IServerHelper serverHelper,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
            _serverHelper = serverHelper;
        }

        protected override async UniTask OnLoadAsync()
        {
        }

        protected override async UniTask OnUnloadAsync()
        {
        }
    }
}
