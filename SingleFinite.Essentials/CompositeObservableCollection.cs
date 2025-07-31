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

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SingleFinite.Essentials;

/// <summary>
/// An observable collection that is a composite of multiple observable
/// collection instances.
/// </summary>
/// <typeparam name="TItem">
/// The type of items held by this collection.
/// </typeparam>
/// <typeparam name="TCollection">
/// The type of collections held by this collection.
/// </typeparam>
public class CompositeObservableCollection<TItem, TCollection> :
    IReadOnlyCollection<TItem>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    where TCollection : IEnumerable<TItem>, INotifyCollectionChanged
{
    #region Fields

    /// <summary>
    /// Holds the collections that make up the composite collection.
    /// </summary>
    private readonly List<TCollection> _collections = [];

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="collections">
    /// The collections that will be composited into this collection.
    /// </param>
    public CompositeObservableCollection(params IEnumerable<TCollection> collections)
    {
        _collections = [.. collections];
        foreach (var collection in _collections)
            collection.CollectionChanged += OnCollectionChanged;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The collections that make up the composite collection.
    /// </summary>
    public IReadOnlyList<TCollection> Collections => _collections;

    #endregion

    #region Properties

    /// <inheritdoc/>
    public int Count => _collections.Sum(collection => collection.Count());

    #endregion

    #region Methods

    /// <summary>
    /// Remove event handlers from all collections.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (var collection in _collections)
            collection.CollectionChanged -= OnCollectionChanged;
    }

    /// <summary>
    /// Pass on the collection changed event from the collection.
    /// </summary>
    /// <param name="sender">The collection that had the change.</param>
    /// <param name="e">The arguments for the change.</param>
    private void OnCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e
    )
    {
        if (sender is not TCollection collection)
            return;

        var adjustedIndex = GetStartingItemIndex(collection);
        if (adjustedIndex < 0)
            return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                CollectionChanged?.Invoke(
                    sender: this,
                    e: new(
                        action: e.Action,
                        changedItems: e.NewItems,
                        startingIndex: e.NewStartingIndex + adjustedIndex
                    )
                );
                PropertyChanged?.Invoke(this, EventArgsCache.CountPropertyChanged);
                break;

            case NotifyCollectionChangedAction.Remove:
                CollectionChanged?.Invoke(
                    sender: this,
                    e: new(
                        action: e.Action,
                        changedItems: e.OldItems,
                        startingIndex: e.OldStartingIndex + adjustedIndex
                    )
                );
                PropertyChanged?.Invoke(this, EventArgsCache.CountPropertyChanged);
                break;

            case NotifyCollectionChangedAction.Move:
                CollectionChanged?.Invoke(
                    sender: this,
                    e: new(
                        action: e.Action,
                        changedItems: e.NewItems,
                        index: e.NewStartingIndex + adjustedIndex,
                        oldIndex: e.OldStartingIndex + adjustedIndex
                    )
                );
                break;

            case NotifyCollectionChangedAction.Replace:
                CollectionChanged?.Invoke(
                    sender: this,
                    e: new(
                        action: e.Action,
                        newItems: e.NewItems ?? new List<TItem>(),
                        oldItems: e.OldItems ?? new List<TItem>(),
                        startingIndex: e.NewStartingIndex + adjustedIndex
                    )
                );
                break;

            case NotifyCollectionChangedAction.Reset:
                CollectionChanged?.Invoke(
                    sender: this,
                    e: EventArgsCache.ResetEventArgs
                );
                PropertyChanged?.Invoke(this, EventArgsCache.CountPropertyChanged);
                break;
        }
    }

    /// <summary>
    /// Get the starting index for items for the given collection.
    /// </summary>
    /// <param name="collection">
    /// The collection to get the starting items index for.
    /// </param>
    /// <returns>
    /// The starting index for items for the given collection.
    /// If the collection is not found, -1 is returned.
    /// </returns>
    private int GetStartingItemIndex(TCollection collection)
    {
        var index = 0;

        foreach (var collectionEntry in _collections)
        {
            if (Object.Equals(collectionEntry, collection))
                return index;

            index += collectionEntry.Count();
        }

        return -1;
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator() =>
        new CompositeEnumerator<TItem>(_collections.Select(l => l.GetEnumerator()));

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region Events

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Types

    /// <summary>
    /// Static event argument classes that can be reused for improved
    /// performance.
    /// </summary>
    private static class EventArgsCache
    {
        public static readonly PropertyChangedEventArgs CountPropertyChanged =
            new("Count");

        public static readonly NotifyCollectionChangedEventArgs ResetEventArgs =
            new(NotifyCollectionChangedAction.Reset);
    }

    #endregion
}
