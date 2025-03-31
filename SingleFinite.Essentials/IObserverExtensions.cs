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
    /// <summary>
    /// Invoke the given callback whenever the observable event is raised.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke when the observable event is raised.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver OnEach(this IObserver observer, Action callback) =>
        new ObserverForEach(observer, callback);

    /// <summary>
    /// Invoke the given callback whenever the observable event is raised.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke when the observable event is raised.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> OnEach<TArgs>(
        this IObserver<TArgs> observer,
        Action<TArgs> callback
    ) => new ObserverForEach<TArgs>(observer, callback);

    /// <summary>
    /// Select a value to pass to chained observers.
    /// </summary>
    /// <typeparam name="TArgsOut">
    /// The type of arguments that will be passed to chained observers.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="selector">
    /// The callback to invoke to select the arguments to pass to chained
    /// observers.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgsOut> Select<TArgsOut>(
        this IObserver observer,
        Func<TArgsOut> selector
    ) => new ObserverSelect<TArgsOut>(observer, selector);

    /// <summary>
    /// Select a value to pass to chained observers.
    /// </summary>
    /// <typeparam name="TArgsIn">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <typeparam name="TArgsOut">
    /// The type of arguments that will be passed to chained observers.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="selector">
    /// The callback to invoke to select the arguments to pass to chained
    /// observers.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgsOut> Select<TArgsIn, TArgsOut>(
        this IObserver<TArgsIn> observer,
        Func<TArgsIn, TArgsOut> selector
    ) => new ObserverSelect<TArgsIn, TArgsOut>(observer, selector);

    /// <summary>
    /// Filter out observable events that don't match the predicate and prevent
    /// them from being passed down the observer chain.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observable event should be
    /// filtered out.  If the callback returns false the observable event will
    /// be filtered out.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver Where(
        this IObserver observer,
        Func<bool> predicate
    ) => new ObserverWhere(observer, predicate);

    /// <summary>
    /// Filter out observable events that don't match the predicate and prevent
    /// them from being passed down the observer chain.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observable event should be
    /// filtered out.  If the callback returns false the observable event will
    /// be filtered out.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> Where<TArgs>(
        this IObserver<TArgs> observer,
        Func<TArgs, bool> predicate
    ) => new ObserverWhere<TArgs>(observer, predicate);

    /// <summary>
    /// Dispose of the observer chain if the predicate is matched.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observer chain should be
    /// disposed.  If the callback returns true the observer chain is disposed.
    /// </param>
    /// <param name="continueOnDispose">
    /// When set to true the next observer in the observer chain will be invoked
    /// even when predicate returns true and this observer chain will be
    /// disposed.  Default is false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver Until(
        this IObserver observer,
        Func<bool> predicate,
        bool continueOnDispose = false
    ) => new ObserverUntil(observer, predicate, continueOnDispose);

    /// <summary>
    /// Dispose of the observer chain if the predicate is matched.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="predicate">
    /// The callback invoked to determine if the observer chain should be
    /// disposed.  If the callback returns true the observer chain is disposed.
    /// </param>
    /// <param name="continueOnDispose">
    /// When set to true the next observer in the observer chain will be invoked
    /// even when predicate returns true and this observer chain will be
    /// disposed.  Default is false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> Until<TArgs>(
        this IObserver<TArgs> observer,
        Func<TArgs, bool> predicate,
        bool continueOnDispose = false
    ) => new ObserverUntil<TArgs>(observer, predicate, continueOnDispose);

    /// <summary>
    /// Dispose of the observer chain when the first event is observed.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver Once(
        this IObserver observer
    ) => Until(
        observer,
        predicate: () => true,
        continueOnDispose: true
    );

    /// <summary>
    /// Dispose of the observer chain when the first event is observed.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with the observed event.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> Once<TArgs>(
        this IObserver<TArgs> observer
    ) => Until(
        observer,
        predicate: _ => true,
        continueOnDispose: true
    );

    /// <summary>
    /// Invoke the next observers using the provided dispatcher.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="dispatcher">
    /// The dispatcher to invoke the next observers with.
    /// </param>
    /// <param name="onError">
    /// Optional action invoked if the next observers throw an exception.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver Dispatch(
        this IObserver observer,
        IDispatcher dispatcher,
        Action<Exception>? onError = default
    ) => new ObserverDispatch(observer, dispatcher, onError);

    /// <summary>
    /// Invoke the next observers using the provided dispatcher.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="dispatcher">
    /// The dispatcher to invoke the next observers with.
    /// </param>
    /// <param name="onError">
    /// Optional action invoked if the next observers throw an exception.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> Dispatch<TArgs>(
        this IObserver<TArgs> observer,
        IDispatcher dispatcher,
        Action<Exception>? onError = default
    ) => new ObserverDispatch<TArgs>(observer, dispatcher, onError);

    /// <summary>
    /// Dispose of the observer chain when the given object is disposed.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="disposeObservable">
    /// The object that when disposed will dispose of this observer.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver On(
        this IObserver observer,
        IDisposeObservable disposeObservable
    ) => new ObserverOn(
        observer,
        disposeObservable
    );

    /// <summary>
    /// Dispose of the observer chain when the given object is disposed.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with the observed event.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="disposeObservable">
    /// The object that when disposed will dispose of this observer.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> On<TArgs>(
        this IObserver<TArgs> observer,
        IDisposeObservable disposeObservable
    ) => new ObserverOn<TArgs>(
        observer,
        disposeObservable
    );

    /// <summary>
    /// Dispose of the observer chain when the given cancellation token is
    /// cancelled.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="cancellationToken">
    /// The cancellation token that when cancelled will dispose of this
    /// observer.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver On(
        this IObserver observer,
        CancellationToken cancellationToken
    ) => new ObserverOn(
        observer,
        cancellationToken
    );

    /// <summary>
    /// Dispose of the observer chain when the given cancellation token is
    /// cancelled.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with the observed event.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="cancellationToken">
    /// The cancellation token that that cancelled will dispose of this
    /// observer.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> On<TArgs>(
        this IObserver<TArgs> observer,
        CancellationToken cancellationToken
    ) => new ObserverOn<TArgs>(
        observer,
        cancellationToken
    );

    /// <summary>
    /// Observer that debounces events.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="delay">The delay period for debouncing.</param>
    /// <param name="dispatcher">
    /// The dispatcher to run on after the delay has passed.
    /// If not set the debounce will be run on the calling thread.
    /// </param>
    /// <param name="onError">
    /// Optional action invoked if the debounced action throws an exception.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver Debounce(
        this IObserver observer,
        TimeSpan delay,
        IDispatcher? dispatcher = default,
        Action<Exception>? onError = default
    ) => new ObserverDebounce(
        observer,
        delay,
        dispatcher,
        onError
    );

    /// <summary>
    /// Observer that debounces events.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with observed events.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="delay">The delay period for debouncing.</param>
    /// <param name="dispatcher">
    /// The dispatcher to run on after the delay has passed.
    /// If not set the debounce will be run on the calling thread.
    /// </param>
    /// <param name="onError">
    /// Optional action invoked if the debounced action throws an exception.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> Debounce<TArgs>(
        this IObserver<TArgs> observer,
        TimeSpan delay,
        IDispatcher? dispatcher = default,
        Action<Exception>? onError = default
    ) => new ObserverDebounce<TArgs>(
        observer,
        delay,
        dispatcher,
        onError
    );

    /// <summary>
    /// Observer that throttles events.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="limit">The limit for throttling.</param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver Throttle(
        this IObserver observer,
        TimeSpan limit
    ) => new ObserverThrottle(
        observer,
        limit
    );

    /// <summary>
    /// Observer that throttles events.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with observed events.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="limit">The limit for throttling.</param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> Throttle<TArgs>(
        this IObserver<TArgs> observer,
        TimeSpan limit
    ) => new ObserverThrottle<TArgs>(
        observer,
        limit
    );

    /// <summary>
    /// Observer that throttles events.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
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
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver ThrottleBuffer(
        this IObserver observer,
        TimeSpan limit,
        IDispatcher? dispatcher = default,
        Action<Exception>? onError = default
    ) => new ObserverThrottleBuffer(
        observer,
        limit,
        dispatcher,
        onError
    );

    /// <summary>
    /// Observer that throttles events.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with observed events.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
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
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> ThrottleBuffer<TArgs>(
        this IObserver<TArgs> observer,
        TimeSpan limit,
        IDispatcher? dispatcher = default,
        Action<Exception>? onError = default
    ) => new ObserverThrottleBuffer<TArgs>(
        observer,
        limit,
        dispatcher,
        onError
    );

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.  Caught exceptions will not move past
    /// this observer.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke whenever an exception is caught.  If the callback
    /// returns false the exception will continue up the chain.  If the callback
    /// returns true the exception will not continue up the chain.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver Catch(
        this IObserver observer,
        Func<Exception, bool> callback
    ) => new ObserverCatch(observer, callback);

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.  Caught exceptions will not move past
    /// this observer.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke whenever an exception is caught.  If the callback
    /// returns false the exception will continue up the chain.  If the callback
    /// returns true the exception will not continue up the chain.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IObserver<TArgs> Catch<TArgs>(
        this IObserver<TArgs> observer,
        Func<TArgs, Exception, bool> callback
    ) => new ObserverCatch<TArgs>(observer, callback);

    /// <summary>
    /// Create an observer that raises the next event in the observer chain as
    /// async by using the provided dispatcher..
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="dispatcher">
    /// The dispatcher the async events will be run with.
    /// </param>
    /// <param name="onError">
    /// Optional action that is invoked if an exception is thrown from the async
    /// observers.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver ToAsync(
        this IObserver observer,
        IDispatcher dispatcher,
        Action<Exception>? onError = default
    ) => new ObserverToAsync(observer, dispatcher, onError);

    /// <summary>
    /// Create an observer that raises the next event in the observer chain as
    /// async by using the provided dispatcher..
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with observed events.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="dispatcher">
    /// The dispatcher the async events will be run with.
    /// </param>
    /// <param name="onError">
    /// Optional action that is invoked if an exception is thrown from the async
    /// observers.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> ToAsync<TArgs>(
        this IObserver<TArgs> observer,
        IDispatcher dispatcher,
        Action<Exception>? onError = default
    ) => new ObserverToAsync<TArgs>(observer, dispatcher, onError);

    /// <summary>
    /// Create an observable that will emit when the parent observer observes
    /// an event.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <returns>
    /// A new observable.
    /// </returns>
    public static Observable ToObservable(
        this IObserver observer
    ) => new ObserverObservable(observer).Observable;

    /// <summary>
    /// Create an observable that will emit when the parent observer observes
    /// an event.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with observed events.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <returns>
    /// A new observable.
    /// </returns>
    public static Observable<TArgs> ToObservable<TArgs>(
        this IObserver<TArgs> observer
    ) => new ObserverObservable<TArgs>(observer).Observable;
}
