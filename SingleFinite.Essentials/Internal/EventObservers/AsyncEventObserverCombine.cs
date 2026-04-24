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
/// An observer that combines multipe observers into a single observer.
/// </summary>
internal class AsyncEventObserverCombine : IAsyncEventObserver
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly ConcurrentDisposeState _disposeState;

    /// <summary>
    /// Holds the original observers passed in.
    /// </summary>
    private readonly IAsyncEventObserver[] _observers;

    /// <summary>
    /// Holds the combined observers.
    /// </summary>
    private readonly IAsyncEventObserver[] _combinedObservers;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="observers">The observers to combine.</param>
    public AsyncEventObserverCombine(params IAsyncEventObserver[] observers)
    {
        _disposeState = new(
            owner: this,
            onDispose: OnDispose
        );

        _observers = observers;
        _combinedObservers = [.. observers.Select(observer =>
            observer.OnEach(() => Next.TryInvoke())
        )];

        foreach (var observer in observers)
            observer.Disposed += Dispose;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Dispose() => _disposeState.Dispose();

    /// <summary>
    /// Clean up this object.
    /// </summary>
    private void OnDispose()
    {
        foreach (var observer in _combinedObservers)
            observer.Dispose();

        foreach (var observer in _observers)
            observer.Disposed -= Dispose;

        Disposed?.Invoke();
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public event Func<Task>? Next;

    /// <inheritdoc/>
    public event Action? Disposed;

    #endregion
}

/// <summary>
/// An observer that combines multipe observers into a single observer.
/// </summary>
/// <typeparam name="TArgs">The type of arguments for the combine.</typeparam>
internal class AsyncEventObserverCombine<TArgs> : IAsyncEventObserver<TArgs>
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly ConcurrentDisposeState _disposeState;

    /// <summary>
    /// Holds the original observers passed in.
    /// </summary>
    private readonly IAsyncEventObserver[] _observers;

    /// <summary>
    /// Holds the combined observers.
    /// </summary>
    private readonly IAsyncEventObserver[] _combinedObservers;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="observers">The observers to combine.</param>
    public AsyncEventObserverCombine(params IAsyncEventObserver<TArgs>[] observers)
    {
        _disposeState = new(
            owner: this,
            onDispose: OnDispose
        );

        _observers = observers;
        _combinedObservers = [.. observers.Select(observer =>
            observer.OnEach(async args =>
            {
                await NextWithArgs.TryInvoke(args);
                await Next.TryInvoke();
            })
        )];

        foreach (var observer in observers)
            observer.Disposed += Dispose;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Dispose() => _disposeState.Dispose();

    /// <summary>
    /// Clean up this object.
    /// </summary>
    private void OnDispose()
    {
        foreach (var observer in _combinedObservers)
            observer.Dispose();

        foreach (var observer in _observers)
            observer.Disposed -= Dispose;

        Disposed?.Invoke();
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public event Func<Task>? Next;

    /// <inheritdoc/>
    public event Func<TArgs, Task>? NextWithArgs;

    /// <inheritdoc/>
    public event Action? Disposed;

    #endregion
}
