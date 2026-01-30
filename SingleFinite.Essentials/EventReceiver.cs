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

namespace SingleFinite.Essentials;

/// <summary>
/// A receiver of events from IEventProvider instances.
/// </summary>
public sealed class EventReceiver : IDisposable
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly DisposeState _disposeState;

    /// <summary>
    /// Actions to invoke when this object is disposed.
    /// </summary>
    private readonly List<Action> _onDisposeActions = [];

    /// <summary>
    /// The event function to invoke when one of the received events is raised.
    /// </summary>
    private readonly Func<object?, Task> _onEvent = _ => Task.CompletedTask;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    private EventReceiver()
    {
        _disposeState = new(
            owner: this,
            onDispose: () =>
            {
                _onDisposeActions.ForEach(disposeAction => disposeAction());
            }
        );
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="onEvent">
    /// The action to invoke when a received event is raised.
    /// </param>
    public EventReceiver(Action onEvent) : this() =>
        _onEvent = _ =>
        {
            onEvent();
            return Task.CompletedTask;
        };

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="onEvent">
    /// The action to invoke when a received event is raised.
    /// </param>
    public EventReceiver(Action<object?> onEvent) : this() =>
        _onEvent = args =>
        {
            onEvent(args);
            return Task.CompletedTask;
        };

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="onEvent">
    /// The action to invoke when a received event is raised.
    /// </param>
    public EventReceiver(Func<Task> onEvent) : this() =>
        _onEvent = async _ =>
        {
            await onEvent();
        };

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="onEvent">
    /// The action to invoke when a received event is raised.
    /// </param>
    public EventReceiver(Func<object?, Task> onEvent) : this() =>
        _onEvent = onEvent;

    #endregion

    #region Methods

    /// <summary>
    /// This method should be invoked by an IEventProvider when an event is
    /// raised.
    /// </summary>
    /// <param name="args">Optional arguments to include.</param>
    public void OnEvent(object? args = default)
    {
        _disposeState.ThrowIfDisposed();
        _onEvent(args);
    }

    /// <summary>
    /// This method should be invoked by an IEventProvider when an event is
    /// raised.
    /// </summary>
    /// <param name="args">Optional arguments to include.</param>
    public async Task OnEventAsync(object? args = default)
    {
        _disposeState.ThrowIfDisposed();
        await _onEvent(args);
    }

    /// <summary>
    /// Register an action to invoke when this object is disposed.
    /// </summary>
    /// <param name="action">The action to register.</param>
    public void OnDispose(Action action)
    {
        _disposeState.ThrowIfDisposed();
        _onDisposeActions.Add(action);
    }

    /// <inheritdoc/>
    public void Dispose() =>
        _disposeState.Dispose();

    #endregion
}
