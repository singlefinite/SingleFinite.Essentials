// MIT License
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
/// An observer that combines multipe observers into a single observer.
/// </summary>
internal class ObserverCombine : IObserver
{
    #region Fields

    /// <summary>
    /// Holds the dispose state.
    /// </summary>
    private readonly ConcurrentDisposeState _disposeState;

    /// <summary>
    /// Holds the observers that are combined.
    /// </summary>
    private readonly IEnumerable<IObserver> _observers;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="observers">The observers to combine.</param>
    public ObserverCombine(IEnumerable<IObserver> observers)
    {
        _observers = observers;
        foreach (var observer in _observers)
            observer.OnEach(() => Next?.Invoke());

        _disposeState = new(
            owner: this,
            onDispose: () =>
            {
                foreach (var observer in _observers)
                    observer.Dispose();
            }
        );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsDisposed => _disposeState.IsDisposed;

    /// <inheritdoc/>
    public Observable Disposed => _disposeState.Disposed;

    #endregion

    #region Methods

    /// <summary>
    /// Dispose of all of the observers that were combined.
    /// </summary>
    public void Dispose() => _disposeState.Dispose();

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised whenever any of the combined observers emit.
    /// </summary>
    public event Action? Next;

    #endregion
}

/// <summary>
/// An observer that combines multipe observers into a single observer.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
internal class ObserverCombine<TArgs> : IObserver<TArgs>
{
    #region Fields

    /// <summary>
    /// Holds the dispose state.
    /// </summary>
    private readonly DisposeState _disposeState;

    /// <summary>
    /// Holds the observers that are combined.
    /// </summary>
    private readonly IEnumerable<IObserver<TArgs>> _observers;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="observers">The observers to combine.</param>
    public ObserverCombine(IEnumerable<IObserver<TArgs>> observers)
    {
        _observers = observers;
        foreach (var observer in _observers)
            observer.OnEach(args => Next?.Invoke(args));

        _disposeState = new(
            owner: this,
            onDispose: () =>
            {
                foreach (var observer in _observers)
                    observer.Dispose();
            }
        );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsDisposed => _disposeState.IsDisposed;

    /// <inheritdoc/>
    public Observable Disposed => _disposeState.Disposed;

    #endregion

    #region Methods

    /// <summary>
    /// Dispose of all of the observers that were combined.
    /// </summary>
    public void Dispose() => _disposeState.Dispose();

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised whenever any of the combined observers emit.
    /// </summary>
    public event Action<TArgs>? Next;

    #endregion
}
