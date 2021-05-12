using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.Dispatcher
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    internal class ActionDispatcher : IActionDispatcher, IDisposable
    {
        private readonly ILogger<ActionDispatcher> _logger;
        private readonly ConcurrentQueue<Action> _queueActions = new();
        private readonly AutoResetEvent _waitHandle = new(false);

        private bool _disposed;
        private Thread? _loopThread;

        public ActionDispatcher(ILogger<ActionDispatcher> logger)
        {
            _logger = logger;
        }

        public void Dispose()
        {
            lock (this)
            {
                if (_disposed)
                    return;

                _disposed = true;
            }

            _waitHandle.Set();

            _loopThread?.Join();
            _loopThread = null;

            _waitHandle.Dispose();
        }

        private bool LoadDispatcher()
        {
            lock (this)
            {
                if (_disposed)
                    return false;

                if (_loopThread != null)
                    return true;

                _loopThread = new Thread(Looper);
            }

            _loopThread.Start();
            return true;
        }

        private void Looper()
        {
            while (true)
            {
                lock (this)
                {
                    if (_disposed)
                        return;
                }

                _waitHandle.WaitOne();
                ProcessQueue();
            }
        }

        private void ProcessQueue()
        {
            while (_queueActions.TryDequeue(out var action))
                //Try catch prevents exception in case of direct insert on ConcurrentQueue instead of Enqueue it
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception while dispatching a task");
                }
        }

        #region Enqueue

        public Task Enqueue(Action action, Action<Exception>? exceptionHandler = null)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            if (!LoadDispatcher())
                throw new ObjectDisposedException(nameof(ActionDispatcher));

            var tcs = new TaskCompletionSource<Task>();
            _queueActions.Enqueue(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(Task.CompletedTask);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);

                    if (exceptionHandler != null)
                        exceptionHandler(ex);
                    else
                        _logger.LogError(ex, "Exception while dispatching a task");
                }
            });
            _waitHandle.Set();
            return tcs.Task;
        }

        public Task Enqueue(Func<Task> task, Action<Exception>? exceptionHandler = null)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (!LoadDispatcher())
                throw new ObjectDisposedException(nameof(ActionDispatcher));

            var tcs = new TaskCompletionSource<Task>();
            _queueActions.Enqueue(() =>
            {
                try
                {
                    tcs.SetResult(task());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);

                    if (exceptionHandler != null)
                        exceptionHandler(ex);
                    else
                        _logger.LogError(ex, "Exception while dispatching a task");
                }
            });
            _waitHandle.Set();
            return tcs.Task;
        }

        public Task<T> Enqueue<T>(Func<T> action, Action<Exception>? exceptionHandler = null)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            if (!LoadDispatcher())
                throw new ObjectDisposedException(nameof(ActionDispatcher));

            var tcs = new TaskCompletionSource<T>();
            _queueActions.Enqueue(() =>
            {
                try
                {
                    tcs.SetResult(action());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);

                    if (exceptionHandler != null)
                        exceptionHandler(ex);
                    else
                        _logger.LogError(ex, "Exception while dispatching a task");
                }
            });
            _waitHandle.Set();
            return tcs.Task;
        }

        public Task<T> Enqueue<T>(Func<Task<T>> task, Action<Exception>? exceptionHandler = null)
        {
            if (task is null)
                throw new ArgumentNullException(nameof(task));

            if (!LoadDispatcher())
                throw new ObjectDisposedException(nameof(ActionDispatcher));

            var tcs = new TaskCompletionSource<T>();
            _queueActions.Enqueue(async () =>
            {
                try
                {
                    tcs.SetResult(await task());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);

                    if (exceptionHandler is not null)
                        exceptionHandler(ex);
                    else
                        _logger.LogError(ex, "Exception while dispatching a task");
                }
            });
            _waitHandle.Set();
            return tcs.Task;
        }

        #endregion
    }
}
