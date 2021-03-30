using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.Unturned.Plugins;
using SilK.Unturned.Extras.Events;
using System;

namespace SilK.Unturned.Extras.Plugins
{
    public abstract class OpenModUnturnedExtrasPlugin : OpenModUnturnedPlugin
    {
        private readonly IEventSubscriber _eventSubscriber;

        protected OpenModUnturnedExtrasPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _eventSubscriber = serviceProvider.GetRequiredService<IEventSubscriber>();
        }

        protected sealed override async UniTask OnLoadAsync()
        {
            await OnLoadExtraAsync();

            _eventSubscriber.SubscribeEvents(this, this);
        }

        protected sealed override async UniTask OnUnloadAsync()
        {
            await OnUnloadExtraAsync();


        }

        protected virtual UniTask OnLoadExtraAsync()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnUnloadExtraAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}
