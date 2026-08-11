// MIT License
// Copyright (c) 2026 Single Finite
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using SingleFinite.Essentials.Internal;

namespace SingleFinite.Essentials;

/// <summary>
/// Extensions for the TaskScope class.
/// </summary>
public static class ITaskScopeExtensions
{
    /// <summary>
    /// Extension members for <see cref="ITaskScope"/>.
    /// </summary>
    /// <param name="scope">The instance being extended.</param>
    extension(ITaskScope scope)
    {
        /// <summary>
        /// Execute the given action.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatcher">
        /// Optional dispatcher to use to execute the function.  If not
        /// specified the default dispatcher for this scope will be used.
        /// </param>
        /// <returns>A task that runs until the action has completed.</returns>
        public Task RunAsync(
            Action action,
            IDispatcher? dispatcher = default
        ) =>
            scope.RunAsync(
                function: _ =>
                {
                    action();
                    return Task.FromResult(0);
                },
                dispatcher: dispatcher
            );

        /// <summary>
        /// Execute the given action.
        /// This method will dispatch the action to be executed and return right 
        /// away without waiting for the action to complete execution.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatcher">
        /// Optional dispatcher to use to execute the function.  If not
        /// specified the default dispatcher for this scope will be used.
        /// </param>
        /// <returns>The task job.</returns>
        public ITaskJob Run(
            Action action,
            IDispatcher? dispatcher = default
        )
        {
            var job = new TaskJob<int>();
            job.Run(_ =>
            {
                action();
                return Task.FromResult(0);
            });
            job.Task.EmitOnException(dispatcher ?? scope.Dispatcher);
            return job;
        }

        /// <summary>
        /// Execute the given async Func.
        /// This method will dispatch the Func to be executed and return right 
        /// away without waiting for the Func to complete execution.
        /// </summary>
        /// <param name="function">The Func to execute.</param>
        /// <param name="dispatcher">
        /// Optional dispatcher to use to execute the function.  If not
        /// specified the default dispatcher for this scope will be used.
        /// </param>
        /// <returns>The task job.</returns>
        public ITaskJob Run(
            Func<Task> function,
            IDispatcher? dispatcher = default
        )
        {
            var job = new TaskJob<int>();
            job.Run(async _ =>
            {
                await function().ConfigureAwait(false);
                return 0;
            });
            job.Task.EmitOnException(dispatcher ?? scope.Dispatcher);
            return job;
        }

        /// <summary>
        /// Execute the given async Func.
        /// This method will dispatch the Func to be executed and return right 
        /// away without waiting for the Func to complete execution.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of result returned by the task.
        /// </typeparam>
        /// <param name="function">The Func to execute.</param>
        /// <param name="dispatcher">
        /// Optional dispatcher to use to execute the function.  If not
        /// specified the default dispatcher for this scope will be used.
        /// </param>
        /// <returns>The task job.</returns>
        public ITaskJob<TResult> Run<TResult>(
            Func<Task<TResult>> function,
            IDispatcher? dispatcher = default
        )
        {
            var job = new TaskJob<TResult>();
            job.Run(async _ => await function().ConfigureAwait(false));
            job.Task.EmitOnException(dispatcher ?? scope.Dispatcher);
            return job;
        }

        /// <summary>
        /// Execute the given cancellable async function without result.
        /// </summary>
        /// <param name="function">The Func to execute.</param>
        /// <param name="dispatcher">
        /// Optional dispatcher to use to execute the function.  If not
        /// specified the default dispatcher for this scope will be used.
        /// </param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>
        /// A task that runs until the function has completed.
        /// </returns>
        public Task RunAsync(
            Func<CancellationToken, Task> function,
            IDispatcher? dispatcher = default,
            CancellationToken cancellationToken = default
        ) =>
            scope.RunAsync(
                function: async cancellationToken =>
                {
                    await function(cancellationToken).ConfigureAwait(false);
                    return Task.CompletedTask;
                },
                dispatcher: dispatcher,
                cancellationToken: cancellationToken
            );

