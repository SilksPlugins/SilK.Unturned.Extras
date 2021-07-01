using Autofac;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.Core.Ioc;
using OpenMod.Unturned.Players.Life.Events;
using OpenMod.Unturned.Users;
using OpenMod.Unturned.Users.Events;
using SDG.Unturned;
using SilK.Unturned.Extras.Events;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.Unturned.Server.Events;
using Priority = OpenMod.API.Prioritization.Priority;

namespace SilK.Unturned.Extras.UI
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    internal class UIManager : IUIManager, IAsyncDisposable,
        IInstanceAsyncEventListener<UnturnedUserDisconnectedEvent>,
        IInstanceAsyncEventListener<UnturnedPlayerDeathEvent>,
        IInstanceAsyncEventListener<UnturnedShutdownCommencedEvent>
    {
        public class UISessions
        {
            public UnturnedUser User { get; }

            public List<(IUISession Session, UISessionOptions? Options)> Sessions { get; }

            public List<(IUISession Session, UISessionOptions? Options)> GetSafeSessionsList()
            {
                lock (Sessions)
                {
                    return Sessions.ToList();
                }
            }

            public UISessions(UnturnedUser user)
            {
                User = user;
                Sessions = new List<(IUISession session, UISessionOptions? options)>();
            }
        }

        private readonly ILifetimeScope _lifetimeScope;
        private readonly IUnturnedUserDirectory _userDirectory;
        private readonly ILogger<UIManager> _logger;

        private readonly Dictionary<CSteamID, UISessions> _uiSessions;
        private readonly HashSet<string> _enabledCursorIds;

        private bool _isDisposed;

        public UIManager(ILifetimeScope lifetimeScope,
            IUnturnedUserDirectory userDirectory,
            ILogger<UIManager> logger)
        {
            _lifetimeScope = lifetimeScope;
            _userDirectory = userDirectory;
            _logger = logger;

            _uiSessions = new Dictionary<CSteamID, UISessions>();
            _enabledCursorIds = new HashSet<string>();

            OnArenaClear += Events_OnArenaClear;
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            OnArenaClear -= Events_OnArenaClear;

            List<IUISession> sessions;

            lock (_uiSessions)
            {
                sessions = _uiSessions.Values.SelectMany(uiSessions => uiSessions.Sessions)
                    .Select(session => session.Session).ToList();

                _uiSessions.Clear();
            }

            foreach (var session in sessions)
            {
                try
                {
                    await session.EndAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred when disposing UI session");
                }
            }

            _enabledCursorIds.Clear();
        }

        public async UniTask HandleEventAsync(object? sender, UnturnedShutdownCommencedEvent @event)
        {
            await DisposeAsync();
        }

        private UISessions GetUISessions(UnturnedUser user)
        {
            lock (_uiSessions)
            {
                if (_uiSessions.TryGetValue(user.SteamId, out var sessions))
                {
                    return sessions;
                }

                sessions = new UISessions(user);

                _uiSessions.Add(user.SteamId, sessions);

                return sessions;
            }
        }

        private TSession CreateInstance<TSession>(UnturnedUser user, ILifetimeScope? scope)
        {
            return ActivatorUtilitiesEx.CreateInstance<TSession>(scope ?? _lifetimeScope, user);
        }

        private void OnUISessionEnded(UISessions sessions, IUISession session)
        {
            lock (sessions.Sessions)
            {
                sessions.Sessions.RemoveAll(x => x.Session == session);
            }
        }

        public async UniTask<TSession> StartSession<TSession>(UnturnedUser user, UISessionOptions? options = null,
            ILifetimeScope? lifetimeScope = null) where TSession : class, IUISession
        {
            await EndSession<TSession>(user);

            var sessions = GetUISessions(user);

            var session = CreateInstance<TSession>(user, lifetimeScope);

            lock (sessions.Sessions)
            {
                sessions.Sessions.Add((session, options));
            }

            session.OnUISessionEnded += (s) => OnUISessionEnded(sessions, s);

            await session.StartAsync();

            return session;
        }

        public async UniTask<bool> EndSession<TSession>(UnturnedUser user) where TSession : class, IUISession
        {
            var sessions = GetUISessions(user);

            IUISession? session;

            lock (sessions.Sessions)
            {
                session = sessions.Sessions.Select(x => x.Session).FirstOrDefault(x => x.GetType() == typeof(TSession));
            }

            if (session != null)
            {
                await session.EndAsync();
            }

            return session != null;
        }

        public async UniTask EndAllSessions<TSession>() where TSession : class, IUISession
        {
            List<IUISession> sessions;

            lock (_uiSessions)
            {
                sessions = _uiSessions.Select(pair => pair.Value.Sessions)
                    .SelectMany(sessions => sessions)
                    .Select(session => session.Session)
                    .Where(x => x.GetType() == typeof(TSession)).ToList();
            }

            foreach (var session in sessions)
            {
                try
                {
                    await session.EndAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred when ending UI session");
                }
            }
        }

        public UniTask<TSession?> GetSession<TSession>(UnturnedUser user) where TSession : class, IUISession
        {
            var sessions = GetUISessions(user);

            TSession? session;

            lock (sessions.Sessions)
            {
                session =
                    sessions.Sessions.Select(x => x.Session)
                        .FirstOrDefault(x => x.GetType() == typeof(TSession)) as TSession;
            }

            return UniTask.FromResult(session);
        }

        public UniTask<IReadOnlyCollection<IUISession>> GetSessions(UnturnedUser user)
        {
            var sessions = GetUISessions(user);

            lock (sessions.Sessions)
            {
                return UniTask.FromResult<IReadOnlyCollection<IUISession>>(
                    sessions.Sessions.Select(x => x.Session).ToList().AsReadOnly());
            }
        }

        public async UniTask<TSession> GetOrStartSession<TSession>(UnturnedUser user, UISessionOptions? options = null,
            ILifetimeScope? lifetimeScope = null) where TSession : class, IUISession
        {
            var session = await GetSession<TSession>(user);

            return session ?? await StartSession<TSession>(user, options, lifetimeScope);
        }

        public async UniTask HandleEventAsync(object? sender, UnturnedUserDisconnectedEvent @event)
        {
            var sessions = GetUISessions(@event.User);

            foreach (var (session, _) in sessions.GetSafeSessionsList())
            {
                try
                {
                    await session.EndAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred when ending UI session");
                }
            }
        }

        public async UniTask HandleEventAsync(object? sender, UnturnedPlayerDeathEvent @event)
        {
            var user = _userDirectory.GetUser(@event.Player.Player);

            var sessions = GetUISessions(user);

            var sessionsToEnd = sessions.GetSafeSessionsList()
                .Where(x => x.Options != null && x.Options.EndOnDeath)
                .Select(x => x.Session);

            foreach (var session in sessionsToEnd)
            {
                try
                {
                    await session.EndAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred when ending UI session");
                }
            }
        }

        public async UniTask<bool> IsCursorEnabled(UnturnedUser user)
        {
            await UniTask.SwitchToMainThread();

            return user.Player.Player.isPluginWidgetFlagActive(EPluginWidgetFlags.Modal);
        }

        public async UniTask SetCursor(UnturnedUser user, string id, bool enabled)
        {
            await UniTask.SwitchToMainThread();

            if (enabled)
            {
                _enabledCursorIds.Add(id);
                user.Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
            }
            else
            {
                _enabledCursorIds.Remove(id);

                if (_enabledCursorIds.Count == 0)
                {
                    user.Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                }
            }
        }

        private void Events_OnArenaClear()
        {
            async UniTask ArenaClearTask()
            {
                var sessionsToEnd = new List<IUISession>();

                lock (_uiSessions)
                {
                    foreach (var uiSessions in _uiSessions.Values)
                    {
                        var sessions = uiSessions.Sessions.Where(x => x.Options != null && x.Options.EndOnArenaClear);

                        sessionsToEnd.AddRange(sessions.Select(x => x.Session));
                    }
                }

                foreach (var session in sessionsToEnd)
                {
                    try
                    {
                        await session.EndAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred when ending UI session");
                    }
                }
            }

            ArenaClearTask().Forget();
        }

        private delegate void ArenaClear();
        private static event ArenaClear? OnArenaClear;

        [HarmonyPatch]
        private static class UnturnedPatches
        {
            [HarmonyPatch(typeof(LevelManager), "arenaClear")]
            [HarmonyPrefix]
            private static void ArenaClear()
            {
                OnArenaClear?.Invoke();
            }
        }
    }
}
