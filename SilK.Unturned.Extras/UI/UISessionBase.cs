using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Core.Helpers;
using OpenMod.Unturned.Players.UI.Events;
using OpenMod.Unturned.Users;
using SDG.NetTransport;
using SDG.Unturned;
using SilK.Unturned.Extras.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.UI
{
    public abstract class UISessionBase : IUISession, IAsyncDisposable
    {
        public abstract string Id { get; }

        public UnturnedUser User { get; }

        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public event Action<IUISession>? OnUISessionEnded;

        protected ITransportConnection TransportConnection => User.Player.SteamPlayer.transportConnection;

        protected readonly IUIManager UIManager;
        protected readonly IUIKeyAllocator KeyAllocator;

        private readonly List<(object, ButtonClickedCallback)> _buttonClickedCallbacks;
        private readonly List<(object, TextInputtedCallback)> _textInputtedCallbacks;

        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly List<IDisposable> _eventsDisposable;
        private bool _isEnded;
        private bool _isDisposed;

        protected UISessionBase(
            UnturnedUser user,
            IServiceProvider serviceProvider)
        {
            User = user;

            UIManager = serviceProvider.GetRequiredService<IUIManager>();
            KeyAllocator = serviceProvider.GetRequiredService<IUIKeyAllocator>();

            _cancellationTokenSource = new CancellationTokenSource();

            _buttonClickedCallbacks = new List<(object, ButtonClickedCallback)>();
            _textInputtedCallbacks = new List<(object, TextInputtedCallback)>();
            
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            var component = serviceProvider.GetRequiredService<IOpenModComponent>();

            var subscriber = serviceProvider.GetRequiredService<IEventSubscriber>();

            _eventsDisposable = new List<IDisposable>
            {
                eventBus.Subscribe(component, (EventCallback<UnturnedPlayerButtonClickedEvent>)OnButtonClickedAsync),
                eventBus.Subscribe(component, (EventCallback<UnturnedPlayerTextInputtedEvent>)OnTextInputtedAsync),
                subscriber.Subscribe(this, component)
            };
        }

        public async UniTask StartAsync()
        {
            await OnStartAsync();
        }

        protected virtual UniTask OnStartAsync() => UniTask.CompletedTask;

        public async UniTask EndAsync()
        {
            if (_isEnded) return;
            _isEnded = true;

            await _eventsDisposable.DisposeAllAsync();

            lock(_buttonClickedCallbacks) _buttonClickedCallbacks.Clear();
            lock (_textInputtedCallbacks) _textInputtedCallbacks.Clear();

            _cancellationTokenSource.Cancel();

            await OnEndAsync();

            OnUISessionEnded?.Invoke(this);
        }

        protected virtual UniTask OnEndAsync() => UniTask.CompletedTask;

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            await EndAsync();

            await OnDisposeAsync();
        }

        protected virtual UniTask OnDisposeAsync() => UniTask.CompletedTask;

        protected void SendUIEffectWithKey(ushort effectId, short key)
        {
            EffectManager.sendUIEffect(effectId, key, TransportConnection, true);
        }

        protected void SendUIEffectWithKey(ushort effectId, short key, object arg0)
        {
            EffectManager.sendUIEffect(effectId, key, TransportConnection, true, arg0.ToString());
        }
        
        protected void SendUIEffectWithKey(ushort effectId, short key, object arg0, object arg1)
        {
            EffectManager.sendUIEffect(effectId, key, TransportConnection, true, arg0.ToString(),
                arg1.ToString());
        }

        protected void SendUIEffectWithKey(ushort effectId, short key, object arg0, object arg1, object arg2)
        {
            EffectManager.sendUIEffect(effectId, key, TransportConnection, true, arg0.ToString(),
                arg1.ToString(), arg2.ToString());
        }

        protected void SendUIEffectWithKey(ushort effectId, short key, object arg0, object arg1, object arg2, object arg3)
        {
            EffectManager.sendUIEffect(effectId, key, TransportConnection, true, arg0.ToString(),
                arg1.ToString(), arg2.ToString(), arg3.ToString());
        }

        protected void SendTextWithKey(short key, string childName, string text)
        {
            EffectManager.sendUIEffectText(key, TransportConnection, true, childName, text);
        }

        protected void SendVisibilityWithKey(short key, string childName, bool visible)
        {
            EffectManager.sendUIEffectVisibility(key, TransportConnection, true, childName, visible);
        }

        protected void SendImageWithKey(short key, string childName, string url, bool shouldCache = true, bool forceRefresh = false)
        {
            EffectManager.sendUIEffectImageURL(key, TransportConnection, true, childName, url, shouldCache,
                forceRefresh);
        }

        protected void ClearEffect(ushort id)
        {
            EffectManager.askEffectClearByID(id, TransportConnection);
        }

        protected UniTask<bool> IsCursorEnabled() => UIManager.IsCursorEnabled(User);

        protected UniTask SetCursor(bool enabled) => UIManager.SetCursor(User, Id, enabled);

        protected delegate UniTask ButtonClickedCallback(string buttonName);

        protected IDisposable SubscribeButtonClick(string buttonName, ButtonClickedCallback callback, bool isPattern = false)
        {
            object pattern;

            if (isPattern)
            {
                pattern = new Regex(buttonName);
            }
            else
            {
                pattern = buttonName;
            }

            var callbackRecord = (pattern, callback);

            lock (_buttonClickedCallbacks)
            {
                _buttonClickedCallbacks.Add(callbackRecord);
            }

            return new DisposeAction(() =>
            {
                lock (_buttonClickedCallbacks)
                {
                    _buttonClickedCallbacks.Remove(callbackRecord);
                }
            });
        }

        protected delegate UniTask TextInputtedCallback(string textInputName, string text);

        protected IDisposable SubscribeTextInputted(string textInputName, TextInputtedCallback callback,
            bool isPattern = false)
        {
            object pattern;

            if (isPattern)
            {
                pattern = new Regex(textInputName);
            }
            else
            {
                pattern = textInputName;
            }

            var callbackRecord = (pattern, callback);

            lock (_textInputtedCallbacks)
            {
                _textInputtedCallbacks.Add(callbackRecord);
            }

            return new DisposeAction(() =>
            {
                lock (_textInputtedCallbacks)
                {
                    _textInputtedCallbacks.Remove(callbackRecord);
                }
            });
        }

        protected virtual UniTask ButtonClickedAsync(string buttonName) => UniTask.CompletedTask;

        protected virtual UniTask TextInputtedAsync(string textInputName, string text) => UniTask.CompletedTask;

        public Task OnButtonClickedAsync(IServiceProvider serviceProvider, object? sender,
            UnturnedPlayerButtonClickedEvent @event)
        {
            async UniTask HandleEvent()
            {
                if (User.SteamId != @event.Player.SteamId) return;

                List<(object, ButtonClickedCallback)> callbacks;

                lock (_buttonClickedCallbacks)
                {
                    callbacks = _buttonClickedCallbacks.ToList();
                }

                foreach (var (pattern, callback) in callbacks)
                {
                    switch (pattern)
                    {
                        case string strPattern:
                            if (strPattern.Equals(@event.ButtonName))
                                await callback(@event.ButtonName);
                            break;

                        case Regex regex:
                            if (regex.IsMatch(@event.ButtonName))
                                await callback(@event.ButtonName);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid button name pattern matching type: " +
                                                                pattern.GetType());
                    }
                }

                await ButtonClickedAsync(@event.ButtonName);
            }

            HandleEvent().Forget();

            return Task.CompletedTask;
        }

        public Task OnTextInputtedAsync(IServiceProvider serviceProvider, object? sender,
            UnturnedPlayerTextInputtedEvent @event)
        {
            async UniTask HandleEvent()
            {
                if (User.SteamId != @event.Player.SteamId) return;

                List<(object, TextInputtedCallback)> callbacks;

                lock (_textInputtedCallbacks)
                {
                    callbacks = _textInputtedCallbacks.ToList();
                }

                foreach (var (pattern, callback) in callbacks)
                {
                    switch (pattern)
                    {
                        case string strPattern:
                            if (strPattern.Equals(@event.TextInputName))
                                await callback(@event.TextInputName, @event.Text);
                            break;

                        case Regex regex:
                            if (regex.IsMatch(@event.TextInputName))
                                await callback(@event.TextInputName, @event.Text);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid text input name pattern matching type: " +
                                                                pattern.GetType());
                    }
                }

                await TextInputtedAsync(@event.TextInputName, @event.Text);
            }

            HandleEvent().Forget();

            return Task.CompletedTask;
        }
    }
}
