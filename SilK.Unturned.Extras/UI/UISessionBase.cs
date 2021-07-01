using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Core.Helpers;
using OpenMod.Unturned.Effects;
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
using Action = System.Action;

namespace SilK.Unturned.Extras.UI
{
    /// <summary>
    /// An implementation of <see cref="IUISession"/> which
    /// provides basic functionality for UI sessions to use.
    /// </summary>
    public abstract class UISessionBase : IUISession, IAsyncDisposable
    {
        /// <inheritdoc cref="IUISession.Id"/>
        public abstract string Id { get; }

        /// <summary>
        /// The user this UI session pertains to.
        /// </summary>
        public UnturnedUser User { get; }

        /// <summary>
        /// The cancellation token used to cancel tasks when the session is ended.
        /// </summary>
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        /// <inheritdoc cref="IUISession.OnUISessionEnded"/>
        public event Action<IUISession>? OnUISessionEnded;

        /// <summary>
        /// The transport connection of the User used
        /// with <see cref="EffectManager"/> to transmit UI information.
        /// </summary>
        protected ITransportConnection TransportConnection => User.Player.SteamPlayer.transportConnection;

        /// <summary>
        /// The <see cref="IUIManager"/> service.
        /// </summary>
        protected readonly IUIManager UIManager;

        /// <summary>
        /// The <b>obsolete</b> <see cref="IUIKeyAllocator"/> service. This property will be removed in v2.0.
        /// </summary>
        [Obsolete("Use KeysProvider instead. This will be removed in v2.0.")]
        protected readonly IUIKeyAllocator KeyAllocator;

        /// <summary>
        /// The <see cref="IUnturnedUIEffectsKeysProvider"/> service.
        /// </summary>
        protected readonly IUnturnedUIEffectsKeysProvider KeysProvider;

        /// <summary>
        /// The <see cref="IOpenModComponent"/> resolved by the provided lifetime scope.
        /// </summary>
        protected readonly IOpenModComponent OpenModComponent;

        private readonly List<(object, ButtonClickedCallback)> _buttonClickedCallbacks;
        private readonly List<(object, TextInputtedCallback)> _textInputtedCallbacks;

        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly List<IDisposable> _eventsDisposable;
        private bool _isEnded;
        private bool _isDisposed;

        /// <summary>
        /// Creates an instance of <see cref="UISessionBase"/>.
        /// </summary>
        /// <param name="user">The user this UI session pertains to.</param>
        /// <param name="serviceProvider">The provider used to resolve services.</param>
        protected UISessionBase(
            UnturnedUser user,
            IServiceProvider serviceProvider)
        {
            User = user;

            UIManager = serviceProvider.GetRequiredService<IUIManager>();
            KeyAllocator = serviceProvider.GetRequiredService<IUIKeyAllocator>();
            KeysProvider = serviceProvider.GetRequiredService<IUnturnedUIEffectsKeysProvider>();
            OpenModComponent = serviceProvider.GetRequiredService<IOpenModComponent>();

            _cancellationTokenSource = new CancellationTokenSource();

            _buttonClickedCallbacks = new List<(object, ButtonClickedCallback)>();
            _textInputtedCallbacks = new List<(object, TextInputtedCallback)>();
            
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();

            var subscriber = serviceProvider.GetRequiredService<IEventSubscriber>();

            _eventsDisposable = new List<IDisposable>
            {
                eventBus.Subscribe(OpenModComponent, (EventCallback<UnturnedPlayerButtonClickedEvent>)OnButtonClickedAsync),
                eventBus.Subscribe(OpenModComponent, (EventCallback<UnturnedPlayerTextInputtedEvent>)OnTextInputtedAsync),
                subscriber.Subscribe(this, OpenModComponent)
            };
        }

        /// <inheritdoc cref="IUISession.StartAsync"/>
        public async UniTask StartAsync()
        {
            await OnStartAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual UniTask OnStartAsync() => UniTask.CompletedTask;

        /// <inheritdoc cref="IUISession.EndAsync"/>
        public async UniTask EndAsync()
        {
            if (_isEnded) return;
            _isEnded = true;

            await _eventsDisposable.DisposeAllAsync();

            lock(_buttonClickedCallbacks) _buttonClickedCallbacks.Clear();
            lock (_textInputtedCallbacks) _textInputtedCallbacks.Clear();

            _cancellationTokenSource.Cancel();

            await SetCursor(false);

            await OnEndAsync();

            OnUISessionEnded?.Invoke(this);
        }

        protected virtual UniTask OnEndAsync() => UniTask.CompletedTask;

        internal event Action? Dispose;

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            await EndAsync();

            await OnDisposeAsync();

            Dispose?.Invoke();
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

            UniTask.RunOnThreadPool(HandleEvent, cancellationToken: CancellationToken).Forget();

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

            UniTask.RunOnThreadPool(HandleEvent, cancellationToken: CancellationToken).Forget();

            return Task.CompletedTask;
        }
    }
}
