using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Action = System.Action;

namespace SilK.Unturned.Extras.Server
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    internal class ServerHelper : IServerHelper, IDisposable
    {
        private readonly List<Action> _syncActions;
        private readonly List<Func<Task>> _tasks;

        public ServerHelper()
        {
            Level.onPostLevelLoaded += OnPostLevelLoaded;

            _syncActions = new List<Action>();
            _tasks = new List<Func<Task>>();
        }

        public void Dispose()
        {
            // ReSharper disable once DelegateSubtraction
            Level.onPostLevelLoaded -= OnPostLevelLoaded;
        }

        private void OnPostLevelLoaded(int level)
        {
            AsyncHelper.RunSync(async () =>
            {
                foreach (var action in _syncActions)
                {
                    action.Invoke();
                }

                foreach (var task in _tasks)
                {
                    await task.Invoke();
                }
            });
        }

        public void RunWhenServerLoaded(Action action)
        {
            if (Level.isLoaded)
            {
                action.Invoke();
            }
            else
            {
                _syncActions.Add(action);
            }
        }

        public async Task RunWhenServerLoaded(Func<Task> task)
        {
            await UniTask.SwitchToMainThread();

            if (Level.isLoaded)
            {
                await task.Invoke();
            }
            else
            {
                _tasks.Add(task);
            }
        }

        public UniTask RunWhenServerLoaded(Func<UniTask> task) =>
            RunWhenServerLoaded(() => task.Invoke().AsTask()).AsUniTask();
    }
}
