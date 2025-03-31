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
/// Emit to an observable when an event is raised on this observer.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
internal class AsyncObserverObservable(
    IAsyncObserver parent
) : AsyncObserverBase(parent)
{
    #region Methods

    /// <summary>
    /// Emit the event on the observable.
    /// </summary>
    /// <returns>Always returns false.</returns>
    protected override async Task<bool> OnEventAsync()
    {
        await _observableSource.EmitAsync();
        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// Observable that is raised when an event is recieved from the observer.
    /// </summary>
    public AsyncObservable Observable => _observableSource.Observable;
    private readonly AsyncObservableSource _observableSource = new();

    #endregion
}

/// <summary>
/// Emit to an observable when an event is raised on this observer.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
internal class AsyncObserverObservable<TArgs>(
    IAsyncObserver<TArgs> parent
) : AsyncObserverBase<TArgs>(parent)
{
    #region Methods

    /// <summary>
    /// Emit the event on the observable.
    /// </summary>
    /// <param name="args">
    /// Arguments passed with the observed event that are passed to the
    /// observable.
    /// </param>
    /// <returns>Always returns false.</returns>
    protected override async Task<bool> OnEventAsync(TArgs args)
    {
        await _observableSource.EmitAsync(args);
        return false;
    }

    #endregion

    #region Events

    /// <summary>
    /// Observable that is raised when an event is recieved from the observer.
    /// </summary>
    public AsyncObservable<TArgs> Observable => _observableSource.Observable;
    private readonly AsyncObservableSource<TArgs> _observableSource = new();

    #endregion
}
