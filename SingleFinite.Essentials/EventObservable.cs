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
/// Implementation of <see cref="AsyncEventObservable"/>.
/// </summary>
public sealed class EventObservable : IEventObservable
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with <see cref="EventObservableSource"/> 
    /// objects.
    /// </summary>
    /// <param name="source">
    /// The source that raises the observable event.
    /// </param>
    internal EventObservable(EventObservableSource source)
    {
        source.Event += Emit;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public IEventObserver Observe() => new EventObserverEventSource<Action>(
        register: handler => Event += handler,
        unregister: handler => Event -= handler,
        handler: eventHandler => () => eventHandler()
    );

    /// <summary>
    /// Raise the event for this observable.
    /// </summary>
    private void Emit() => Event?.Invoke();

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
    public static IEventObserver Observe<TEventDelegate>(
        Action<TEventDelegate> register,
        Action<TEventDelegate> unregister,
        Func<Action, TEventDelegate> handler
    ) => new EventObserverEventSource<TEventDelegate>(
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
    public static IEventObserver<TArgs> Observe<TEventDelegate, TArgs>(
        Action<TEventDelegate> register,
        Action<TEventDelegate> unregister,
        Func<Action<TArgs>, TEventDelegate> handler
    ) => new EventObserverEventSource<TEventDelegate, TArgs>(
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
    public static IEventObserver Combine(
        params IEventObservable[] observables
    ) =>
        new EventObserverCombine(
            [.. observables.Select(observable => observable.Observe())]
        );

    /// <summary>
    /// Combine the given observables into a single observer.
    /// </summary>
    /// <typeparam name="TArgs">The type of args passed through.</typeparam>
    /// <param name="observables">The observables to combine.</param>
    /// <returns>
    /// A new observer that emits when any of the observers emit.
    /// </returns>
    public static IEventObserver<TArgs> Combine<TArgs>(
        params IEventObservable<TArgs>[] observables
    ) =>
        new EventObserverCombine<TArgs>(
            [.. observables.Select(observable => observable.Observe())]
        );

    #endregion

    #region Events

    /// <inheritdoc/>
    public event Action? Event;

    #endregion
}

/// <summary>
/// Implementation of <see cref="AsyncEventObservable"/>.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with the event.
/// </typeparam>
public sealed class EventObservable<TArgs> : IEventObservable<TArgs>
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with <see cref="EventObservableSource"/> 
    /// objects.
    /// </summary>
    /// <param name="source">
    /// The source that raises the observable event.
    /// </param>
    internal EventObservable(EventObservableSource<TArgs> source)
    {
        source.Event += Emit;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public IEventObserver<TArgs> Observe() =>
        new EventObserverEventSource<Action<TArgs>, TArgs>(
            register: handler => Event += handler,
            unregister: handler => Event -= handler,
            handler: eventHandler => args => eventHandler(args)
        );

    /// <inheritdoc/>
    IEventObserver IEventObservable.Observe() => new EventObserverEventSource<Action>(
        register: handler => EventWithoutArgs += handler,
        unregister: handler => EventWithoutArgs -= handler,
        handler: eventHandler => () => eventHandler()
    );

    /// <summary>
    /// Raise the event for this observable.
    /// </summary>
    /// <param name="args">The args included with the event.</param>
    private void Emit(TArgs args)
    {
        Event?.Invoke(args);
        EventWithoutArgs?.Invoke();
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public event Action<TArgs>? Event;

    /// <summary>
    /// Backing field for event without args.
    /// </summary>
    private event Action? EventWithoutArgs;

    /// <inheritdoc/>
    event Action? IEventObservable.Event
    {
        add => EventWithoutArgs += value;
        remove => EventWithoutArgs -= value;
    }

    #endregion
}
