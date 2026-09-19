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

namespace SingleFinite.Essentials.Internal.EventObservers;

/// <summary>
/// EventObserver that throttles events.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
/// <param name="limit">The limit for throttling.</param>
/// <param name="scope">
/// The scope to invoke the next observers with.
/// </param>
/// <param name="dispatcher">
/// The dispatcher to invoke the next observers with.
/// </param>
internal class AsyncEventObserverThrottleLatest(
    IAsyncEventObserver parent,
    TimeSpan limit,
    ITaskScopeContext? scope,
    ITaskDispatcher? dispatcher
) : AsyncEventObserverBase(parent)
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
            scope: scope,
            dispatcher: dispatcher
        );

        return Task.FromResult(!isThrottled);
    }

    #endregion
}

/// <summary>
/// EventObserver that throttles events.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="limit">The limit for throttling.</param>
/// <param name="scope">
/// The scope to invoke the next observers with.
/// </param>
/// <param name="dispatcher">
/// The dispatcher to invoke the next observers with.
/// </param>
internal class AsyncEventObserverThrottleLatest<TArgs>(
    IAsyncEventObserver<TArgs> parent,
    TimeSpan limit,
    ITaskScopeContext? scope,
    ITaskDispatcher? dispatcher
) : AsyncEventObserverBase<TArgs>(parent)
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
            scope: scope,
            dispatcher: dispatcher
        );

        return Task.FromResult(!isThrottled);
    }

    #endregion
}
