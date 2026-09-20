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
/// Base class for task dispatchers.
/// </summary>
public abstract class TaskDispatcher : ITaskDispatcher
{
    #region Methods

    /// <summary>
    /// Set the task scope context for the current async local.
    /// </summary>
    /// <param name="scope">The scope to set.</param>
    /// <param name="dispatcher">The dispatcher to set.</param>
    /// <param name="cancellationToken">The cancellation token to set.</param>
    protected static void SetTaskContext(
        ITaskScope scope,
        ITaskDispatcher dispatcher,
        CancellationToken cancellationToken
    )
    {
        TaskContext.ScopeLocal.Value = scope;
        TaskContext.DispatcherLocal.Value = dispatcher;
        TaskContext.CancellationTokenLocal.Value = cancellationToken;
    }

    /// <inheritdoc/>
    public abstract Task<TResult> RunAsync<TResult>(
        Func<Task<TResult>> function,
        ITaskScope scope,
        CancellationToken cancellation
    );

    #endregion

    #region Events

    /// <summary>
    /// Observable that emits when an unhandled exception occurs in a
    /// dispatcher.  This observable will emit on thread pool threads.
    /// </summary>
    public static IEventObservable<UnhandledDispatcherException>
        UnhandledException => UnhandledExceptionSource.Observable;
    internal static readonly EventObservableSource<UnhandledDispatcherException>
        UnhandledExceptionSource = new();

    #endregion
}
