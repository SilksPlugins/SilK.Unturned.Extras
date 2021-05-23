using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.Dispatcher
{
    /// <summary>
    /// Executes given actions/tasks in the order they are queued.
    /// </summary>
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Highest)]
    internal class ActionDispatcher : IActionDispatcher, IAsyncDisposable
    {
        private readonly ILogger<ActionDispatcher> _logger;
        private readonly ConcurrentQueue<Func<Task>> _queuedTasks = new();

        private bool _disposed;
        private AsyncAutoResetEvent? _disposeWaitHandle;

        private readonly AsyncLock _mutex = new();
        private bool _processingQueue;

        public ActionDispatcher(ILogger<ActionDispatcher> logger)
        {
            _logger = logger;
        }

        public async ValueTask DisposeAsync()
        {
            using (await _mutex.LockAsync())
            {
                if (_disposed)
                    return;

                _disposed = true;

                if (_queuedTasks.IsEmpty) return;

                _disposeWaitHandle = new AsyncAutoResetEvent();
            }

            await _disposeWaitHandle.WaitAsync();
        }

        private async Task ProcessQueue()
        {
            while (true)
            {
                while (_queuedTasks.TryDequeue(out var task))
                {
                    try
                    {
                        await task();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception while dispatching a task");
                    }
                }

                // New tasks are only queued when the mutex is locked.
                // By locking the mutex and checking if it is empty,
                // even if a new task is being queued, a new process queue
                // will begin after the new task is queued.
                // TLDR: No tasks will end up being unprocessed
                using (await _mutex.LockAsync())
                {
                    if (!_queuedTasks.IsEmpty) continue;

                    _processingQueue = false;
                    _disposeWaitHandle?.Set();

                    break;
                }
            }
        }

        private async Task EnsureQueueProcessing(Func<Task> task)
        {
            using (await _mutex.LockAsync())
            {
                _queuedTasks.Enqueue(task);

                if (_processingQueue) return;

                _processingQueue = true;

                Task.Run(ProcessQueue);
            }
        }

        public Task Enqueue(Action action, Action<Exception>? exceptionHandler = null)
        {
            return Enqueue(() =>
            {
                action();

                return Task.CompletedTask;
            }, exceptionHandler);
        }

        public Task Enqueue(Func<Task> task, Action<Exception>? exceptionHandler = null)
        {
            return Enqueue(async () =>
            {
                await task();

                return false;
            }, exceptionHandler);
        }

        public Task<T> Enqueue<T>(Func<T> action, Action<Exception>? exceptionHandler = null)
        {
            return Enqueue(() =>
            {
                var result = action();

                return Task.FromResult(result);
            }, exceptionHandler);
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> task, Action<Exception>? exceptionHandler = null)
        {
            var tcs = new TaskCompletionSource<T>();

            await EnsureQueueProcessing(async () =>
            {
                try
                {
                    var result = await task();

                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);

                    if (exceptionHandler == null)
                    {
                        _logger.LogError(ex, "Exception while dispatching a task");
                    }
                    else
                    {
                        exceptionHandler(ex);
                    }
                }
            });

            return await tcs.Task;
        }
    }
}
