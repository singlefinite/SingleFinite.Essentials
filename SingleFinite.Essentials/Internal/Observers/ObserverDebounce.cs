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
/// Observer that debounces events.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
/// <param name="delay">The delay period for debouncing.</param>
/// <param name="dispatcher">
/// The dispatcher to run on after the delay has passed.
/// If not set the debounce will be run under the synchronization context
/// of the thread this method was called on.
/// </param>
internal class ObserverDebounce(
    IObserver parent,
    TimeSpan delay,
    IDispatcher? dispatcher
) : ObserverBase(parent), IObserver
{
    #region Fields

    /// <summary>
    /// Debouncer used to debounce.
    /// </summary>
    private readonly Debouncer _debouncer = new();

    #endregion

    #region Methods

    /// <summary>
    /// Wait for the configured delay and if no new events have been raised
    /// when the delay period has passed pass the event onto the next observer.
    /// </summary>
    /// <returns>
    /// This method always returns false since the next event is only raised
    /// after the delay has passed.
    /// </returns>
    protected override bool OnEvent()
    {
        _debouncer.Debounce(
            action: () => BranchNext?.Invoke(),
            delay: delay,
            dispatcher: dispatcher
        );

        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised when an event has been debounced.
    /// </summary>
    event Action IObserver.Next
    {
        add => BranchNext += value;
        remove => BranchNext -= value;
    }
    private event Action? BranchNext;

    #endregion
}

/// <summary>
/// Observer that debounces events.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="delay">The delay period for debouncing.</param>
/// <param name="dispatcher">
/// The dispatcher to run on after the delay has passed.
/// If not set the debounce will be run under the synchronization context
/// of the thread this method was called on.
/// </param>
internal class ObserverDebounce<TArgs>(
    IObserver<TArgs> parent,
    TimeSpan delay,
    IDispatcher? dispatcher
) : ObserverBase<TArgs>(parent), IObserver<TArgs>
{
    #region Fields

    /// <summary>
    /// Debouncer used to debounce.
    /// </summary>
    private readonly Debouncer _debouncer = new();

    #endregion

    #region Methods

    /// <summary>
    /// Wait for the configured delay and if no new events have been raised
    /// when the delay period has passed pass the event onto the next observer.
    /// </summary>
    /// <param name="args">Arguments passed with the observed event.</param>
    /// <returns>
    /// This method always returns false since the next event is only raised
    /// after the delay has passed.
    /// </returns>
    protected override bool OnEvent(TArgs args)
    {
        _debouncer.Debounce(
            action: () => BranchNext?.Invoke(args),
            delay: delay,
            dispatcher: dispatcher
        );

        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised when an event has been debounced.
    /// </summary>
    event Action<TArgs> IObserver<TArgs>.Next
    {
        add => BranchNext += value;
        remove => BranchNext -= value;
    }
    private event Action<TArgs>? BranchNext;

    #endregion
}
