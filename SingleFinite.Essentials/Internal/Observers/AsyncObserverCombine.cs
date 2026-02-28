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

namespace SingleFinite.Essentials.Internal.Observers;

/// <summary>
/// An observer that combines multipe observers into a single observer.
/// </summary>
internal class AsyncObserverCombine : IAsyncObserver
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly ConcurrentDisposeState _disposeState;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="observers">The observers to combine.</param>
    public AsyncObserverCombine(params IAsyncObserver[] observers)
    {
        _disposeState = new(owner: this);

        foreach (var observer in observers)
        {
            observer
                .OnEach(async () =>
                {
                    if (Next is not null)
                        await Next.Invoke();
                })
                .Until(_disposeState.CancellationToken);
        }
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsDisposed => _disposeState.IsDisposed;

    /// <inheritdoc/>
    public Observable Disposed => _disposeState.Disposed;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Dispose() => _disposeState.Dispose();

    #endregion

    #region Events

    /// <inheritdoc/>
    public event Func<Task>? Next;

    #endregion
}

/// <summary>
/// An observer that combines multipe observers into a single observer.
/// </summary>
/// <typeparam name="TArgs">The type of arguments for the combine.</typeparam>
internal class AsyncObserverCombine<TArgs> : IAsyncObserver<TArgs>
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly ConcurrentDisposeState _disposeState;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="observers">The observers to combine.</param>
    public AsyncObserverCombine(params IAsyncObserver<TArgs>[] observers)
    {
        _disposeState = new(owner: this);

        foreach (var observer in observers)
        {
            observer
                .OnEach(async args =>
                {
                    if (NextWithArgs is not null)
                        await NextWithArgs.Invoke(args);
                    if (Next is not null)
                        await Next.Invoke();
                })
                .Until(_disposeState.CancellationToken);
        }
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsDisposed => _disposeState.IsDisposed;

    /// <inheritdoc/>
    public Observable Disposed => _disposeState.Disposed;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Dispose() => _disposeState.Dispose();

    #endregion

    #region Events

    /// <inheritdoc/>
    public event Func<Task>? Next;

    /// <inheritdoc/>
    public event Func<TArgs, Task>? NextWithArgs;

    #endregion
}
