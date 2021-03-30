using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using System;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.Server
{
    [Service]
    public interface IServerHelper
    {
        /// <summary>
        /// Runs the given method when the server is loaded.
        /// If the server is already loaded, the method runs immediately.
        /// </summary>
        /// <param name="action">The given method to be ran.</param>
        void RunWhenLoaded(Action action);

        /// <summary>
        /// Runs the given task when the server is loaded.
        /// If the server is already loaded, the task runs immediately.
        /// The given task will always run on the main thread.
        /// </summary>
        /// <param name="task">The given task to be ran.</param>
        Task RunWhenLoaded(Func<Task> task);

        /// <summary>
        /// Runs the given task when the server is loaded.
        /// If the server is already loaded, the task runs immediately.
        /// The given task will always run on the main thread.
        /// </summary>
        /// <param name="task">The given task to be ran.</param>
        UniTask RunWhenLoaded(Func<UniTask> task);
    }
}
