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
/// A debouncer service that uses a timer.
/// </summary>
public class Debouncer : IDisposeObservable
{
    #region Fields

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
        DisposeState = new(
            owner: this,
            onDispose: OnDispose
        );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public DisposeState DisposeState { get; }

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
    /// </param>
    /// <param name="onError">
    /// Optional handler for any exceptions that are thrown by the action when
    /// it is invoked.
    /// </param>
    public void Debounce(
        Action action,
        TimeSpan delay,
        IDispatcher dispatcher,
        Action<Exception>? onError = default
    )
    {
        DisposeState.ThrowIfDisposed();

        lock (_timerLock)
        {
            _timer?.Dispose();
            _timer = new(
                callback: OnTimeout,
                state: () =>
                {
                    dispatcher.Run(
                        action: action,
                        onError: onError
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
    /// <param name="func">
    /// The func to invoke if a debounce method has not been called before the
    /// given delay has passed.
    /// </param>
    /// <param name="delay">
    /// The amount of time to wait before invoking the given func.
    /// </param>
    /// <param name="dispatcher">
    /// The dispatcher that will run the func after the delay has passed.
    /// </param>
    /// <param name="onError">
    /// Optional handler for any exceptions that are thrown by the func when it
    /// is invoked.
    /// </param>
    public void Debounce(
        Func<Task> func,
        TimeSpan delay,
        IDispatcher dispatcher,
        Action<Exception>? onError = default
    )
    {
        DisposeState.ThrowIfDisposed();

        lock (_timerLock)
        {
            _timer?.Dispose();
            _timer = new(
                callback: OnTimeout,
                state: () =>
                {
                    dispatcher.Run(
                        func: func,
                        onError: onError
                    );
                },
                dueTime: delay,
                period: Timeout.InfiniteTimeSpan
            );
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

        lock (_timerLock)
        {
            _timer?.Dispose();
            _timer = null;
        }

        action();
    }

    /// <summary>
    /// Cancel any pending debounce and dispose of this object.
    /// </summary>
    private void OnDispose()
    {
        lock (_timerLock)
        {
            _timer?.Dispose();
            _timer = null;
        }
    }

    #endregion
}
