using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Plugins;
using SilK.Unturned.Extras.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.Plugins
{
    public abstract class OpenModUnturnedExtrasPlugin : OpenModUnturnedPlugin
    {
        private readonly Dictionary<Type, MethodInfo> _subscribedMethods;

        protected OpenModUnturnedExtrasPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _subscribedMethods = new Dictionary<Type, MethodInfo>();
        }

        protected sealed override async UniTask OnLoadAsync()
        {
            await OnLoadExtraAsync();

            var eventListeners = GetType().GetInterfaces().Where(x =>
                x.IsGenericType && (typeof(IExtraEventListener<>).IsAssignableFrom(x.GetGenericTypeDefinition()) ||
                                    typeof(IAsyncExtraEventListener<>).IsAssignableFrom(x.GetGenericTypeDefinition())));

            foreach (var listener in eventListeners)
            {
                var eventType = listener.GetGenericArguments().Single();

                var method = listener.GetMethod("HandleEventAsync", BindingFlags.Public | BindingFlags.Instance);

                if (method == null)
                {
                    Logger.LogWarning(
                        $"Event callback method for type {eventType.FullName} could not be found. Plugin will not function properly.");
                    continue;
                }

                _subscribedMethods.Add(eventType, method);

                EventBus.Subscribe(this, eventType,
                    (_, sender, @event) => ProxyHandleEventAsync(sender, @event, eventType,
                        typeof(IAsyncExtraEventListener<>).IsAssignableFrom(listener.GetGenericTypeDefinition())));
            }
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

        private async Task ProxyHandleEventAsync(object? sender, IEvent @event, Type eventType, bool trueAsync)
        {
            var method = _subscribedMethods[eventType];

            var task = (UniTask) method.Invoke(this, new[] {sender, @event});

            if (trueAsync)
            {
                task.Forget();
            }
            else
            {
                await task.AsTask();
            }
        }
    }
}
