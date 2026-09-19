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
            ITaskDispatcher? dispatcher = default
        ) =>
            scope.Run(
                function: () =>
                {
                    action();
                    return Task.FromResult(0);
                },
                dispatcher: dispatcher
            );

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
            ITaskDispatcher? dispatcher = default
        ) =>
            scope.Run(
                function: async () =>
                {
                    await function();
                    return 0;
                },
                dispatcher: dispatcher
            );
    }
}
