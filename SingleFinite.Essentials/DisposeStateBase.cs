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

namespace SingleFinite.Essentials;

/// <summary>
/// Base class for DisposeState classes.
/// </summary>
public abstract class DisposeStateBase : IDisposable
{
    #region Fields

    /// <summary>
    /// The object whose dispose state is being managed by this object.
    /// </summary>
    private readonly object _owner;

    /// <summary>
    /// Optional action to invoke when this object is disposed.
    /// </summary>
    private readonly Action? _onDispose;

    /// <summary>
    /// Source for Disposed observable.
    /// </summary>
    private readonly ObservableSource _disposedSource = new();

    /// <summary>
    /// Source for CancellationToken.
    /// </summary>
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    #endregion

    #region Constructors

    /// <param name="owner">
    /// The object whose dispose state is being managed by this object.
    /// </param>
    /// <param name="onDispose">
    /// Optional action to invoke when this object is disposed.
    /// </param>
    internal DisposeStateBase(
        object owner,
        Action? onDispose = default
    )
    {
        _owner = owner;
        _onDispose = onDispose;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if this object has been disposed.
    /// </summary>
    public bool IsDisposed { get; protected set; }

    /// <summary>
    /// Observable that will notify observers when this object is disposed.
    /// </summary>
    public Observable Disposed =>
        _disposedSource.Observable;

    /// <summary>
    /// A token that is canceled when this object is disposed.
    /// </summary>
    public CancellationToken CancellationToken =>
        _cancellationTokenSource.Token;

    #endregion

    #region Methods

    /// <summary>
    /// Called when the Dispose is called.  This method should set IsDisposed
    /// to true and return true only if IsDisposed was changed from false to
    /// true.
    /// </summary>
    /// <returns>true if IsDisposed was changed from false to true.</returns>
    protected abstract bool StartDispose();

    /// <summary>
    /// Dispose of this object.
    /// </summary>
    public void Dispose()
    {
        if (StartDispose())
        {
            GC.SuppressFinalize(this);
            OnDispose();
        }
    }

    /// <summary>
    /// Called when this object has been disposed.
    /// </summary>
    private void OnDispose()
    {
        _onDispose?.Invoke();
        _disposedSource.Emit();
        _cancellationTokenSource.Cancel();
    }

    /// <summary>
    /// Throw an exception if this object has been disposed.
    /// </summary>
    public void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            condition: IsDisposed,
            instance: _owner
        );
    }

    #endregion
}
