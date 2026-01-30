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
/// A throttling service.
/// </summary>
public sealed class Throttler
{
    #region Fields

    /// <summary>
    /// Holds the date and time of the last action that was invoked through the
    /// throttle method.
    /// </summary>
    private DateTimeOffset _lastAction = DateTimeOffset.UtcNow.AddSeconds(-1);

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
    /// <returns>
    /// true if the action was not invoked.
    /// false if the action was invoked.
    /// </returns>
    public bool Throttle(
        Action action,
        TimeSpan limit
    ) => Throttle(action, limit, out var _);

    /// <summary>
    /// Throttle the given action.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    /// <param name="limit">
    /// If the time since the last action invoked through this method is less
    /// than this timespan the action will not be invoked.
    /// </param>
    /// <returns>
    /// <param name="elapsed">
    /// This will be set amount of time that has elapsed since the last time
    /// throttle was called.
    /// </param>
    /// true if the action was not invoked.
    /// false if the action was invoked.
    /// </returns>
    public bool Throttle(
        Action action,
        TimeSpan limit,
        out TimeSpan elapsed
    )
    {
        var now = DateTimeOffset.UtcNow;
        elapsed = now - _lastAction;

        if (elapsed < limit)
            return true;

        _lastAction = now;
        action();

        return false;
    }

    #endregion
}
