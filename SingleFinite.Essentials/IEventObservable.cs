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
/// Wraps an event to make registering and unregistering callbacks 
/// for the event more convenient when working with dependency injection scopes.
/// Observable instances are created by instances of the 
/// <see cref="EventObservableSource"/> class.
/// </summary>
public interface IEventObservable
{
    /// <summary>
    /// Create an observer for this observable.
    /// </summary>
    /// <returns>
    /// An observer that runs when the event for this observable is raised.
    /// </returns>
    IEventObserver Observe();

    /// <summary>
    /// The underlying event.
    /// </summary>
    event Action? Event;
}

/// <summary>
/// Wraps an event to make registering and unregistering callbacks 
/// for the event more convenient when working with dependency injection scopes.
/// Observable instances are created by instances of the 
/// <see cref="EventObservableSource{TArgs}"/> class.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with the event.
/// </typeparam>
public interface IEventObservable<out TArgs> : IEventObservable
{
    /// <summary>
    /// Create an observer for this observable.
    /// </summary>
    /// <returns>
    /// An observer that runs when the event for this observable is raised.
    /// </returns>
    new IEventObserver<TArgs> Observe();

    /// <summary>
    /// The underlying event.
    /// </summary>
    new event Action<TArgs>? Event;
}
