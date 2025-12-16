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
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly ConcurrentDisposeState _disposeState;

    /// <summary>
    /// The event receiver used by this object.
    /// </summary>
    private readonly EventReceiver _eventReceiver;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="eventProviders">Event providers to combine.</param>
    public ObserverCombine(params IEnumerable<IEventProvider> eventProviders)
    {
        _eventReceiver = new(() => Next?.Invoke());

        foreach (var eventProvider in eventProviders)
            eventProvider.Provide(_eventReceiver);

        _disposeState = new(
            owner: this,
            onDispose: () =>
            {
                _eventReceiver.Dispose();
                foreach (var eventProvider in eventProviders)
                    (eventProvider as IDisposable)?.Dispose();
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

    /// <inheritdoc/>
    public void Dispose() => _disposeState.Dispose();

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised whenever any of the event providers raises an
    /// event.
    /// </summary>
    public event Action? Next;

    #endregion
}

/// <summary>
/// An observer that combines multipe observers into a single observer.
/// </summary>
/// <typeparam name="TArgs">
/// The type to cast event arguments to.  If an event provider doesn't have any
/// arguments or can't be cast to the given type this observer will emit with
/// null for the arguments.
/// </typeparam>
internal class Observer<TArgs> : IObserver<TArgs?>
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly ConcurrentDisposeState _disposeState;

    /// <summary>
    /// The event receiver used by this object.
    /// </summary>
    private readonly EventReceiver _eventReceiver;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="eventProviders">Event providers to combine.</param>
    public Observer(params IEnumerable<IEventProvider> eventProviders)
    {
        _eventReceiver = new(args =>
        {
            if (Next is not null)
            {
                if (args is not null && typeof(TArgs).IsAssignableFrom(args.GetType()))
                    Next((TArgs)args);
                else
                    Next(default);
            }
        });

        foreach (var eventProvider in eventProviders)
            eventProvider.Provide(_eventReceiver);

        _disposeState = new(
            owner: this,
            onDispose: () =>
            {
                _eventReceiver.Dispose();
                foreach (var eventProvider in eventProviders)
                    (eventProvider as IDisposable)?.Dispose();
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

    /// <inheritdoc/>
    public void Dispose() => _disposeState.Dispose();

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised whenever any of the event providers raises an
    /// event.
    /// </summary>
    public event Action<TArgs?>? Next;

    #endregion
}
