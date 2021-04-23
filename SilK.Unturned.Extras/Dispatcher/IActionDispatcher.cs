using OpenMod.API.Ioc;
using System;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.Dispatcher
{
    /// <summary>
    /// Executes given actions/tasks in the order they are queued.
    /// Credits to Rube200 on the OpenMod.Economy project,
    /// where this code is from: https://github.com/Rube200
    /// </summary>
    [Service]
    public interface IActionDispatcher
    {
        /// <summary>
        /// Enqueue the given action and waits until this action is finished executing.
        /// </summary>
        /// <param name="action">The action to enqueue.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        public Task Enqueue(Action action, Action<Exception>? exceptionHandler = null);

        /// <summary>
        /// Enqueue the given task and waits until this task is finished executing.
        /// </summary>
        /// <param name="task">The task to enqueue.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        public Task Enqueue(Func<Task> task, Action<Exception>? exceptionHandler = null);

        /// <summary>
        /// Enqueue the given action and waits until this action is finished executing.
        /// </summary>
        /// <param name="action">The action to enqueue.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>The result from the given action.</returns>
        public Task<T> Enqueue<T>(Func<T> action, Action<Exception>? exceptionHandler = null);

        /// <summary>
        /// Enqueue the given task and waits until this task is finished executing.
        /// </summary>
        /// <param name="task">The task to enqueue.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>The result from the given task.</returns>
        public Task<T> Enqueue<T>(Func<Task<T>> task, Action<Exception>? exceptionHandler = null);
    }
}