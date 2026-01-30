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

using SingleFinite.Essentials.Internal.Observers;

namespace SingleFinite.Essentials;

/// <summary>
/// Methods that modify how an observable event is handled.  Observers are
/// chained together to create different handler logic.
/// </summary>
public static class IAsyncObserverExtensions
{
    extension(IAsyncObserver observer)
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
        public IAsyncObserver OnEach(Func<Task> callback) =>
            new AsyncObserverForEach(
                parent: observer,
                callback: callback
            );

        /// <summary>
        /// Invoke the given callback whenever the observable event is raised.
        /// </summary>
        /// <param name="callback">
        /// The callback to invoke when the observable event is raised.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IAsyncObserver OnEach(Action callback) =>
            new AsyncObserverForEach(
                parent: observer,
                callback: () =>
                {
                    callback();
                    return Task.CompletedTask;
                }
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
        public IAsyncObserver<TArgsOut> Select<TArgsOut>(
            Func<Task<TArgsOut>> selector
        ) =>
            new AsyncObserverSelect<TArgsOut>(
                parent: observer,
                selector: selector
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
        public IAsyncObserver<TArgsOut> Select<TArgsOut>(
            Func<TArgsOut> selector
        ) =>
            new AsyncObserverSelect<TArgsOut>(
                parent: observer,
                selector: () => Task.FromResult(selector())
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
        public IAsyncObserver Where(Func<Task<bool>> predicate) =>
            new AsyncObserverWhere(
                parent: observer,
                predicate: predicate
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
        public IAsyncObserver Where(Func<bool> predicate) =>
            new AsyncObserverWhere(
                parent: observer,
                predicate: () => Task.FromResult(predicate())
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
        public IAsyncObserver Until(
            Func<Task<bool>> predicate
        ) =>
            new AsyncObserverUntil(
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
        public IAsyncObserver Until(
            Func<bool> predicate
        ) =>
            new AsyncObserverUntil(
                parent: observer,
                predicate: () => Task.FromResult(predicate())
            );

        /// <summary>
        /// Dispose of the observer chain when the given object is disposed.
        /// </summary>
        /// <param name="disposeObservable">
        /// The object that that when disposed will dispose of this observer.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IAsyncObserver Until(IDisposeObservable disposeObservable) =>
            new AsyncObserverUntil(
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
        public IAsyncObserver Until(CancellationToken cancellationToken) =>
            new AsyncObserverUntil(
                parent: observer,
                cancellationToken: cancellationToken
            );

        /// <summary>
        /// Dispose of the observer chain when the first event is observed.
        /// </summary>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IAsyncObserver Once() =>
            Until(
                observer: observer,
                predicate: () => Task.FromResult(true)
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
        public IAsyncObserver Dispatch(IDispatcher dispatcher) =>
            new AsyncObserverDispatch(
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
        public IAsyncObserver Debounce(
            TimeSpan delay,
            IDispatcher? dispatcher = default
        ) =>
            new AsyncObserverDebounce(
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
        public IAsyncObserver Throttle(TimeSpan limit) =>
            new AsyncObserverThrottle(
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
        public IAsyncObserver ThrottleLatest(
            TimeSpan limit,
            IDispatcher? dispatcher = default
        ) =>
            new AsyncObserverThrottleLatest(
                parent: observer,
                limit: limit,
                dispatcher: dispatcher
            );

        /// <summary>
        /// Invoke the given callback whenever an exception thrown below this
        /// observer in the chain is thrown.
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
        public IAsyncObserver Catch(Func<Exception, Task<bool>> callback) =>
            new AsyncObserverCatch(
                parent: observer,
                callback: callback
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
        public IAsyncObserver Catch(Func<Exception, bool> callback) =>
            new AsyncObserverCatch(
                parent: observer,
                callback: ex => Task.FromResult(callback(ex))
            );

        /// <summary>
        /// Limit the number of observers that can be executing at the same
        /// time.
        /// </summary>
        /// <param name="maxConcurrent">
        /// The max number of observers allowed to be executing at the same
        /// time.  If a new event occurs and there are already max number of
        /// observers executing the event will be buffered until enough
        /// executing observers complete to allow the new event to be passed on.
        /// Must be zero or greater or an exception will be thrown.
        /// </param>
        /// <param name="maxBuffer">
        /// The max number of events that will be buffered if there are already
        /// max number of concurrent observers executing.  If a new event occurs
        /// and there are already max number of events buffered the event will
        /// be ignored.  If this is set to -1 there is no limit.  Default is -1.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if maxConcurrent is less than zero.
        /// </exception>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IAsyncObserver Limit(
            int maxConcurrent,
            int maxBuffer = -1
        ) =>
            new AsyncObserverLimit(
                parent: observer,
                maxConcurrent: maxConcurrent,
                maxBuffer: maxBuffer
            );

        /// <summary>
        /// Turn this async observer into a synchronous observer.
        /// </summary>
        public IObserver ToSync() =>
            new AsyncObserverToSync(
                parent: observer
            );

        /// <summary>
        /// Create an observable that will emit when the parent observer
        /// observes an event.
        /// </summary>
        /// <returns>
        /// A new observable.
        /// </returns>
        public AsyncObservable ToObservable() =>
            new AsyncObserverObservable(
                parent: observer
            ).Observable;
    }

    extension<TArgs>(IAsyncObserver<TArgs> observer)
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
        public IAsyncObserver<TArgs> OnEach(Func<TArgs, Task> callback) =>
            new AsyncObserverForEach<TArgs>(
                parent: observer,
                callback: callback
            );

        /// <summary>
        /// Invoke the given callback whenever the observable event is raised.
        /// </summary>
        /// <param name="callback">
        /// The callback to invoke when the observable event is raised.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IAsyncObserver<TArgs> OnEach(Action<TArgs> callback) =>
            new AsyncObserverForEach<TArgs>(
                parent: observer,
                callback: args =>
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
        /// <param name="selector">
        /// The callback to invoke to select the arguments to pass to chained
        /// observers.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IAsyncObserver<TArgsOut> Select<TArgsOut>(
            Func<TArgs, Task<TArgsOut>> selector
        ) =>
            new AsyncObserverSelect<TArgs, TArgsOut>(
                parent: observer,
                selector: selector
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
        public IAsyncObserver<TArgsOut> Select<TArgsOut>(
            Func<TArgs, TArgsOut> selector
        ) =>
            new AsyncObserverSelect<TArgs, TArgsOut>(
                parent: observer,
                selector: (args) => Task.FromResult(selector(args))
            );

        /// <summary>
        /// Turn the observer that has an argument into an observer that doesn't
        /// have an argument.
        /// </summary>
        /// <returns>A new observer that doesn't have an argument.</returns>
        public IAsyncObserver Select() =>
            new AsyncObserverSelectNone<TArgs>(
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
        public IAsyncObserver<TArgs> Where(Func<TArgs, Task<bool>> predicate) =>
            new AsyncObserverWhere<TArgs>(
                parent: observer,
                predicate: predicate
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
        public IAsyncObserver<TArgs> Where(Func<TArgs, bool> predicate) =>
            new AsyncObserverWhere<TArgs>(
                parent: observer,
                predicate: args => Task.FromResult(predicate(args))
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
        public IAsyncObserver<TArgs> Until(
            Func<TArgs, Task<bool>> predicate
        ) =>
            new AsyncObserverUntil<TArgs>(
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
        public IAsyncObserver<TArgs> Until(
            Func<TArgs, bool> predicate
        ) =>
            new AsyncObserverUntil<TArgs>(
                parent: observer,
                predicate: args => Task.FromResult(predicate(args))
            );

        /// <summary>
        /// Dispose of the observer chain when the given object is disposed.
        /// </summary>
        /// <param name="disposeObservable">
        /// The object that that when disposed will dispose of this observer.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IAsyncObserver<TArgs> Until(IDisposeObservable disposeObservable) =>
            new AsyncObserverUntil<TArgs>(
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
        public IAsyncObserver<TArgs> Until(CancellationToken cancellationToken) =>
            new AsyncObserverUntil<TArgs>(
                parent: observer,
                cancellationToken: cancellationToken
            );

        /// <summary>
        /// Dispose of the observer chain when the first event is observed.
        /// </summary>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IAsyncObserver<TArgs> Once() =>
            Until(
                observer: observer,
                predicate: _ => Task.FromResult(true)
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
        public IAsyncObserver<TArgs> Dispatch(IDispatcher dispatcher) =>
            new AsyncObserverDispatch<TArgs>(
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
        public IAsyncObserver<TArgs> Debounce(
            TimeSpan delay,
            IDispatcher? dispatcher = default
        ) =>
            new AsyncObserverDebounce<TArgs>(
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
        public IAsyncObserver<TArgs> Throttle(TimeSpan limit) =>
            new AsyncObserverThrottle<TArgs>(
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
        public IAsyncObserver<TArgs> ThrottleLatest(
            TimeSpan limit,
            IDispatcher? dispatcher = default
        ) =>
            new AsyncObserverThrottleLatest<TArgs>(
                parent: observer,
                limit: limit,
                dispatcher: dispatcher
            );

        /// <summary>
        /// Invoke the given callback whenever an exception thrown below this
        /// observer in the chain is thrown.
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
        public IAsyncObserver<TArgs> Catch(
            Func<TArgs, Exception, Task<bool>> callback
        ) =>
            new AsyncObserverCatch<TArgs>(
                parent: observer,
                callback: callback
            );

        /// <summary>
        /// Invoke the given callback whenever an exception thrown below this
        /// observer in the chain is thrown.
        /// </summary>
        /// <param name="callback">
        /// The callback to invoke whenever an exception is caught.  If the
        /// exception is not handled it will be rethrown and continue up the
        /// chain. By default the exception is considered handled unless the
        /// IsHandled property of the ExceptionEventArgs is changed to false.
        /// </param>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IAsyncObserver<TArgs> Catch(
            Func<TArgs, Exception, bool> callback
        ) =>
            new AsyncObserverCatch<TArgs>(
                parent: observer,
                callback: (args, ex) => Task.FromResult(callback(args, ex))
            );

        /// <summary>
        /// Limit the number of observers that can be executing at the same
        /// time.
        /// </summary>
        /// <param name="maxConcurrent">
        /// The max number of observers allowed to be executing at the same
        /// time.  If a new event occurs and there are already max number of
        /// observers executing the event will be buffered until enough
        /// executing observers complete to allow the new event to be passed on.
        /// Must be zero or greater or an exception will be thrown.
        /// </param>
        /// <param name="maxBuffer">
        /// The max number of events that will be buffered if there are already
        /// max number of concurrent observers executing.  If a new event occurs
        /// and there are already max number of events buffered the event will
        /// be ignored.  If this is set to -1 there is no limit.  Default is -1.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if maxConcurrent is less than zero.
        /// </exception>
        /// <returns>
        /// A new observer that has been added to the chain of observers.
        /// </returns>
        public IAsyncObserver<TArgs> Limit(
            int maxConcurrent,
            int maxBuffer = -1
        ) =>
            new AsyncObserverLimit<TArgs>(
                parent: observer,
                maxConcurrent: maxConcurrent,
                maxBuffer: maxBuffer
            );

        /// <summary>
        /// Turn this async observer into a synchronous observer.
        /// </summary>
        public IObserver<TArgs> ToSync() =>
            new AsyncObserverToSync<TArgs>(
                parent: observer
            );

        /// <summary>
        /// Create an observable that will emit when the parent observer
        /// observes an event.
        /// </summary>
        /// <returns>
        /// A new observable.
        /// </returns>
        public AsyncObservable<TArgs> ToObservable() =>
            new AsyncObserverObservable<TArgs>(
                parent: observer
            ).Observable;
    }
}
