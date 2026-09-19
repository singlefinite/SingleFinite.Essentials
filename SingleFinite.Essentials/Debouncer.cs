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
public sealed class Debouncer : IDisposable
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
    /// The timer used to wait for the debounce delay to elapse.
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

    #region Methods

    /// <summary>
    /// Debounce the given action.
    /// </summary>
    /// <param name="action">
    /// The action to invoke if a debounce method has not been called before the
    /// given delay has elapsed.
    /// </param>
    /// <param name="delay">
    /// The amount of time to wait before invoking the given action.
    /// </param>
    /// <param name="scope">
    /// The scope that will run the action after the delay has elapsed.
    /// </param>
    /// <param name="dispatcher">
    /// The dispatcher that will run the action after the delay has elapsed.
    /// </param>
    public void Debounce(
        Action action,
        TimeSpan delay,
        ITaskScope? scope = default,
        ITaskDispatcher? dispatcher = default
    )
    {
        _disposeState.ThrowIfDisposed();

        var resolvedScope = scope ?? new TaskScope();

        lock (_timerLock)
        {
            _timer?.Dispose();
            _timer = new(
                callback: OnTimeout,
                state: () =>
                {
                    resolvedScope.Run(
                        action: action,
                        dispatcher: dispatcher
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
    /// The method invoked when a debounce delay has elapsed.
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
}
