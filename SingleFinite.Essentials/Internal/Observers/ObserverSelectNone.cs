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
/// This class is used to turn an observer that has an argument into an observer
/// that doesn't have an argument.
/// </summary>
/// <typeparam name="TArgs">The type of argument being observed.</typeparam>
internal class ObserverSelectNone<TArgs> : IObserver
{
    #region Fields

    /// <summary>
    /// Holds the parent to this observer.
    /// </summary>
    private readonly IObserver<TArgs> _parent;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parent">The parent to this observer.</param>
    public ObserverSelectNone(IObserver<TArgs> parent)
    {
        _parent = parent;
        parent.Next += _ => Next?.Invoke();
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsDisposed => _parent.IsDisposed;

    /// <inheritdoc/>
    public Observable Disposed => _parent.Disposed;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Dispose() =>
        _parent.Dispose();

    #endregion

    #region Events

    /// <inheritdoc/>
    public event Action? Next;

    #endregion
}
