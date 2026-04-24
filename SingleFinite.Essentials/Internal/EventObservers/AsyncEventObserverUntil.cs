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
/// An observer that will observe events until the passed in condition is met.
/// </summary>
internal class AsyncEventObserverUntil : AsyncEventObserverBase
{
    #region Fields

    /// <summary>
    /// The condition that when met will dispose of the oberver chain.
    /// </summary>
    private readonly Func<Task<bool>>? _predicate;

    /// <summary>
    /// The registration for the cancellation token.
    /// </summary>
    private readonly CancellationTokenRegistration? _cancellationTokenRegistration;

    /// <summary>
    /// The observer that when emits will dispose of this observer.
    /// </summary>
    private readonly IEventObserver? _disposeObserver;

    /// <summary>
    /// The observer that when emits will dispose of this observer.
    /// </summary>
    private readonly IAsyncEventObserver? _asyncDisposeObserver;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="predicate">
    /// The condition that when met will dispose of the oberver chain.
    /// </param>
    public AsyncEventObserverUntil(
        IAsyncEventObserver parent,
        Func<Task<bool>> predicate
    ) : base(parent)
    {
        _predicate = predicate;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="cancellationToken">
    /// The cancellation token that when cancelled will dispose of this
    /// observer.
    /// </param>
    public AsyncEventObserverUntil(
        IAsyncEventObserver parent,
        CancellationToken cancellationToken
    ) : base(parent)
    {
        _cancellationTokenRegistration = cancellationToken.Register(Dispose);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="disposeObserver">
    /// The observer that when emits will dispose of this observer.
    /// </param>
    public AsyncEventObserverUntil(
        IAsyncEventObserver parent,
        IEventObserver disposeObserver
    ) : base(parent)
    {
        _disposeObserver = disposeObserver.OnEach(Dispose);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="disposeObserver">
    /// The observer that when emits will dispose of this observer.
    /// </param>
    public AsyncEventObserverUntil(
        IAsyncEventObserver parent,
        IAsyncEventObserver disposeObserver
    ) : base(parent)
    {
        _asyncDisposeObserver = disposeObserver.OnEach(Dispose);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Evaluate the dispose condition and dispose if the condition is met.
    /// </summary>
    /// <returns>
    /// True if the dispose condition is not met, false if it is.  If there is
    /// no dispose condition this will always return true.
    /// </returns>
    protected override async Task<bool> OnEventAsync()
    {
        var willDispose = false;
        if (_predicate is not null)
            willDispose = await _predicate();

        if (willDispose == true)
            Dispose();

        return willDispose != true;
    }

    /// <summary>
    /// Clean up this object.
    /// </summary>
    protected override void OnDispose()
    {
        _cancellationTokenRegistration?.Unregister();
        _disposeObserver?.Dispose();
        _asyncDisposeObserver?.Dispose();
    }

    #endregion
}

/// <summary>
/// An observer that will observe events until the passed in condition is met.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
internal class AsyncEventObserverUntil<TArgs> : AsyncEventObserverBase<TArgs>
{
    #region Fields

    /// <summary>
    /// The condition that when met will dispose of the oberver chain.
    /// </summary>
    private readonly Func<TArgs, Task<bool>>? _predicate;

    /// <summary>
    /// The registration for the cancellation token.
    /// </summary>
    private readonly CancellationTokenRegistration? _cancellationTokenRegistration;

    /// <summary>
    /// The observer that when emits will dispose of this observer.
    /// </summary>
    private readonly IEventObserver? _disposeObserver;

    /// <summary>
    /// The observer that when emits will dispose of this observer.
    /// </summary>
    private readonly IAsyncEventObserver? _asyncDisposeObserver;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="predicate">
    /// The condition that when met will dispose of the oberver chain.
    /// </param>
    public AsyncEventObserverUntil(
        IAsyncEventObserver<TArgs> parent,
        Func<TArgs, Task<bool>> predicate
    ) : base(parent)
    {
        _predicate = predicate;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="cancellationToken">
    /// The cancellation token that when cancelled will dispose of this
    /// observer.
    /// </param>
    public AsyncEventObserverUntil(
        IAsyncEventObserver<TArgs> parent,
        CancellationToken cancellationToken
    ) : base(parent)
    {
        _cancellationTokenRegistration = cancellationToken.Register(() =>
        {
            Dispose();
            _cancellationTokenRegistration?.Dispose();
        });
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="disposeObserver">
    /// The observer that when emits will dispose of this observer.
    /// </param>
    public AsyncEventObserverUntil(
        IAsyncEventObserver<TArgs> parent,
        IEventObserver disposeObserver
    ) : base(parent)
    {
        _disposeObserver = disposeObserver.OnEach(Dispose);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    /// <param name="disposeObserver">
    /// The observer that when emits will dispose of this observer.
    /// </param>
    public AsyncEventObserverUntil(
        IAsyncEventObserver<TArgs> parent,
        IAsyncEventObserver disposeObserver
    ) : base(parent)
    {
        _asyncDisposeObserver = disposeObserver.OnEach(Dispose);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Evaluate the dispose condition and dispose if the condition is met.
    /// </summary>
    /// <param name="args">Args passed to the predicate.</param>
    /// <returns>
    /// True if the dispose condition is not met, false if it is.  If there is
    /// no dispose condition this will always return true.
    /// </returns>
    protected override async Task<bool> OnEventAsync(TArgs args)
    {
        var willDispose = false;
        if (_predicate is not null)
            willDispose = await _predicate(args);

        if (willDispose == true)
            Dispose();

        return willDispose != true;
    }

    /// <summary>
    /// Clean up this object.
    /// </summary>
    protected override void OnDispose()
    {
        _cancellationTokenRegistration?.Unregister();
        _disposeObserver?.Dispose();
        _asyncDisposeObserver?.Dispose();
    }

    #endregion
}
