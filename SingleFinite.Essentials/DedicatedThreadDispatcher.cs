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

using System.Collections.Concurrent;

namespace SingleFinite.Essentials;

/// <summary>
/// Implementation of <see cref="IDispatcher"/> that queues functions on to a
/// dedicated thread.  Normally a platform specific dispatcher should be used to
/// dispatch to the UI thread for that platform.  However, this dispatcher is 
/// useful for unit testing when there is no UI thread provided by the unit 
/// testing framework.
/// </summary>
public sealed class DedicatedThreadDispatcher :
    IDispatcher,
    IDisposable,
    IDisposeObservable
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly DisposeState _disposeState;

    /// <summary>
    /// Holds the exception handler for the dispatcher.
    /// </summary>
    private readonly Action<Exception>? _onError;

    /// <summary>
    /// The single thread that dispatched functions will be run on.
    /// </summary>
    private readonly Thread _thread;

    /// <summary>
    /// The blocking queue used to dispatch functions through.
    /// </summary>
    private readonly BlockingCollection<Action> _queue = [];

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="thread">
    /// The thread the dispatcher will run actions and functions on.  If not set
    /// a new Thread will be created and used.
    /// </param>
    /// <param name="onError">
    /// Optional exception handler that is invoked when the OnError method is
    /// invoked.
    /// </param>
    public DedicatedThreadDispatcher(
        Thread? thread = default,
        Action<Exception>? onError = default
    )
    {
        _onError = onError;

        _disposeState = new(
            owner: this,
            onDispose: OnDispose
        );

        _thread = thread ?? new(ThreadStart)
        {
            Name = GetType().FullName
        };
        _thread.Start();
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsDisposed => _disposeState.IsDisposed;

    /// <inheritdoc/>
    public CancellationToken CancellationToken =>
        _disposeState.CancellationToken;

    #endregion

    #region Methods

    /// <summary>
    /// This method is run on the dedicated thread and will execute functions
    /// and actions from the queue until the queue has been completed.
    /// </summary>
    private void ThreadStart()
    {
        var test = SynchronizationContext.Current;
        try
        {
            while (!_queue.IsCompleted)
            {
                var action = _queue.Take();
                action();
            }
        }
        catch (InvalidOperationException)
        {
            // Ignore exception thrown when action queue has been completed.
        }
    }

    /// <summary>
    /// Implements <see cref="IDispatcher"/> by dispatching the function 
    /// execution to the dedicated thread.  If this method is called from the 
    /// dedicated thread the function will be executed right away instead of 
    /// being queued.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result returned by the function.
    /// </typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that runs until the function has completed.</returns>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this object has been disposed.
    /// </exception>
    public Task<TResult> RunAsync<TResult>(
        Func<Task<TResult>> function,
        CancellationToken cancellationToken = default
    )
    {
        _disposeState.ThrowIfDisposed();

        if (Thread.CurrentThread == _thread)
            return function();

        var taskCompletionSource = new TaskCompletionSource<TResult>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        _queue.Add(
            item: async () =>
            {
                try
                {
                    var result = await function().ConfigureAwait(false);
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            },
            cancellationToken: cancellationToken
        );

        return taskCompletionSource.Task;
    }

    /// <inheritdoc/>
    public void OnError(Exception ex) => _onError?.Invoke(ex);

    /// <inheritdoc/>
    public void Dispose() => _disposeState.Dispose();

    /// <summary>
    /// Mark the queue as complete when disposed and wait for the dedicated 
    /// thread to stop.
    /// </summary>
    private void OnDispose()
    {
        _queue.CompleteAdding();
        _thread.Join();
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public Observable Disposed => _disposeState.Disposed;

    #endregion
}

