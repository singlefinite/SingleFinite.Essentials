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
/// This class wraps an event to make registering and unregistering callbacks 
/// for the event more convenient when working with dependency injection scopes.
/// Observable instances are created by instances of the 
/// <see cref="ObservableSource"/> class.
/// </summary>
public sealed class Observable
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with <see cref="ObservableSource"/> 
    /// objects.
    /// </summary>
    /// <param name="source">
    /// The source that raises the observable event.
    /// </param>
    internal Observable(ObservableSource source)
    {
        source.Event += Emit;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create an observer for this observable.
    /// </summary>
    /// <returns>
    /// An observer that runs when the event for this observable is raised.
    /// </returns>
    public IObserver Observe() => new ObserverEventSource<Action>(
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
    /// of this observer is passed into the func.
    /// </param>
    /// <returns>An observer that observes from the event.</returns>
    public static IObserver Observe<TEventDelegate>(
        Action<TEventDelegate> register,
        Action<TEventDelegate> unregister,
        Func<Action, TEventDelegate> handler
    ) => new ObserverEventSource<TEventDelegate>(
        register,
        unregister,
        handler
    );

    /// <summary>
    /// Combine the given observers into a single observer.
    /// </summary>
    /// <param name="observers">The observers to combine.</param>
    /// <returns>
    /// A new observer that emits when any of the provided observers emit.
    /// </returns>
    public static IObserver Combine(params IEnumerable<IObserver> observers) =>
        new ObserverCombine(observers);

    /// <summary>
    /// Combine the given observables into a single observer.
    /// </summary>
    /// <param name="observables">The observables to combine.</param>
    /// <returns>
    /// A new observer that emits when any of the provided observables emit.
    /// </returns>
    public static IObserver Combine(
        params IEnumerable<Observable> observables
    ) =>
        new ObserverCombine(
            observables.Select(observable => observable.Observe())
        );

    /// <summary>
    /// Combine the given observers into a single observer.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with the event.
    /// </typeparam>
    /// <param name="observers">The observers to combine.</param>
    /// <returns>
    /// A new observer that emits when any of the provided observers emit.
    /// </returns>
    public static IObserver<TArgs> Combine<TArgs>(params IEnumerable<IObserver<TArgs>> observers) =>
        new ObserverCombine<TArgs>(observers);

    /// <summary>
    /// Combine the given observables into a single observer.
    /// </summary>
    /// <typeparam name="TArgs">
    /// The type of arguments passed with the event.
    /// </typeparam>
    /// <param name="observables">The observables to combine.</param>
    /// <returns>
    /// A new observer that emits when any of the provided observables emit.
    /// </returns>
    public static IObserver<TArgs> Combine<TArgs>(
        params IEnumerable<Observable<TArgs>> observables
    ) =>
        new ObserverCombine<TArgs>(
            observables.Select(observable => observable.Observe())
        );

    #endregion

    #region Events

    /// <summary>
    /// The underlying event.
    /// </summary>
    public event Action? Event;

    #endregion
}

/// <summary>
/// This class wraps an event to make registering and unregistering callbacks 
/// for the event more convenient when working with dependency injection scopes.
/// Observable instances are created by instances of the 
/// <see cref="ObservableSource{TArgs}"/> class.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with the event.
/// </typeparam>
public sealed class Observable<TArgs>
{
    #region Constructors

    /// <summary>
    /// Instances of this class are created with <see cref="ObservableSource"/> 
    /// objects.
    /// </summary>
    /// <param name="source">
    /// The source that raises the observable event.
    /// </param>
    internal Observable(ObservableSource<TArgs> source)
    {
        source.Event += Emit;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create an observer for this observable.
    /// </summary>
    /// <returns>
    /// An observer that runs when the event for this observable is raised.
    /// </returns>
    public IObserver<TArgs> Observe() =>
        new ObserverEventSource<Action<TArgs>, TArgs>(
            register: handler => Event += handler,
            unregister: handler => Event -= handler,
            handler: eventHandler => args => eventHandler(args)
        );

    /// <summary>
    /// Raise the event for this observable.
    /// </summary>
    /// <param name="args">The args included with the event.</param>
    private void Emit(TArgs args) => Event?.Invoke(args);

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
    /// of this observer is passed into the func.
    /// </param>
    /// <returns>An observer that observes from the event.</returns>
    public static IObserver<TArgs> Observe<TEventDelegate>(
        Action<TEventDelegate> register,
        Action<TEventDelegate> unregister,
        Func<Action<TArgs>, TEventDelegate> handler
    ) => new ObserverEventSource<TEventDelegate, TArgs>(
        register,
        unregister,
        handler
    );

    #endregion

    #region Events

    /// <summary>
    /// The underlying event.
    /// </summary>
    public event Action<TArgs>? Event;

    #endregion
}
