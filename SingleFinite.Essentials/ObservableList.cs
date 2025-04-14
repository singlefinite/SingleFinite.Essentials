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

using System;
using System.Collections;

namespace SingleFinite.Essentials;

/// <summary>
/// A list that is observable.
/// </summary>
/// <typeparam name="TItem">The type of item held by the list.</typeparam>
public class ObservableList<TItem> : IList<TItem>
{
    #region Fields

    /// <summary>
    /// The underlying list that holds the items.
    /// </summary>
    private readonly List<TItem> _list;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ObservableList()
    {
        _list = [];
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="items">
    /// The initial items to fill the list with.
    /// </param>
    public ObservableList(IEnumerable<TItem> items)
    {
        _list = [.. items];
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public TItem this[int index]
    {
        get => _list[index];
        set
        {
            RemoveAt(index);
            Insert(index, value);
        }
    }

    /// <inheritdoc/>
    public int Count => _list.Count;

    /// <summary>
    /// This will always be false.
    /// </summary>
    public bool IsReadOnly => false;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public bool Contains(TItem item) =>
        _list.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(TItem[] array, int arrayIndex) =>
        _list.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public int IndexOf(TItem item) =>
        _list.IndexOf(item);

    /// <inheritdoc/>
    public void Add(TItem item) =>
        Insert(_list.Count, item);

    /// <inheritdoc/>
    public void Insert(int index, TItem item)
    {
        _list.Insert(index, item);
        _changedSource.Emit(
            new(
                ChangeType: ListChangeType.Add,
                Items: [item],
                OldIndex: -1,
                NewIndex: index
            )
        );
    }

    /// <summary>
    /// Move an item within the list.  This is the same as calling the RemoveAt
    /// method followed by the Insert method but it will result in the Changed
    /// observable only being emitted once with a Move change type.
    /// </summary>
    /// <param name="oldIndex">
    /// The index of the item that is to be moved.
    /// </param>
    /// <param name="newIndex">
    /// The new index of the item that is to be moved.
    /// </param>
    public void Move(int oldIndex, int newIndex)
    {
        var item = _list[oldIndex];

        _list.RemoveAt(oldIndex);
        _list.Insert(newIndex, item);

        _changedSource.Emit(
            new(
                ChangeType: ListChangeType.Move,
                Items: [item],
                OldIndex: oldIndex,
                NewIndex: newIndex
            )
        );
    }

    /// <inheritdoc/>
    public bool Remove(TItem item)
    {
        var index = _list.IndexOf(item);
        if (index == -1)
            return false;

        RemoveAt(index);
        return true;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        var item = _list[index];
        _list.RemoveAt(index);
        _changedSource.Emit(
            new(
                ChangeType: ListChangeType.Remove,
                Items: [item],
                OldIndex: index,
                NewIndex: -1
            )
        );
    }

    /// <inheritdoc/>
    public void Clear()
    {
        var items = _list.ToArray();
        _list.Clear();
        _changedSource.Emit(
            new(
                ChangeType: ListChangeType.Remove,
                Items: items,
                OldIndex: 0,
                NewIndex: -1
            )
        );
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator() =>
        _list.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() =>
        _list.GetEnumerator();

    #endregion

    #region Events

    /// <summary>
    /// Observable that emits when the list has been changed.
    /// </summary>
    public Observable<ListChange<TItem>> Changed => _changedSource.Observable;
    private readonly ObservableSource<ListChange<TItem>> _changedSource = new();

    #endregion
}
