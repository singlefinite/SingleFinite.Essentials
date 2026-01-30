// MIT License
// Copyright (c) 2025 Single Finite
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
/// Defines a scope for managing and executing asynchronous tasks with support
/// for hierarchical cancellation and dispatcher selection.
/// </summary>
/// <remarks>
/// A task scope provides a context for running asynchronous operations,
/// allowing for structured concurrency and coordinated cancellation of related
/// tasks. Child scopes inherit cancellation from their parent, enabling
/// cascading cancellation of task groups. Use this interface to organize and
/// control the lifetime of related asynchronous work, especially in
/// applications that require fine-grained task management or custom dispatching
/// strategies.
/// </remarks>
public interface ITaskScope : ICancelObservable, IDisposeObservable
{
    /// <summary>
    /// The default dispatcher for this scope.
    /// </summary>
    IDispatcher Dispatcher { get; }

    /// <summary>
    /// Create a new TaskScope that is a child of this scope.  If this scope is
    /// cancelled any descendants of this scope will be cancelled as well.
    /// </summary>
    /// <param name="dispatcher">
    /// The dispatcher for the newly created child scope.  If none is specified
    /// the dispatcher of this scope will be used.
    /// </param>
    /// <returns>A new child scope.</returns>
    TaskScope CreateChildScope(IDispatcher? dispatcher = default);

    /// <summary>
    /// Execute the given async function.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result returned by the function.
    /// </typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="dispatcher">
    /// Optional dispatcher to use to execute the function.  If not specified
    /// the default dispatcher for this scope will be used.
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that runs until the function has completed.</returns>
    Task<TResult> RunAsync<TResult>(
        Func<Task<TResult>> function,
        IDispatcher? dispatcher = default,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Execute the given cancellable async function.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result returned by the function.
    /// </typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="dispatcher">
    /// Optional dispatcher to use to execute the function.  If not specified
    /// the default dispatcher for this scope will be used.
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that runs until the function has completed.</returns>
    Task<TResult> RunAsync<TResult>(
        Func<CancellationToken, Task<TResult>> function,
        IDispatcher? dispatcher = default,
        CancellationToken cancellationToken = default
    );
}
