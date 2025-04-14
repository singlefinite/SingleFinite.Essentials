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
/// Monitor observers by owner.  When an owner is removed from this object
/// all the observers that are monitored by this object that belong to the
/// owner will be disposed.
/// </summary>
public sealed class ObserverMonitor
{
    #region Fields

    /// <summary>
    /// Holds the list of observers by owner.
    /// </summary>
    private readonly Dictionary<object, List<IDisposable>> _observerDictionary = [];

    #endregion

    #region Methods

    /// <summary>
    /// Add an observer to monitor for the given owner.
    /// </summary>
    /// <param name="owner">The owner of the observer.</param>
    /// <param name="observers">The observers to monitor.</param>
    public void Add(object owner, params IEnumerable<IDisposable> observers)
    {
        if (_observerDictionary.TryGetValue(owner, out var observerList))
            observerList.AddRange(observers);
        else
            _observerDictionary.Add(owner, [.. observers]);
    }

    /// <summary>
    /// Remove the owner from this object and dispose of all observers that
    /// belong to the given owner.
    /// </summary>
    /// <param name="owner">The owner to remove from the monitor.</param>
    public void Remove(object owner)
    {
        if (_observerDictionary.Remove(owner, out var observers))
        {
            foreach (var observer in observers)
                observer.Dispose();
        }
    }

    /// <summary>
    /// Remove all owners from this object and dispose of all observers that
    /// were being monitored.
    /// </summary>
    public void Clear()
    {
        var allObservers = _observerDictionary.Values
            .SelectMany(observers => observers)
            .ToList();

        _observerDictionary.Clear();

        foreach (var observer in allObservers)
            observer.Dispose();
    }

    #endregion
}
