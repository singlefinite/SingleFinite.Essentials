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

namespace SingleFinite.Essentials;

/// <summary>
/// The source for an <see cref="AsyncObservable"/>.  Instances of this class
/// can be kept private within its owner while the observable is shared publicly 
/// outside of the owner.  Since events are only raised through this class it 
/// prevents events from being raised outside of the owning class.
/// </summary>
public sealed class AsyncObservableSource
{
    #region Properties

    /// <summary>
    /// The observable provided by this class.
    /// </summary>
    public AsyncObservable Observable { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public AsyncObservableSource()
    {
        Observable = new(this);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raise the event for the observable.
    /// </summary>
    /// <returns>The running task.</returns>
    public Task EmitAsync() => Event?.Invoke() ?? Task.CompletedTask;

    /// <summary>
    /// Raise the event and return immediately without waiting for the event
    /// to complete.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to raise the event on.</param>
    /// <param name="onError">
    /// Optional action that will be invoked if the event generates an
    /// exception.
    /// </param>
    public void EmitEvent(
        IDispatcher dispatcher,
        Action<Exception>? onError = default
    ) => dispatcher.Run(
        func: EmitAsync,
        onError: onError
    );

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised when the RaiseEvent method is invoked.
    /// </summary>
    internal event Func<Task>? Event;

    #endregion
}

/// <summary>
/// The source for an <see cref="AsyncObservableSource{TArgs}"/>.  Instances of
/// this class can be kept private within its owner while the observable is
/// shared publicly outside of the owner.  Since events are only raised through
/// this class it prevents events from being raised outside of the owning class.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with the event.
/// </typeparam>
public sealed class AsyncObservableSource<TArgs>
{
    #region Properties

    /// <summary>
    /// The observable provided by this class.
    /// </summary>
    public AsyncObservable<TArgs> Observable { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public AsyncObservableSource()
    {
        Observable = new(this);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raise the event for the observable.
    /// </summary>
    /// <param name="args">The arguments to pass with the event.</param>
    /// <returns>The running task.</returns>
    public Task EmitAsync(TArgs args) =>
        Event?.Invoke(args) ?? Task.CompletedTask;

    /// <summary>
    /// Raise the event and return immediately without waiting for the event
    /// to complete.
    /// </summary>
    /// <param name="args">The arguments to pass with the event.</param>
    /// <param name="dispatcher">The dispatcher to raise the event on.</param>
    /// <param name="onError">
    /// Optional action that will be invoked if the event generates an
    /// exception.
    /// </param>
    public void Emit(
        TArgs args,
        IDispatcher dispatcher,
        Action<Exception>? onError = default
    ) => dispatcher.Run(
        func: () => EmitAsync(args),
        onError: onError
    );

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised when the RaiseEvent method is invoked.
    /// </summary>
    internal event Func<TArgs, Task>? Event;

    #endregion
}
