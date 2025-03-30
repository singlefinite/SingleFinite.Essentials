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

using System.Diagnostics;

namespace SingleFinite.Essentials;

/// <summary>
/// This class is similiar to the <see cref="Throttler"/> class but it differs
/// in that it will invoke the last throttled action after the limit time
/// elapses and a new call has not been made in that time.  This can be thought
/// off as a throttler that has a buffer size of 1.
/// </summary>
public class ThrottleBuffer : IDisposeObservable
{
    #region Fields

    /// <summary>
    /// Used for throttling.
    /// </summary>
    private readonly Throttler _throttler = new();

    /// <summary>
    /// Use for debouncing.
    /// </summary>
    private readonly Debouncer _debouncer = new();

    #endregion

    #region Properties

    /// <inheritdoc/>
    public DisposeState DisposeState => _debouncer.DisposeState;

    #endregion

    #region Methods

    /// <summary>
    /// Throttle the given action.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    /// <param name="limit">
    /// If the time since the last action invoked through this method is less
    /// than this timespan the action will not be invoked.
    /// </param>
    /// <param name="dispatcher">
    /// The dispatcher to use to potentially invoke the action in the future if
    /// it was throttled.
    /// </param>
    /// <param name="onError">
    /// Optional handler for any exceptions that are thrown by the action when
    /// it is invoked through the dispatcher.
    /// </param>
    /// <returns>
    /// true if the action was not invoked.
    /// false if the action was invoked.
    /// </returns>
    public bool Throttle(
        Action action,
        TimeSpan limit,
        IDispatcher dispatcher,
        Action<Exception>? onError = default
    )
    {
        _debouncer.Cancel();

        if (_throttler.Throttle(action, limit))
        {
            _debouncer.Debounce(
                action: () => Throttle(action, limit, dispatcher, onError),
                delay: limit,
                dispatcher: dispatcher,
                onError: onError
            );

            return true;
        }

        return false;
    }

    #endregion
}
