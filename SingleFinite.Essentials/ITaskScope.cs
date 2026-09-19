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
/// Used to manage a hierarchy of asynchronous tasks that can be cancelled.
/// </summary>
public interface ITaskScope
{
    /// <summary>
    /// The default dispatcher for this scope.
    /// </summary>
    ITaskDispatcher Dispatcher { get; }

    /// <summary>
    /// Token that is cancelled when this scope is disposed.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Create a new TaskScope that is a child of this scope.  If this scope is
    /// cancelled any descendants of this scope will be cancelled as well.
    /// </summary>
    /// <param name="dispatcher">
    /// The dispatcher for the newly created child scope.  If none is specified
    /// the dispatcher of this scope will be used.
    /// </param>
    /// <returns>A new child scope.</returns>
    TaskScope CreateChildScope(ITaskDispatcher? dispatcher = default);

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
    /// <returns>A job that runs until the function has completed.</returns>
    ITaskJob<TResult> Run<TResult>(
        Func<Task<TResult>> function,
        ITaskDispatcher? dispatcher = default
    );
}
