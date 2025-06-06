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

using SingleFinite.Essentials.Internal.Observers;

namespace SingleFinite.Essentials;

/// <summary>
/// Methods that modify how an observable event is handled.  Observers are
/// chained together to create different handler logic.
/// </summary>
public static class IAsyncObserverExtensions
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
    public static IAsyncObserver OnEach(
        this IAsyncObserver observer,
        Func<Task> callback
    ) =>
        new AsyncObserverForEach(observer, callback);

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
    public static IAsyncObserver OnEach(
        this IAsyncObserver observer,
        Action callback
    ) =>
        new AsyncObserverForEach(
            observer,
            () =>
            {
                callback();
                return Task.CompletedTask;
            }
        );

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
    public static IAsyncObserver<TArgs> OnEach<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Func<TArgs, Task> callback
    ) => new AsyncObserverForEach<TArgs>(observer, callback);

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
    public static IAsyncObserver<TArgs> OnEach<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Action<TArgs> callback
    ) => new AsyncObserverForEach<TArgs>(
        observer,
        args =>
        {
            callback(args);
            return Task.CompletedTask;
        }
    );

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
    public static IAsyncObserver<TArgsOut> Select<TArgsOut>(
        this IAsyncObserver observer,
        Func<Task<TArgsOut>> selector
    ) => new AsyncObserverSelect<TArgsOut>(observer, selector);

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
    public static IAsyncObserver<TArgsOut> Select<TArgsOut>(
        this IAsyncObserver observer,
        Func<TArgsOut> selector
    ) => new AsyncObserverSelect<TArgsOut>(
        observer,
        () => Task.FromResult(selector())
    );

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
    public static IAsyncObserver<TArgsOut> Select<TArgsIn, TArgsOut>(
        this IAsyncObserver<TArgsIn> observer,
        Func<TArgsIn, Task<TArgsOut>> selector
    ) => new AsyncObserverSelect<TArgsIn, TArgsOut>(observer, selector);

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
    public static IAsyncObserver<TArgsOut> Select<TArgsIn, TArgsOut>(
        this IAsyncObserver<TArgsIn> observer,
        Func<TArgsIn, TArgsOut> selector
    ) => new AsyncObserverSelect<TArgsIn, TArgsOut>(
        observer,
        (args) => Task.FromResult(selector(args))
    );

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
    public static IAsyncObserver Where(
        this IAsyncObserver observer,
        Func<Task<bool>> predicate
    ) => new AsyncObserverWhere(observer, predicate);

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
    public static IAsyncObserver Where(
        this IAsyncObserver observer,
        Func<bool> predicate
    ) => new AsyncObserverWhere(
        observer,
        () => Task.FromResult(predicate())
    );

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
    public static IAsyncObserver<TArgs> Where<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Func<TArgs, Task<bool>> predicate
    ) => new AsyncObserverWhere<TArgs>(observer, predicate);

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
    public static IAsyncObserver<TArgs> Where<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Func<TArgs, bool> predicate
    ) => new AsyncObserverWhere<TArgs>(
        observer,
        args => Task.FromResult(predicate(args))
    );

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
    public static IAsyncObserver Until(
        this IAsyncObserver observer,
        Func<Task<bool>> predicate,
        bool continueOnDispose = false
    ) => new AsyncObserverUntil(observer, predicate, continueOnDispose);

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
    public static IAsyncObserver Until(
        this IAsyncObserver observer,
        Func<bool> predicate,
        bool continueOnDispose = false
    ) => new AsyncObserverUntil(
        observer,
        () => Task.FromResult(predicate()),
        continueOnDispose
    );

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
    public static IAsyncObserver<TArgs> Until<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Func<TArgs, Task<bool>> predicate,
        bool continueOnDispose = false
    ) => new AsyncObserverUntil<TArgs>(
        observer,
        predicate,
        continueOnDispose
    );

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
    public static IAsyncObserver<TArgs> Until<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Func<TArgs, bool> predicate,
        bool continueOnDispose = false
    ) => new AsyncObserverUntil<TArgs>(
        observer,
        args => Task.FromResult(predicate(args)),
        continueOnDispose
    );

    /// <summary>
    /// Dispose of the observer chain when the first event is observed.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver Once(
        this IAsyncObserver observer
    ) => Until(
        observer,
        predicate: () => Task.FromResult(true),
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
    public static IAsyncObserver<TArgs> Once<TArgs>(
        this IAsyncObserver<TArgs> observer
    ) => Until(
        observer,
        predicate: _ => Task.FromResult(true),
        continueOnDispose: true
    );

    /// <summary>
    /// Invoke the next observers using the provided dispatcher.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="dispatcher">
    /// The dispatcher to invoke the next observers with.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver Dispatch(
        this IAsyncObserver observer,
        IDispatcher dispatcher
    ) => new AsyncObserverDispatch(observer, dispatcher);

    /// <summary>
    /// Invoke the next observers using the provided dispatcher.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="dispatcher">
    /// The dispatcher to invoke the next observers with.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> Dispatch<TArgs>(
        this IAsyncObserver<TArgs> observer,
        IDispatcher dispatcher
    ) => new AsyncObserverDispatch<TArgs>(observer, dispatcher);

    /// <summary>
    /// Dispose of the observer chain when the given object is disposed.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="disposeObservable">
    /// The object that that when disposed will dispose of this observer.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver On(
        this IAsyncObserver observer,
        IDisposeObservable disposeObservable
    ) => new AsyncObserverOn(
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
    /// The object that that when disposed will dispose of this observer.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> On<TArgs>(
        this IAsyncObserver<TArgs> observer,
        IDisposeObservable disposeObservable
    ) => new AsyncObserverOn<TArgs>(
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
    public static IAsyncObserver On(
        this IAsyncObserver observer,
        CancellationToken cancellationToken
    ) => new AsyncObserverOn(
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
    public static IAsyncObserver<TArgs> On<TArgs>(
        this IAsyncObserver<TArgs> observer,
        CancellationToken cancellationToken
    ) => new AsyncObserverOn<TArgs>(
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
    /// If not set the debounce will be run under the synchronization context
    /// of the thread this method was called on.
    /// </param>
    /// <param name="onError">
    /// Optional action invoked if the debounced action throws an exception.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver Debounce(
        this IAsyncObserver observer,
        TimeSpan delay,
        IDispatcher? dispatcher = default,
        Action<Exception>? onError = default
    ) => new AsyncObserverDebounce(
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
    /// If not set the debounce will be run under the synchronization context
    /// of the thread this method was called on.
    /// </param>
    /// <param name="onError">
    /// Optional action invoked if the debounced action throws an exception.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> Debounce<TArgs>(
        this IAsyncObserver<TArgs> observer,
        TimeSpan delay,
        IDispatcher? dispatcher = default,
        Action<Exception>? onError = default
    ) => new AsyncObserverDebounce<TArgs>(
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
    public static IAsyncObserver Throttle(
        this IAsyncObserver observer,
        TimeSpan limit
    ) => new AsyncObserverThrottle(
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
    public static IAsyncObserver<TArgs> Throttle<TArgs>(
        this IAsyncObserver<TArgs> observer,
        TimeSpan limit
    ) => new AsyncObserverThrottle<TArgs>(
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
    /// it was throttled.  If not set the action will be run under the
    /// synchronization context of the thread this method was called on.
    /// </param>
    /// <param name="onError">
    /// Optional handler for any exceptions that are thrown by the action when
    /// it is invoked through the dispatcher.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver ThrottleLatest(
        this IAsyncObserver observer,
        TimeSpan limit,
        IDispatcher? dispatcher = default,
        Action<Exception>? onError = default
    ) => new AsyncObserverThrottleLatest(
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
    /// it was throttled.  If not set the action will be run under the
    /// synchronization context of the thread this method was called on.
    /// </param>
    /// <param name="onError">
    /// Optional handler for any exceptions that are thrown by the action when
    /// it is invoked through the dispatcher.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> ThrottleLatest<TArgs>(
        this IAsyncObserver<TArgs> observer,
        TimeSpan limit,
        IDispatcher? dispatcher = default,
        Action<Exception>? onError = default
    ) => new AsyncObserverThrottleLatest<TArgs>(
        observer,
        limit,
        dispatcher,
        onError
    );

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.
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
    public static IAsyncObserver Catch(
        this IAsyncObserver observer,
        Func<Exception, Task<bool>> callback
    ) => new AsyncObserverCatch(observer, callback);

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
    public static IAsyncObserver Catch(
        this IAsyncObserver observer,
        Func<Exception, bool> callback
    ) => new AsyncObserverCatch(
        observer,
        ex => Task.FromResult(callback(ex))
    );

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.
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
    public static IAsyncObserver<TArgs> Catch<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Func<TArgs, Exception, Task<bool>> callback
    ) => new AsyncObserverCatch<TArgs>(observer, callback);

    /// <summary>
    /// Invoke the given callback whenever an exception thrown below this
    /// observer in the chain is thrown.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="callback">
    /// The callback to invoke whenever an exception is caught.  If the
    /// exception is not handled it will be rethrown and continue up the chain.
    /// By default the exception is considered handled unless the IsHandled
    /// property of the ExceptionEventArgs is changed to false.
    /// </param>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> Catch<TArgs>(
        this IAsyncObserver<TArgs> observer,
        Func<TArgs, Exception, bool> callback
    ) => new AsyncObserverCatch<TArgs>(
        observer,
        (args, ex) => Task.FromResult(callback(args, ex))
    );

    /// <summary>
    /// Limit the number of observers that can be executing at the same time.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="maxConcurrent">
    /// The max number of observers allowed to be executing at the same time.
    /// If a new event occurs and there are already max number of observers
    /// executing the event will be buffered until enough executing observers
    /// complete to allow the new event to be passed on.  Must be zero or
    /// greater or an exception will be thrown.
    /// </param>
    /// <param name="maxBuffer">
    /// The max number of events that will be buffered if there are already max
    /// number of concurrent observers executing.  If a new event occurs and there
    /// are already max number of events buffered the event will be ignored.  If
    /// this is set to -1 there is no limit.  Default is -1.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if maxConcurrent is less than zero.
    /// </exception>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver Limit(
        this IAsyncObserver observer,
        int maxConcurrent,
        int maxBuffer = -1
    ) => new AsyncObserverLimit(
        observer,
        maxConcurrent,
        maxBuffer
    );

    /// <summary>
    /// Limit the number of observers that can be executing at the same time.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed into the observer.
    /// </typeparam>
    /// <param name="observer">The observer to extend.</param>
    /// <param name="maxConcurrent">
    /// The max number of observers allowed to be executing at the same time.
    /// If a new event occurs and there are already max number of observers
    /// executing the event will be buffered until enough executing observers
    /// complete to allow the new event to be passed on.  Must be zero or
    /// greater or an exception will be thrown.
    /// </param>
    /// <param name="maxBuffer">
    /// The max number of events that will be buffered if there are already max
    /// number of concurrent observers executing.  If a new event occurs and there
    /// are already max number of events buffered the event will be ignored.  If
    /// this is set to -1 there is no limit.  Default is -1.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if maxConcurrent is less than zero.
    /// </exception>
    /// <returns>
    /// A new observer that has been added to the chain of observers.
    /// </returns>
    public static IAsyncObserver<TArgs> Limit<TArgs>(
        this IAsyncObserver<TArgs> observer,
        int maxConcurrent,
        int maxBuffer = -1
    ) => new AsyncObserverLimit<TArgs>(
        observer,
        maxConcurrent,
        maxBuffer
    );

    /// <summary>
    /// Create an observable that will emit when the parent observer observes
    /// an event.
    /// </summary>
    /// <param name="observer">The observer to extend.</param>
    /// <returns>
    /// A new observable.
    /// </returns>
    public static AsyncObservable ToObservable(
        this IAsyncObserver observer
    ) => new AsyncObserverObservable(observer).Observable;

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
    public static AsyncObservable<TArgs> ToObservable<TArgs>(
        this IAsyncObserver<TArgs> observer
    ) => new AsyncObserverObservable<TArgs>(observer).Observable;
}
