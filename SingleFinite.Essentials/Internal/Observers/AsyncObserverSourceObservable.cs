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

namespace SingleFinite.Essentials.Internal.Observers;

/// <summary>
/// An observer that is the root of an observer chain and has an observable
/// for a source.
/// </summary>
internal class AsyncObserverSourceObservable : IAsyncObserver
{
    #region Fields

    /// <summary>
    /// Set to true when this object has been disposed.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// An observable that is the source of events for the observer.
    /// </summary>
    private readonly AsyncObservable _observable;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="observable">
    /// An observable that is the source of events for the observer.
    /// </param>
    public AsyncObserverSourceObservable(AsyncObservable observable)
    {
        _observable = observable;
        _observable.Event += OnEventAsync;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raise the Event.
    /// </summary>
    private Task OnEventAsync() => Next?.Invoke() ?? Task.CompletedTask;

    /// <summary>
    /// Unsubscribe from the observable events.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _observable.Event -= OnEventAsync;
    }

    #endregion

    #region Events

    /// <summary>
    /// Raised when the observable event is raised. 
    /// </summary>
    public event Func<Task>? Next;

    #endregion
}

/// <summary>
/// An observer that is the root of an observer chain and has an observable
/// for a source.
/// </summary>
internal class AsyncObserverSource<TArgs> : IAsyncObserver<TArgs>
{
    #region Fields

    /// <summary>
    /// Set to true when this object has been disposed.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// An observable that is the source of events for the observer.
    /// </summary>
    private readonly AsyncObservable<TArgs> _observable;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="observable">
    /// An observable that is the source of events for the observer.
    /// </param>
    public AsyncObserverSource(AsyncObservable<TArgs> observable)
    {
        _observable = observable;
        _observable.Event += OnEventAsync;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raise the Event.
    /// </summary>
    private Task OnEventAsync(TArgs args) =>
        Next?.Invoke(args) ?? Task.CompletedTask;

    /// <summary>
    /// Unsubscribe from the observable events.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _observable.Event -= OnEventAsync;
    }

    #endregion

    #region Events

    /// <summary>
    /// Raised when the observable event is raised. 
    /// </summary>
    public event Func<TArgs, Task>? Next;

    #endregion
}
