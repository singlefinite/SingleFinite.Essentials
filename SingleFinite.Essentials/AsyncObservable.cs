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
/// Implementation of <see cref="IAsyncObservable"/>.
/// </summary>
public sealed class AsyncObservable : IAsyncObservable
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with
    /// <see cref="AsyncObservableSource"/> objects.
    /// </summary>
    /// <param name="source">
    /// The source that raises the observable event.
    /// </param>
    internal AsyncObservable(AsyncObservableSource source)
    {
        source.Event += EmitAsync;
    }

    #endregion

    #region Methods

    /// <inhertidoc/>
    public IAsyncObserver Observe() => new AsyncObserverEventSource<Func<Task>>(
        register: handler => Event += handler,
        unregister: handler => Event -= handler,
        handler: eventHandler => () => eventHandler()
    );

    /// <summary>
    /// Raise the event for this observable.
    /// </summary>
    /// <returns>The running task.</returns>
    private Task EmitAsync() => Event?.Invoke() ?? Task.CompletedTask;

    /// <summary>
    /// Create an observer for a generic event.
    /// </summary>
    /// <typeparam name="TEventDelegate">The event delegate type.</typeparam>
    /// <param name="register">Action used to register event handler.</param>
    /// <param name="unregister">
    /// Action used to unregister event handler.
    /// </param>
    /// <param name="handler">
    /// Func used to get handler.  The action that raises the Next event
    /// of this observer is passed into the Func.
    /// </param>
    /// <returns>An observer that observes from the event.</returns>
    public static IAsyncObserver Observe<TEventDelegate>(
        Action<TEventDelegate> register,
        Action<TEventDelegate> unregister,
        Func<Func<Task>, TEventDelegate> handler
    ) => new AsyncObserverEventSource<TEventDelegate>(
        register,
        unregister,
        handler
    );

    /// <summary>
    /// Create an observer for a generic event.
    /// </summary>
    /// <typeparam name="TEventDelegate">The event delegate type.</typeparam>
    /// <typeparam name="TArgs">The type of args passed through.</typeparam>
    /// <param name="register">Action used to register event handler.</param>
    /// <param name="unregister">
    /// Action used to unregister event handler.
    /// </param>
    /// <param name="handler">
    /// Func used to get handler.  The action that raises the Next event
    /// of this observer is passed into the Func.
    /// </param>
    /// <returns>An observer that observes from the event.</returns>
    public static IAsyncObserver<TArgs> Observe<TEventDelegate, TArgs>(
        Action<TEventDelegate> register,
        Action<TEventDelegate> unregister,
        Func<Func<TArgs, Task>, TEventDelegate> handler
    ) => new AsyncObserverEventSource<TEventDelegate, TArgs>(
        register,
        unregister,
        handler
    );

    /// <summary>
    /// Combine the given observables into a single observer.
    /// </summary>
    /// <param name="observables">The observables to combine.</param>
    /// <returns>
    /// A new observer that emits when any of the observers emit.
    /// </returns>
    public static IAsyncObserver Combine(
        params IAsyncObservable[] observables
    ) =>
        new AsyncObserverCombine(
            [.. observables.Select(observable => observable.Observe())]
        );

    /// <summary>
    /// Combine the given observers into a single observer.
    /// </summary>
    /// <param name="observers">The observers to combine.</param>
    /// <returns>
    /// A new observer that emits when any of the provided observers emits.
    /// </returns>
    public static IAsyncObserver Combine(params IAsyncObserver[] observers) =>
        new AsyncObserverCombine(observers);

    /// <summary>
    /// Combine the given observables into a single observer.
    /// </summary>
    /// <param name="observables">The observables to combine.</param>
    /// <typeparam name="TArgs">The type of args passed through.</typeparam>
    /// <returns>
    /// A new observer that emits when any of the observers emit.
    /// </returns>
    public static IAsyncObserver<TArgs> Combine<TArgs>(
        params IAsyncObservable<TArgs>[] observables
    ) =>
        new AsyncObserverCombine<TArgs>(
            [.. observables.Select(observable => observable.Observe())]
        );

    /// <summary>
    /// Combine the given observers into a single observer.
    /// </summary>
    /// <typeparam name="TArgs">The type of args passed through.</typeparam>
    /// <param name="observers">The observers to combine.</param>
    /// <returns>
    /// A new observer that emits when any of the observers emit.
    /// </returns>
    public static IAsyncObserver<TArgs> Combine<TArgs>(
        params IAsyncObserver<TArgs>[] observers
    ) =>
        new AsyncObserverCombine<TArgs>(observers);

    #endregion

    #region Events

    /// <inhertidoc/>
    public event Func<Task>? Event;

    #endregion
}

/// <summary>
/// Implementation of <see cref="IAsyncObservable"/>.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with the event.
/// </typeparam>
public sealed class AsyncObservable<TArgs> : IAsyncObservable<TArgs>
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with
    /// <see cref="AsyncObservableSource"/> objects.
    /// </summary>
    /// <param name="source">
    /// The source that raises the observable event.
    /// </param>
    internal AsyncObservable(AsyncObservableSource<TArgs> source)
    {
        source.Event += EmitAsync;
    }

    #endregion

    #region Methods

    /// <inhertidoc/>
    public IAsyncObserver<TArgs> Observe() =>
        new AsyncObserverEventSource<Func<TArgs, Task>, TArgs>(
            register: handler => Event += handler,
            unregister: handler => Event -= handler,
            handler: eventHandler => args => eventHandler(args)
        );

    /// <summary>
    /// Raise the event for this observable.
    /// </summary>
    /// <param name="args">The args included with the event.</param>
    /// <returns>The running task.</returns>
    private Task EmitAsync(TArgs args) =>
        Event?.Invoke(args) ?? Task.CompletedTask;

    #endregion

    #region Events

    /// <inhertidoc/>
    public event Func<TArgs, Task>? Event;

    #endregion
}
