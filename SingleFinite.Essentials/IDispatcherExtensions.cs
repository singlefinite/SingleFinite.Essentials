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

namespace SingleFinite.Essentials;

/// <summary>
/// Functions that extend the <seealso cref="IDispatcher"/> interface.
/// </summary>
public static class IDispatcherExtensions
{
    extension(IDispatcher dispatcher)
    {
        /// <summary>
        /// Execute the given async function without result.
        /// </summary>
        /// <param name="function">The function to execute.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>
        /// A task that runs until the function has completed.
        /// </returns>
        public Task RunAsync(
            Func<Task> function,
            CancellationToken cancellationToken = default
        ) =>
            dispatcher.RunAsync(
                function: async () =>
                {
                    await function().ConfigureAwait(false);
                    return 0;
                },
                cancellationToken: cancellationToken
            );

        /// <summary>
        /// Execute the given function with result.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of result returned by the function.
        /// </typeparam>
        /// <param name="function">The function to execute.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>
        /// A task that runs until the function has completed.
        /// </returns>
        public Task<TResult> RunAsync<TResult>(
            Func<TResult> function,
            CancellationToken cancellationToken = default
        ) =>
            dispatcher.RunAsync(
                function: () => Task.FromResult(function()),
                cancellationToken: cancellationToken
            );

        /// <summary>
        /// Execute the given action.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A task that runs until the action has completed.</returns>
        public Task RunAsync(
            Action action,
            CancellationToken cancellationToken = default
        ) =>
            dispatcher.RunAsync(
                function: () =>
                {
                    action();
                    return Task.CompletedTask;
                },
                cancellationToken: cancellationToken
            );

        /// <summary>
        /// Execute the given action.
        /// This method will dispatch the action to be executed and return right 
        /// away without waiting for the action to complete execution.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public void Run(
            Action action,
            CancellationToken cancellationToken = default
        ) =>
            dispatcher.RunAsync(
                function: () =>
                {
                    action();
                    return Task.CompletedTask;
                },
                cancellationToken: cancellationToken
            ).ReportOnException(dispatcher);

        /// <summary>
        /// Execute the given async Func.
        /// This method will dispatch the Func to be executed and return right 
        /// away without waiting for the Func to complete execution.
        /// </summary>
        /// <param name="function">The Func to execute.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public void Run(
            Func<Task> function,
            CancellationToken cancellationToken = default
        ) =>
            dispatcher.RunAsync(
                function: async () =>
                {
                    await function().ConfigureAwait(false);
                    return Task.CompletedTask;
                },
                cancellationToken: cancellationToken
            ).ReportOnException(dispatcher);
    }
}
