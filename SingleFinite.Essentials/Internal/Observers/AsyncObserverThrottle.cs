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

namespace SingleFinite.Essentials.Internal.Observers;

/// <summary>
/// Observer that throttles events.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
/// <param name="limit">The limit for throttling.</param>
internal class AsyncObserverThrottle(
    IAsyncObserver parent,
    TimeSpan limit
) : AsyncObserverBase(parent)
{
    #region Fields

    /// <summary>
    /// Used to throttle events.
    /// </summary>
    private readonly Throttler _throttler = new();

    #endregion

    #region Methods

    /// <summary>
    /// Throttle the event if needed.
    /// </summary>
    /// <returns>Returns true if the event wasn't throttled.</returns>
    protected override Task<bool> OnEventAsync() =>
        Task.FromResult(
            !_throttler.Throttle(
                action: () => { },
                limit: limit
            )
        );

    #endregion
}

/// <summary>
/// Observer that throttles events.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="limit">The limit for throttling.</param>
internal class AsyncObserverThrottle<TArgs>(
    IAsyncObserver<TArgs> parent,
    TimeSpan limit
) : AsyncObserverBase<TArgs>(parent)
{
    #region Fields

    /// <summary>
    /// Used to throttle events.
    /// </summary>
    private readonly Throttler _throttler = new();

    #endregion

    #region Methods

    /// <summary>
    /// Throttle the event if needed.
    /// </summary>
    /// <param name="args">Arguments to pass with the event.</param>
    /// <returns>Returns true if the event wasn't throttled.</returns>
    protected override Task<bool> OnEventAsync(TArgs args) =>
        Task.FromResult(
            !_throttler.Throttle(
                action: () => { },
                limit: limit
            )
        );

    #endregion
}