        /// <summary>
        /// Execute the given cancellable function with result.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of result returned by the function.
        /// </typeparam>
        /// <param name="function">The function to execute.</param>
        /// <param name="dispatcher">
        /// Optional dispatcher to use to execute the function.  If not
        /// specified the default dispatcher for this scope will be used.
        /// </param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>
        /// A task that runs until the function has completed.
        /// </returns>
        public Task<TResult> RunAsync<TResult>(
            Func<CancellationToken, TResult> function,
            IDispatcher? dispatcher = default,
            CancellationToken cancellationToken = default
        ) =>
            scope.RunAsync(
                function: cancellationToken => Task.FromResult(
                    function(cancellationToken)
                ),
                dispatcher: dispatcher,
                cancellationToken: cancellationToken
            );

        /// <summary>
        /// Execute the given cancellable action.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatcher">
        /// Optional dispatcher to use to execute the function.  If not
        /// specified the default dispatcher for this scope will be used.
        /// </param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A task that runs until the action has completed.</returns>
        public Task RunAsync(
            Action<CancellationToken> action,
            IDispatcher? dispatcher = default,
            CancellationToken cancellationToken = default
        ) =>
            scope.RunAsync(
                function: cancellationToken =>
                {
                    action(cancellationToken);
                    return Task.CompletedTask;
                },
                dispatcher: dispatcher,
                cancellationToken: cancellationToken
            );

        /// <summary>
        /// Execute the given cancellable action.
        /// This method will dispatch the action to be executed and return right 
        /// away without waiting for the action to complete execution.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="dispatcher">
        /// Optional dispatcher to use to execute the function.  If not
        /// specified the default dispatcher for this scope will be used.
        /// </param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The task job.</returns>
        public ITaskJob Run(
            Action<CancellationToken> action,
            IDispatcher? dispatcher = default,
            CancellationToken cancellationToken = default
        )
        {
            var job = new TaskJob<int>(cancellationToken);
            job.Run(
                function: cancellationTokenFromFunc =>
                {
                    action(cancellationTokenFromFunc);
                    return Task.FromResult(0);
                }
            );
            job.Task.EmitOnException(dispatcher ?? scope.Dispatcher);
            return job;
        }

        /// <summary>
        /// Execute the given cancellable async function.
        /// This method will dispatch the function to be executed and return
        /// right away without waiting for the function to complete execution.
        /// </summary>
        /// <param name="function">The async function to execute.</param>
        /// <param name="dispatcher">
        /// Optional dispatcher to use to execute the function.  If not
        /// specified the default dispatcher for this scope will be used.
        /// </param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The task job.</returns>
        public ITaskJob Run(
            Func<CancellationToken, Task> function,
            IDispatcher? dispatcher = default,
            CancellationToken cancellationToken = default
        )
        {
            var job = new TaskJob<int>(cancellationToken);
            job.Run(
                function: async cancellationTokenFromFunc =>
                {
                    await function(cancellationTokenFromFunc).ConfigureAwait(false);
                    return 0;
                }
            );
            job.Task.EmitOnException(dispatcher ?? scope.Dispatcher);
            return job;
        }

        /// <summary>
        /// Execute the given cancellable async function.
        /// This method will dispatch the function to be executed and return
        /// right away without waiting for the function to complete execution.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of result returned by the task.
        /// </typeparam>
        /// <param name="function">The async function to execute.</param>
        /// <param name="dispatcher">
        /// Optional dispatcher to use to execute the function.  If not
        /// specified the default dispatcher for this scope will be used.
        /// </param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The task job.</returns>
        public ITaskJob<TResult> Run<TResult>(
            Func<CancellationToken, Task<TResult>> function,
            IDispatcher? dispatcher = default,
            CancellationToken cancellationToken = default
        )
        {
            var job = new TaskJob<TResult>(cancellationToken);
            job.Run(
                function: async cancellationTokenFromFunc =>
                    await function(cancellationTokenFromFunc).ConfigureAwait(false)
            );
            job.Task.EmitOnException(dispatcher ?? scope.Dispatcher);
            return job;
        }
    }
}
