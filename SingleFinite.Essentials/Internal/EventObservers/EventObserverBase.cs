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

namespace SingleFinite.Essentials.Internal.EventObservers;

/// <summary>
/// Base class for common observer class behavior.
/// </summary>
internal abstract class EventObserverBase : IEventObserver
{
    #region Fields

    /// <summary>
    /// Holds the parent to this observer.
    /// </summary>
    private readonly IEventObserver _parent;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    public EventObserverBase(IEventObserver parent)
    {
        _parent = parent;
        _parent.Next += OnParentNext;
        _parent.Disposed += OnParentDisposed;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle the Next event from parent.
    /// </summary>
    private void OnParentNext()
    {
        if (OnEvent())
            RaiseNextEvent();
    }

    /// <summary>
    /// Handle the Disposed event from parent.
    /// </summary>
    protected void OnParentDisposed()
    {
        _parent.Next -= OnParentNext;
        _parent.Disposed -= OnParentDisposed;

        OnDispose();

        Disposed?.Invoke();
    }

    /// <summary>
    /// This method is invoked when the parent event is raised.
    /// </summary>
    /// <returns>
    /// True if this observers event should be raised which would continue down
    /// the chain of observers. False if this observers event should not be
    /// raised which would stop the remaining chain of observers from seeing the
    /// event.
    /// </returns>
    protected abstract bool OnEvent();

    /// <summary>
    /// Raise the Event for this observer which moves execution down the chain.
    /// </summary>
    protected void RaiseNextEvent() => Next?.Invoke();

    /// <summary>
    /// Invoke the parent Dispose method.  The expectation is that the dispose
    /// will eventually reach the first observer in the chain which will
    /// unregister the source event handler.
    /// </summary>
    public virtual void Dispose() => _parent.Dispose();

    /// <summary>
    /// Called when this observer is disposed.
    /// </summary>
    protected virtual void OnDispose()
    {
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public virtual event Action? Next;

    /// <inheritdoc/>
    public event Action? Disposed;

    #endregion
}

/// <summary>
/// Base class for observer classes.
/// </summary>
internal abstract class EventObserverBase<TArgs> : IEventObserver<TArgs>
{
    #region Fields

    /// <summary>
    /// Holds the parent to this observer.
    /// </summary>
    private readonly IEventObserver<TArgs> _parent;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    public EventObserverBase(IEventObserver<TArgs> parent)
    {
        _parent = parent;
        _parent.NextWithArgs += OnParentNext;
        _parent.Disposed += OnParentDisposed;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle the Next event from parent.
    /// </summary>
    /// <param name="args">The arguments passed with the event.</param>
    private void OnParentNext(TArgs args)
    {
        if (OnEvent(args))
            RaiseNextEvent(args);
    }

    /// <summary>
    /// Handle the Disposed event from parent.
    /// </summary>
    protected void OnParentDisposed()
    {
        _parent.NextWithArgs -= OnParentNext;
        _parent.Disposed -= OnParentDisposed;

        OnDispose();

        Disposed?.Invoke();
    }

    /// <summary>
    /// This method is invoked when the parent event is raised.
    /// </summary>
    /// <returns>
    /// True if this observers event should be raised which would continue down
    /// the chain of observers. False if this observers event should not be
    /// raised which would stop the remaining chain of observers from seeing the
    /// event.
    /// </returns>
    protected abstract bool OnEvent(TArgs args);

    /// <summary>
    /// Raise the Event for this observer which moves execution down the chain.
    /// </summary>
    /// <param name="args">The args to pass with the event.</param>
    protected void RaiseNextEvent(TArgs args)
    {
        NextWithArgs?.Invoke(args);
        Next?.Invoke();
    }

    /// <summary>
    /// Invoke the parent Dispose method.  The expectation is that the dispose
    /// will eventually reach the first observer in the chain which will
    /// unregister the source event handler.
    /// </summary>
    public virtual void Dispose() => _parent.Dispose();

    /// <summary>
    /// Called when this observer is disposed.
    /// </summary>
    protected virtual void OnDispose()
    {
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public virtual event Action? Next;

    /// <inheritdoc/>
    public virtual event Action<TArgs>? NextWithArgs;

    /// <inheritdoc/>
    public event Action? Disposed;

    #endregion
}
