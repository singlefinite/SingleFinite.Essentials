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
internal class AsyncObserverCombine : IAsyncObserver
{
    #region Fields

    /// <summary>
    /// Holds the observers that are combined.
    /// </summary>
    private readonly IEnumerable<IAsyncObserver> _observers;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="observers">The observers to combine.</param>
    public AsyncObserverCombine(IEnumerable<IAsyncObserver> observers)
    {
        _observers = observers;
        foreach (var observer in _observers)
            observer.OnEach(() => Next?.Invoke() ?? Task.CompletedTask);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Dispose of all of the observers that were combined.
    /// </summary>
    public void Dispose()
    {
        foreach (var observer in _observers)
            observer.Dispose();
    }

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised whenever any of the combined observers emit.
    /// </summary>
    public event Func<Task>? Next;

    #endregion
}

/// <summary>
/// An observer that combines multipe observers into a single observer.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
internal class AsyncObserverCombine<TArgs> : IAsyncObserver<TArgs>
{
    #region Fields

    /// <summary>
    /// Holds the observers that are combined.
    /// </summary>
    private readonly IEnumerable<IAsyncObserver<TArgs>> _observers;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="observers">The observers to combine.</param>
    public AsyncObserverCombine(IEnumerable<IAsyncObserver<TArgs>> observers)
    {
        _observers = observers;
        foreach (var observer in _observers)
            observer.OnEach(args => Next?.Invoke(args) ?? Task.CompletedTask);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Dispose of all of the observers that were combined.
    /// </summary>
    public void Dispose()
    {
        foreach (var observer in _observers)
            observer.Dispose();
    }

    #endregion

    #region Events

    /// <summary>
    /// Event that is raised whenever any of the combined observers emit.
    /// </summary>
    public event Func<TArgs, Task>? Next;

    #endregion
}
