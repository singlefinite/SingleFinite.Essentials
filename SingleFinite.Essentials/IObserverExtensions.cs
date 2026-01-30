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

using SingleFinite.Essentials.Internal.Observers;

namespace SingleFinite.Essentials;

/// <summary>
/// Methods that modify how an observable event is handled.  Observers are
/// chained together to create different handler logic.
/// </summary>
public static class IObserverExtensions
{
    extension(IObserver observer)
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
        public IObserver OnEach(Action callback) =>
            new ObserverForEach(
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
        public IObserver<TArgsOut> Select<TArgsOut>(Func<TArgsOut> selector) =>
            new ObserverSelect<TArgsOut>(
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
        public IObserver Where(Func<bool> predicate) =>
            new ObserverWhere(
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
        public IObserver Until(
            Func<bool> predicate
        ) =>
            new ObserverUntil(
                parent: observer,
                predicate: predicate
            );

        /// <summary>
        /// Dispose of the observer chain when the given object is disposed.
        /// </summary>
        /// <param name="disposeObservable">
        /// The object that when disposed will dispose of this observer.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IObserver Until(IDisposeObservable disposeObservable) =>
            new ObserverUntil(
                parent: observer,
                disposeObservable: disposeObservable
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
        public IObserver Until(CancellationToken cancellationToken) =>
            new ObserverUntil(
                parent: observer,
                cancellationToken: cancellationToken
            );

        /// <summary>
        /// Dispose of the observer chain when the first event is observed.
        /// </summary>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IObserver Once() =>
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
        public IObserver Dispatch(
            IDispatcher dispatcher
        ) =>
            new ObserverDispatch(
                parent: observer,
                dispatcher: dispatcher
            );

        /// <summary>
        /// Observer that debounces events.
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
        public IObserver Debounce(
            TimeSpan delay,
            IDispatcher? dispatcher = default
        ) =>
            new ObserverDebounce(
                parent: observer,
                delay: delay,
                dispatcher: dispatcher
            );

        /// <summary>
        /// Observer that throttles events.
        /// </summary>
        /// <param name="limit">The limit for throttling.</param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IObserver Throttle(TimeSpan limit) =>
            new ObserverThrottle(
                parent: observer,
                limit: limit
            );

        /// <summary>
        /// Observer that throttles events.
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
        public IObserver ThrottleLatest(
            TimeSpan limit,
            IDispatcher? dispatcher = default
        ) =>
            new ObserverThrottleLatest(
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
        public IObserver Catch(Func<Exception, bool> callback) =>
            new ObserverCatch(
                parent: observer,
                callback: callback
            );

        /// <summary>
        /// Create an observer that raises the next event in the observer chain
        /// as async by using the provided dispatcher.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher the async events will be run with.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IAsyncObserver ToAsync(
            IDispatcher dispatcher
        ) =>
            new ObserverToAsync(
                parent: observer,
                dispatcher: dispatcher
            );

        /// <summary>
        /// Create an observable that will emit when the parent observer
        /// observes an event.
        /// </summary>
        /// <returns>
        /// A new observable.
        /// </returns>
        public Observable ToObservable() =>
            new ObserverObservable(
                parent: observer
            ).Observable;
    }

    extension<TArgs>(IObserver<TArgs> observer)
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
        public IObserver<TArgs> OnEach(Action<TArgs> callback) =>
            new ObserverForEach<TArgs>(
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
        public IObserver<TArgsOut> Select<TArgsOut>(
            Func<TArgs, TArgsOut> selector
        ) =>
            new ObserverSelect<TArgs, TArgsOut>(
                parent: observer,
                selector: selector
            );

        /// <summary>
        /// Turn the observer that has an argument into an observer that doesn't
        /// have an argument.
        /// </summary>
        /// <returns>A new observer that doesn't have an argument.</returns>
        public IObserver Select() =>
            new ObserverSelectNone<TArgs>(
                parent: observer
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
        public IObserver<TArgs> Where(Func<TArgs, bool> predicate) =>
            new ObserverWhere<TArgs>(
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
        public IObserver<TArgs> Until(
            Func<TArgs, bool> predicate
        ) =>
            new ObserverUntil<TArgs>(
                parent: observer,
                predicate: predicate
            );

        /// <summary>
        /// Dispose of the observer chain when the given object is disposed.
        /// </summary>
        /// <param name="disposeObservable">
        /// The object that when disposed will dispose of this observer.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IObserver<TArgs> Until(IDisposeObservable disposeObservable) =>
            new ObserverUntil<TArgs>(
                parent: observer,
                disposeObservable: disposeObservable
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
        public IObserver<TArgs> Until(CancellationToken cancellationToken) =>
            new ObserverUntil<TArgs>(
                parent: observer,
                cancellationToken: cancellationToken
            );

        /// <summary>
        /// Dispose of the observer chain when the first event is observed.
        /// </summary>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IObserver<TArgs> Once() =>
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
        public IObserver<TArgs> Dispatch(
            IDispatcher dispatcher
        ) =>
            new ObserverDispatch<TArgs>(
                parent: observer,
                dispatcher: dispatcher
            );

        /// <summary>
        /// Observer that debounces events.
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
        public IObserver<TArgs> Debounce(
            TimeSpan delay,
            IDispatcher? dispatcher = default
        ) =>
            new ObserverDebounce<TArgs>(
                parent: observer,
                delay: delay,
                dispatcher: dispatcher
            );

        /// <summary>
        /// Observer that throttles events.
        /// </summary>
        /// <param name="limit">The limit for throttling.</param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IObserver<TArgs> Throttle(TimeSpan limit) =>
            new ObserverThrottle<TArgs>(
                parent: observer,
                limit: limit
            );

        /// <summary>
        /// Observer that throttles events.
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
        public IObserver<TArgs> ThrottleLatest(
            TimeSpan limit,
            IDispatcher? dispatcher = default
        ) =>
            new ObserverThrottleLatest<TArgs>(
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
        public IObserver<TArgs> Catch(Func<TArgs, Exception, bool> callback) =>
            new ObserverCatch<TArgs>(
                parent: observer,
                callback: callback
            );

        /// <summary>
        /// Create an observer that raises the next event in the observer chain
        /// as async by using the provided dispatcher.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher the async events will be run with.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IAsyncObserver<TArgs> ToAsync(
            IDispatcher dispatcher
        ) =>
            new ObserverToAsync<TArgs>(
                parent: observer,
                dispatcher: dispatcher
            );

        /// <summary>
        /// Create an observable that will emit when the parent observer
        /// observes an event.
        /// </summary>
        /// <returns>
        /// A new observable.
        /// </returns>
        public Observable<TArgs> ToObservable() =>
            new ObserverObservable<TArgs>(
                parent: observer
            ).Observable;
    }
}
