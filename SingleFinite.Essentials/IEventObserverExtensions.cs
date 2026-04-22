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

using SingleFinite.Essentials.Internal.EventObservers;

namespace SingleFinite.Essentials;

/// <summary>
/// Methods that modify how an observable event is handled.  EventObservers are
/// chained together to create different handler logic.
/// </summary>
public static class IEventObserverExtensions
{
    /// <summary>
    /// Extension members for <seealso cref="IEventObserver"/>.
    /// </summary>
    /// <param name="observer">The instance being extended.</param>
    extension(IEventObserver observer)
    {
        /// <summary>
        /// Invoke the given callback whenever the observable event is raised.
        /// </summary>
        /// <param name="callback">
        /// The callback to invoke when the observable event is raised.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver OnEach(Action callback) =>
            new EventObserverOnEach(
                parent: observer,
                callback: callback
            );

        /// <summary>
        /// Select a value to pass to chained observers.
        /// </summary>
        /// <typeparam name="TArgsOut">
        /// The type of arguments that will be passed to chained observers.
        /// </typeparam>
        /// <param name="selector">
        /// The callback to invoke to select the arguments to pass to chained
        /// observers.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver<TArgsOut> Select<TArgsOut>(Func<TArgsOut> selector) =>
            new EventObserverSelect<TArgsOut>(
                parent: observer,
                selector: selector
            );

        /// <summary>
        /// Filter out observable events that don't match the predicate and
        /// prevent them from being passed down the observer chain.
        /// </summary>
        /// <param name="predicate">
        /// The callback invoked to determine if the observable event should be
        /// filtered out.  If the callback returns false the observable event
        /// will be filtered out.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver Where(Func<bool> predicate) =>
            new EventObserverWhere(
                parent: observer,
                predicate: predicate
            );

        /// <summary>
        /// Dispose of the observer chain if the predicate is matched.
        /// </summary>
        /// <param name="predicate">
        /// The callback invoked to determine if the observer chain should be
        /// disposed.  If the callback returns true the observer chain is
        /// disposed.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver Until(
            Func<bool> predicate
        ) =>
            new EventObserverUntil(
                parent: observer,
                predicate: predicate
            );

        /// <summary>
        /// Dispose of the observer chain when the given cancellation token is
        /// cancelled.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token that when cancelled will dispose of this
        /// observer.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver Until(CancellationToken cancellationToken) =>
            new EventObserverUntil(
                parent: observer,
                cancellationToken: cancellationToken
            );

        /// <summary>
        /// Dispose of the observer chain when the given observer emits.
        /// </summary>
        /// <param name="disposeObserver">
        /// The observer that when emits will dispose of this observer.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver Until(IEventObserver disposeObserver) =>
            new EventObserverUntil(
                parent: observer,
                disposeObserver: disposeObserver.ToObservable().Observe()
            );

        /// <summary>
        /// Dispose of the observer chain when the given observable emits.
        /// </summary>
        /// <param name="disposeObservable">
        /// The observable that when emits will dispose of this observer.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver Until(IEventObservable disposeObservable) =>
            new EventObserverUntil(
                parent: observer,
                disposeObserver: disposeObservable.Observe()
            );

        /// <summary>
        /// Dispose of the observer chain when the given observable emits.
        /// </summary>
        /// <param name="disposeObservable">
        /// The observable that when emits will dispose of this observer.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver Until<TArgs>(IEventObservable<TArgs> disposeObservable) =>
            new EventObserverUntil(
                parent: observer,
                disposeObserver: disposeObservable.Observe()
            );

        /// <summary>
        /// Dispose of the observer chain when the first event is observed.
        /// </summary>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver Once() =>
            Until(
                observer: observer,
                predicate: () => true
            );

