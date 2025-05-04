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
/// Implementation of <see cref="IDispatcher"/> that invokes functions using the
/// same synchronization context from the thread that this class was created on.
/// </summary>
public sealed class ContinuationDispatcher :
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
    /// Holds the task scheduler used to invoke functions.
    /// </summary>
    private readonly TaskScheduler _taskScheduler;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="onError">
    /// Optional exception handler that is invoked when the OnError method is
    /// invoked.
    /// </param>
    public ContinuationDispatcher(Action<Exception>? onError = default)
    {
        _onError = onError;
        _disposeState = new(this);
        _taskScheduler = TaskScheduler.Current;
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
    /// Invoke the function on the thread that called this method.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result returned by the function.
    /// </typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task that runs until the function has completed.</returns>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this object has been disposed.
    /// </exception>
    public Task<TResult> RunAsync<TResult>(Func<Task<TResult>> func)
    {
        _disposeState.ThrowIfDisposed();

        var taskCompletionSource = new TaskCompletionSource<TResult>();

        Task.Factory.StartNew(
            function: async () =>
            {
                try
                {
                    var result = await func();
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            },
            cancellationToken: CancellationToken.None,
            creationOptions: TaskCreationOptions.None,
            scheduler: _taskScheduler
        );

        return taskCompletionSource.Task;
    }

    /// <inheritdoc/>
    public void OnError(Exception ex) => _onError?.Invoke(ex);

    /// <inheritdoc/>
    public void Dispose() => _disposeState.Dispose();

    #endregion

    #region Events

    /// <inheritdoc/>
    public Observable Disposed => _disposeState.Disposed;

    #endregion
}
