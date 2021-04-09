using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
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
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.UI
{
    public abstract class UISessionBase : IUISession, IAsyncDisposable,
        IInstanceAsyncEventListener<UnturnedPlayerButtonClickedEvent>,
        IInstanceAsyncEventListener<UnturnedPlayerTextInputtedEvent>
    {
        public abstract string Id { get; }

        public UnturnedUser User { get; }

        protected ITransportConnection TransportConnection => User.Player.SteamPlayer.transportConnection;

        private readonly IEventSubscriber _eventSubscriber;
        private readonly IOpenModComponent _openModComponent;

        private readonly List<(object, ButtonClickedCallback)> _buttonClickedCallbacks;
        private readonly List<(object, TextInputtedCallback)> _textInputtedCallbacks;

        private IDisposable? _eventsDisposable;
        private bool _isEnded;
        private bool _isDisposed;

        protected UISessionBase(
            UnturnedUser user,
            IServiceProvider serviceProvider)
        {
            User = user;

            _eventSubscriber = serviceProvider.GetRequiredService<IEventSubscriber>();
            _openModComponent = serviceProvider.GetRequiredService<IOpenModComponent>();

            _buttonClickedCallbacks = new List<(object, ButtonClickedCallback)>();
            _textInputtedCallbacks = new List<(object, TextInputtedCallback)>();
        }

        public async UniTask StartAsync()
        {
            _eventsDisposable = _eventSubscriber.Subscribe(this, _openModComponent);

            await OnStartAsync();
        }

        protected virtual UniTask OnStartAsync() => UniTask.CompletedTask;

        public async UniTask EndAsync()
        {
            if (_isEnded) return;
            _isEnded = true;

            _eventsDisposable?.Dispose();

            await OnEndAsync();

            await DisposeAsync();
        }

        protected virtual UniTask OnEndAsync() => UniTask.CompletedTask;

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            await EndAsync();
        }

        protected short GetEffectKeyFromId(ushort id)
        {
            // id:       0 - 65535
            // key: -32768 - 32767

            return (short)(id + short.MinValue);
        }

        protected void SendUIEffect(ushort effectId)
        {
            EffectManager.sendUIEffect(effectId, EffectKeyFromId(effectId), TransportConnection, true);
        }

        protected void SendUIEffect(ushort effectId, object arg0)
        {
            EffectManager.sendUIEffect(effectId, EffectKeyFromId(effectId), TransportConnection, true, arg0.ToString());
        }
        
        protected void SendUIEffect(ushort effectId, object arg0, object arg1)
        {
            EffectManager.sendUIEffect(effectId, EffectKeyFromId(effectId), TransportConnection, true, arg0.ToString(),
                arg1.ToString());
        }

        protected void SendUIEffect(ushort effectId, object arg0, object arg1, object arg2)
        {
            EffectManager.sendUIEffect(effectId, EffectKeyFromId(effectId), TransportConnection, true, arg0.ToString(),
                arg1.ToString(), arg2.ToString());
        }

        protected void SendUIEffect(ushort effectId, object arg0, object arg1, object arg2, object arg3)
        {
            EffectManager.sendUIEffect(effectId, EffectKeyFromId(effectId), TransportConnection, true, arg0.ToString(),
                arg1.ToString(), arg2.ToString(), arg3.ToString());
        }

        protected void SendText(short key, string childName, string text)
        {
            EffectManager.sendUIEffectText(key, TransportConnection, true, childName, text);
        }

        protected void SendVisibility(short key, string childName, bool visible)
        {
            EffectManager.sendUIEffectVisibility(key, TransportConnection, true, childName, visible);
        }

        protected void SendImage(short key, string childName, string url, bool shouldCache = true, bool forceRefresh = false)
        {
            EffectManager.sendUIEffectImageURL(key, TransportConnection, true, childName, url, shouldCache,
                forceRefresh);
        }

        protected void ClearEffect(ushort id)
        {
            EffectManager.askEffectClearByID(id, TransportConnection);
        }

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

        public async UniTask HandleEventAsync(object? sender, UnturnedPlayerButtonClickedEvent @event)
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

        public async UniTask HandleEventAsync(object? sender, UnturnedPlayerTextInputtedEvent @event)
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
    }
}