        /// <summary>
        /// Invoke the next observers using the provided dispatcher.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher to invoke the next observers with.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver Dispatch(
            IDispatcher dispatcher
        ) =>
            new EventObserverDispatch(
                parent: observer,
                dispatcher: dispatcher
            );

        /// <summary>
        /// EventObserver that debounces events.
        /// </summary>
        /// <param name="delay">The delay period for debouncing.</param>
        /// <param name="dispatcher">
        /// The dispatcher to run on after the delay has passed.
        /// If not set the debounce will be run under the synchronization
        /// context of the thread this method was called on.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver Debounce(
            TimeSpan delay,
            IDispatcher? dispatcher = default
        ) =>
            new EventObserverDebounce(
                parent: observer,
                delay: delay,
                dispatcher: dispatcher
            );

        /// <summary>
        /// EventObserver that throttles events.
        /// </summary>
        /// <param name="limit">The limit for throttling.</param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver Throttle(TimeSpan limit) =>
            new EventObserverThrottle(
                parent: observer,
                limit: limit
            );

        /// <summary>
        /// EventObserver that throttles events.
        /// </summary>
        /// <param name="limit">The limit for throttling.</param>
        /// <param name="dispatcher">
        /// The dispatcher to use to potentially invoke the action in the future
        /// if it was throttled.  If not set the action will be run under the
        /// synchronization context of the thread this method was called on.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver ThrottleLatest(
            TimeSpan limit,
            IDispatcher? dispatcher = default
        ) =>
            new EventObserverThrottleLatest(
                parent: observer,
                limit: limit,
                dispatcher: dispatcher
            );

        /// <summary>
        /// Invoke the given callback whenever an exception thrown below this
        /// observer in the chain is thrown.  Caught exceptions will not move
        /// past this observer.
        /// </summary>
        /// <param name="callback">
        /// The callback to invoke whenever an exception is caught.  If the
        /// callback returns false the exception will continue up the chain.  If
        /// the callback returns true the exception will not continue up the
        /// chain.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver Catch(Func<Exception, bool> callback) =>
            new EventObserverCatch(
                parent: observer,
                callback: callback
            );

        /// <summary>
        /// Create an observable that will emit when the parent observer
        /// observes an event.
        /// </summary>
        /// <returns>
        /// A new observable.
        /// </returns>
        public IEventObservable ToObservable() =>
            new EventObserverEventObservable(
                parent: observer
            ).EventObservable;
    }

    /// <summary>
    /// Extension members for <seealso cref="IEventObserver{TArgs}"/>.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments being extended.
    /// </typeparam>
    /// <param name="observer">The instance being extended.</param>
    extension<TArgs>(IEventObserver<TArgs> observer)
    {
        /// <summary>
        /// Invoke the given callback whenever the observable event is raised.
        /// </summary>
        /// <param name="callback">
        /// The callback to invoke when the observable event is raised.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver<TArgs> OnEach(Action<TArgs> callback) =>
            new EventObserverOnEach<TArgs>(
                parent: observer,
                callback: callback
            );

        /// <summary>
        /// Select a value to pass to chained observers.
        /// </summary>
        /// <typeparam name="TArgsOut">
        /// The type of arguments that will be passed to chained observers.
        /// </typeparam>
        /// <param name="selector">
        /// The callback to invoke to select the arguments to pass to chained
        /// observers.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver<TArgsOut> Select<TArgsOut>(
            Func<TArgs, TArgsOut> selector
        ) =>
            new EventObserverSelect<TArgs, TArgsOut>(
                parent: observer,
                selector: selector
            );

        /// <summary>
        /// Filter out observable events that don't match the predicate and
        /// prevent them from being passed down the observer chain.
        /// </summary>
        /// <param name="predicate">
        /// The callback invoked to determine if the observable event should be
        /// filtered out.  If the callback returns false the observable event
        /// will be filtered out.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver<TArgs> Where(Func<TArgs, bool> predicate) =>
            new EventObserverWhere<TArgs>(
                parent: observer,
                predicate: predicate
            );

        /// <summary>
        /// Dispose of the observer chain if the predicate is matched.
        /// </summary>
        /// <param name="predicate">
        /// The callback invoked to determine if the observer chain should be
        /// disposed.  If the callback returns true the observer chain is
        /// disposed.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver<TArgs> Until(
            Func<TArgs, bool> predicate
        ) =>
            new EventObserverUntil<TArgs>(
                parent: observer,
                predicate: predicate
            );

        /// <summary>
        /// Dispose of the observer chain when the given cancellation token is
        /// cancelled.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token that that cancelled will dispose of this
        /// observer.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver<TArgs> Until(CancellationToken cancellationToken) =>
            new EventObserverUntil<TArgs>(
                parent: observer,
                cancellationToken: cancellationToken
            );

        /// <summary>
        /// Dispose of the observer chain when the given observer emits.
        /// </summary>
        /// <param name="disposeObserver">
        /// The observer that when emits will dispose of this observer.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver<TArgs> Until(IEventObserver disposeObserver) =>
            new EventObserverUntil<TArgs>(
                parent: observer,
                disposeObserver: disposeObserver.ToObservable().Observe()
            );

        /// <summary>
        /// Dispose of the observer chain when the given observable emits.
        /// </summary>
        /// <param name="disposeObservable">
        /// The observable that when emits will dispose of this observer.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver Until(IEventObservable disposeObservable) =>
            new EventObserverUntil<TArgs>(
                parent: observer,
                disposeObserver: disposeObservable.Observe()
            );

        /// <summary>
        /// Dispose of the observer chain when the given observable emits.
        /// </summary>
        /// <param name="disposeObservable">
        /// The observable that when emits will dispose of this observer.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver Until<TDisposeArgs>(IEventObservable<TDisposeArgs> disposeObservable) =>
            new EventObserverUntil<TArgs>(
                parent: observer,
                disposeObserver: disposeObservable.Observe()
            );

        /// <summary>
        /// Dispose of the observer chain when the first event is observed.
        /// </summary>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver<TArgs> Once() =>
            Until(
                observer: observer,
                predicate: _ => true
            );

        /// <summary>
        /// Invoke the next observers using the provided dispatcher.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher to invoke the next observers with.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver<TArgs> Dispatch(
            IDispatcher dispatcher
        ) =>
            new EventObserverDispatch<TArgs>(
                parent: observer,
                dispatcher: dispatcher
            );

        /// <summary>
        /// EventObserver that debounces events.
        /// </summary>
        /// <param name="delay">The delay period for debouncing.</param>
        /// <param name="dispatcher">
        /// The dispatcher to run on after the delay has passed.
        /// If not set the debounce will be run under the synchronization
        /// context of the thread this method was called on.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver<TArgs> Debounce(
            TimeSpan delay,
            IDispatcher? dispatcher = default
        ) =>
            new EventObserverDebounce<TArgs>(
                parent: observer,
                delay: delay,
                dispatcher: dispatcher
            );

        /// <summary>
        /// EventObserver that throttles events.
        /// </summary>
        /// <param name="limit">The limit for throttling.</param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver<TArgs> Throttle(TimeSpan limit) =>
            new EventObserverThrottle<TArgs>(
                parent: observer,
                limit: limit
            );

        /// <summary>
        /// EventObserver that throttles events.
        /// </summary>
        /// <param name="limit">The limit for throttling.</param>
        /// <param name="dispatcher">
        /// The dispatcher to use to potentially invoke the action in the future
        /// if it was throttled.  If not set the action will be run under the
        /// synchronization context of the thread this method was called on.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver<TArgs> ThrottleLatest(
            TimeSpan limit,
            IDispatcher? dispatcher = default
        ) =>
            new EventObserverThrottleLatest<TArgs>(
                parent: observer,
                limit: limit,
                dispatcher: dispatcher
            );

        /// <summary>
        /// Invoke the given callback whenever an exception thrown below this
        /// observer in the chain is thrown.  Caught exceptions will not move
        /// past this observer.
        /// </summary>
        /// <param name="callback">
        /// The callback to invoke whenever an exception is caught.  If the
        /// callback returns false the exception will continue up the chain.  If
        /// the callback returns true the exception will not continue up the
        /// chain.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IEventObserver<TArgs> Catch(Func<TArgs, Exception, bool> callback) =>
            new EventObserverCatch<TArgs>(
                parent: observer,
                callback: callback
            );

        /// <summary>
        /// Create an observable that will emit when the parent observer
        /// observes an event.
        /// </summary>
        /// <returns>
        /// A new observable.
        /// </returns>
        public IEventObservable<TArgs> ToObservable() =>
            new EventObserverEventObservable<TArgs>(
                parent: observer
            ).EventObservable;
    }
}
