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
/// Base class for common async observer class behavior.
/// </summary>
internal abstract class AsyncEventObserverBase : IAsyncEventObserver
{
    #region Fields

    /// <summary>
    /// Holds the parent to this observer.
    /// </summary>
    private readonly IAsyncEventObserver _parent;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    public AsyncEventObserverBase(IAsyncEventObserver parent)
    {
        _parent = parent;
        _parent.Next += async () =>
        {
            if (await OnEventAsync())
                await RaiseNextEventAsync();
        };
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsDisposed => _parent.IsDisposed;

    #endregion

    #region Methods

    /// <summary>
    /// This method is invoked when the parent Next event is raised.
    /// </summary>
    /// <returns>
    /// True if this observers Next event should be raised which would continue
    /// down the chain of observers. False if this observers Next event should
    /// not be raised which would stop the remaining chain of observers from
    /// seeing the event.
    /// </returns>
    protected abstract Task<bool> OnEventAsync();

    /// <summary>
    /// Raise the Next event for this observer which moves execution down the
    /// chain.
    /// </summary>
    /// <returns>A task that completes when the event handlers finish.</returns>
    protected Task RaiseNextEventAsync() => Next.TryInvoke();

    /// <summary>
    /// Invoke the parent Dispose method.  The expectation is that the dispose
    /// will eventually reach the first observer in the chain which will
    /// unregister the source event handler.
    /// </summary>
    public virtual void Dispose() => _parent.Dispose();

    #endregion

    #region Events

    /// <inheritdoc/>
    public IEventObservable Disposed => _parent.Disposed;

    /// <inheritdoc/>
    public virtual event Func<Task>? Next;

    #endregion
}

/// <summary>
/// Base class for observer classes.
/// </summary>
internal abstract class AsyncEventObserverBase<TArgs> : IAsyncEventObserver<TArgs>
{
    #region Fields

    /// <summary>
    /// Holds the parent to this observer.
    /// </summary>
    private readonly IAsyncEventObserver<TArgs> _parent;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    public AsyncEventObserverBase(IAsyncEventObserver<TArgs> parent)
    {
        _parent = parent;
        _parent.NextWithArgs += async args =>
        {
            if (await OnEventAsync(args))
                await RaiseNextEventAsync(args);
        };
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsDisposed => _parent.IsDisposed;

    #endregion

    #region Methods

    /// <summary>
    /// This method is invoked when the parent Next event is raised.
    /// </summary>
    /// <returns>
    /// True if this observers Next event should be raised which would continue
    /// down the chain of observers. False if this observers Next event should
    /// not be raised which would stop the remaining chain of observers from
    /// seeing the event.
    /// </returns>
    protected abstract Task<bool> OnEventAsync(TArgs args);

    /// <summary>
    /// Raise the Next event for this observer which moves execution down the
    /// chain.
    /// </summary>
    /// <param name="args">The args to pass with the event.</param>
    /// <returns>A task that completes when the event handlers finish.</returns>
    protected async Task RaiseNextEventAsync(TArgs args)
    {
        await NextWithArgs.TryInvoke(args);
        await Next.TryInvoke();
    }

    /// <summary>
    /// Invoke the parent Dispose method.  The expectation is that the dispose
    /// will eventually reach the first observer in the chain which will
    /// unregister the source event handler.
    /// </summary>
    public virtual void Dispose() => _parent.Dispose();

    #endregion

    #region Events

    /// <inheritdoc/>
    public IEventObservable Disposed => _parent.Disposed;

    /// <inheritdoc/>
    public virtual event Func<Task>? Next;

    /// <inheritdoc/>
    public virtual event Func<TArgs, Task>? NextWithArgs;

    #endregion
}
