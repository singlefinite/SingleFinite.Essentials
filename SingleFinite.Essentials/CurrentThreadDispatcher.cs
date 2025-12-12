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
/// Implementation of <see cref="IDispatcher"/> that invokes functions on the
/// same thread that calls the RunAsync method.
/// </summary>
public sealed class CurrentThreadDispatcher :
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

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="onError">
    /// Optional exception handler that is invoked when the OnError method is
    /// invoked.
    /// </param>
    public CurrentThreadDispatcher(Action<Exception>? onError = default)
    {
        _onError = onError;
        _disposeState = new(this);
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
        return function();
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
