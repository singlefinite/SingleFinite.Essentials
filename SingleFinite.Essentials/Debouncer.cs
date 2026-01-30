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
/// A debouncer service that uses a timer.
/// </summary>
public sealed class Debouncer : IDisposable, IDisposeObservable
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly DisposeState _disposeState;

    /// <summary>
    /// Used to synchronize access to the timer.
    /// </summary>
    private readonly Lock _timerLock = new();

    /// <summary>
    /// The timer used to wait for the debounce delay to pass.
    /// </summary>
    private Timer? _timer = null;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public Debouncer()
    {
        _disposeState = new(
            owner: this,
            onDispose: Cancel
        );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsDisposed => _disposeState.IsDisposed;

    #endregion

    #region Methods

    /// <summary>
    /// Debounce the given action.
    /// </summary>
    /// <param name="action">
    /// The action to invoke if a debounce method has not been called before the
    /// given delay has passed.
    /// </param>
    /// <param name="delay">
    /// The amount of time to wait before invoking the given action.
    /// </param>
    /// <param name="dispatcher">
    /// The dispatcher that will run the action after the delay has passed.
    /// If not set the debounce will be run under the synchronization context
    /// of the thread this method was called on.
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public void Debounce(
        Action action,
        TimeSpan delay,
        IDispatcher? dispatcher = default,
        CancellationToken cancellationToken = default
    )
    {
        _disposeState.ThrowIfDisposed();

        var resolvedDispatcher = dispatcher ?? new ContinuationDispatcher();

        lock (_timerLock)
        {
            _timer?.Dispose();
            _timer = new(
                callback: OnTimeout,
                state: () =>
                {
                    resolvedDispatcher.Run(
                        action: action,
                        cancellationToken: cancellationToken
                    );
                },
                dueTime: delay,
                period: Timeout.InfiniteTimeSpan
            );
        }
    }

    /// <summary>
    /// Debounce the given function.
    /// </summary>
    /// <param name="function">
    /// The Func to invoke if a debounce method has not been called before the
    /// given delay has passed.
    /// </param>
    /// <param name="delay">
    /// The amount of time to wait before invoking the given Func.
    /// </param>
    /// <param name="dispatcher">
    /// The dispatcher that will run the Func after the delay has passed.
    /// If not set the debounce will be run under the synchronization context
    /// of the thread this method was called on.
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public void Debounce(
        Func<Task> function,
        TimeSpan delay,
        IDispatcher? dispatcher = default,
        CancellationToken cancellationToken = default
    )
    {
        _disposeState.ThrowIfDisposed();

        var resolvedDispatcher = dispatcher ?? new ContinuationDispatcher();

        lock (_timerLock)
        {
            _timer?.Dispose();
            _timer = new(
                callback: OnTimeout,
                state: () =>
                {
                    resolvedDispatcher.Run(
                        function: function,
                        cancellationToken: cancellationToken
                    );
                },
                dueTime: delay,
                period: Timeout.InfiniteTimeSpan
            );
        }
    }

    /// <summary>
    /// Cancel a pending debounce if there is one.
    /// </summary>
    public void Cancel()
    {
        lock (_timerLock)
        {
            _timer?.Dispose();
            _timer = null;
        }
    }

    /// <summary>
    /// The method invoked when a debounce delay has passed.
    /// </summary>
    /// <param name="state">The debounce info to process.</param>
    private void OnTimeout(object? state)
    {
        if (state is not Action action)
            return;

        Cancel();
        action();
    }

    /// <summary>
    /// Cancel any pending debounce and dispose of this object.
    /// </summary>
    public void Dispose() => _disposeState.Dispose();

    #endregion

    #region Events

    /// <inheritdoc/>
    public Observable Disposed => _disposeState.Disposed;

    #endregion
}
