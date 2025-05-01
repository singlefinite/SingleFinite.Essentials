﻿// MIT License
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
/// <param name="dispatcher">
/// The dispatcher to use to potentially invoke the action in the future if
/// it was throttled.  If not set the action will be run on the calling
/// thread.
/// </param>
/// <param name="onError">
/// Optional handler for any exceptions that are thrown by the action when
/// it is invoked through the dispatcher.
/// </param>
internal class AsyncObserverThrottleLatest(
    IAsyncObserver parent,
    TimeSpan limit,
    IDispatcher? dispatcher,
    Action<Exception>? onError
) : AsyncObserverBase(parent)
{
    #region Fields

    /// <summary>
    /// Used to throttle events.
    /// </summary>
    private readonly ThrottlerLatest _throttleLatest = new();

    #endregion

    #region Methods

    /// <summary>
    /// Throttle the event if needed.
    /// </summary>
    /// <returns>Returns true if the event wasn't throttled.</returns>
    protected override Task<bool> OnEventAsync()
    {
        var isThrottled = false;
        isThrottled = _throttleLatest.Throttle(
            action: async () =>
            {
                if (isThrottled)
                    await RaiseNextEventAsync();
            },
            limit: limit,
            dispatcher: dispatcher,
            onError: onError
        );

        return Task.FromResult(!isThrottled);
    }

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
/// <param name="dispatcher">
/// The dispatcher to use to potentially invoke the action in the future if
/// it was throttled.  If not set the action will be run on the calling
/// thread.
/// </param>
/// <param name="onError">
/// Optional handler for any exceptions that are thrown by the action when
/// it is invoked through the dispatcher.
/// </param>
internal class AsyncObserverThrottleLatest<TArgs>(
    IAsyncObserver<TArgs> parent,
    TimeSpan limit,
    IDispatcher? dispatcher,
    Action<Exception>? onError
) : AsyncObserverBase<TArgs>(parent)
{
    #region Fields

    /// <summary>
    /// Used to throttle events.
    /// </summary>
    private readonly ThrottlerLatest _throttleLatest = new();

    #endregion

    #region Methods

    /// <summary>
    /// Throttle the event if needed.
    /// </summary>
    /// <param name="args">Arguments to pass with the event.</param>
    /// <returns>Returns true if the event wasn't throttled.</returns>
    protected override Task<bool> OnEventAsync(TArgs args)
    {
        var isThrottled = false;
        isThrottled = _throttleLatest.Throttle(
            action: async () =>
            {
                if (isThrottled)
                    await RaiseNextEventAsync(args);
            },
            limit: limit,
            dispatcher: dispatcher,
            onError: onError
        );

        return Task.FromResult(!isThrottled);
    }

    #endregion
}
