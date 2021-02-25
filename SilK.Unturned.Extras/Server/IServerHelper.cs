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
        /// Runs the given task when the server is loaded.
        /// If the server is already loaded, the task runs immediately.
        /// </summary>
        /// <param name="task">The given task to be ran.</param>
        void RunWhenServerLoaded(Func<Task> task);

        /// <summary>
        /// Runs the given task when the server is loaded.
        /// If the server is already loaded, the task runs immediately.
        /// </summary>
        /// <param name="task">The given task to be ran.</param>
        void RunWhenServerLoaded(Func<UniTask> task);
    }
}
