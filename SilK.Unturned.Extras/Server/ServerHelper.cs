using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.Server
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    internal class ServerHelper : IServerHelper, IDisposable
    {
        private readonly List<Func<Task>> _tasks;

        public ServerHelper()
        {
            Level.onPostLevelLoaded += OnPostLevelLoaded;

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
                foreach (var task in _tasks)
                {
                    await task.Invoke();
                }
            });
        }

        public void RunWhenServerLoaded(Func<Task> task) => _tasks.Add(task);

        public void RunWhenServerLoaded(Func<UniTask> task) => _tasks.Add(() => task.Invoke().AsTask());
    }
}
