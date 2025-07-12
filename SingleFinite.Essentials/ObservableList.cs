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

namespace SingleFinite.Essentials;

/// <summary>
/// Implementation of <see cref="IObservableList{TItem}"/>."/>
/// </summary>
/// <typeparam name="TItem">The type of item held by the list.</typeparam>
public class ObservableList<TItem> : IObservableList<TItem>, IList, IReadOnlyList<TItem>
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

    /// <inheritdoc/>
    bool IList.IsReadOnly => ((IList)_list).IsReadOnly;

    /// <inheritdoc/>
    bool IList.IsFixedSize => ((IList)_list).IsFixedSize;

    /// <inheritdoc/>
    object? IList.this[int index]
    {
        get => ((IList)_list)[index];
        set => ((IList)_list)[index] = value;
    }

    /// <inheritdoc/>
    bool ICollection<TItem>.IsReadOnly => ((ICollection<TItem>)_list).IsReadOnly;

    /// <inheritdoc/>
    bool ICollection.IsSynchronized => ((ICollection)_list).IsSynchronized;

    /// <inheritdoc/>
    object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;

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
    public void AddRange(IEnumerable<TItem> items) =>
        InsertRange(_list.Count, items);

    /// <inheritdoc/>
    public void Insert(int index, TItem item) =>
        InsertRange(index, [item]);

    /// <inheritdoc/>
    public void InsertRange(int index, IEnumerable<TItem> items)
    {
        _list.InsertRange(index, items);
        _changedSource.Emit(
            new(
                ChangeType: ListChangeType.Add,
                Items: items,
                OldIndex: -1,
                NewIndex: index
            )
        );
    }

    /// <inheritdoc/>
    public void Move(int oldIndex, int newIndex) =>
        MoveRange(oldIndex, newIndex, 1);

    /// <inheritdoc/>
    public void MoveRange(int oldIndex, int newIndex, int count)
    {
        var items = _list
            .Skip(oldIndex)
            .Take(count)
            .ToList();

        _list.RemoveRange(oldIndex, count);
        _list.InsertRange(newIndex, items);

        _changedSource.Emit(
            new(
                ChangeType: ListChangeType.Move,
                Items: items,
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
    public void RemoveAt(int index) =>
        RemoveRange(index, 1);

    /// <inheritdoc/>
    public void RemoveRange(int index, int count)
    {
        var removedItems = _list
            .Skip(index)
            .Take(count)
            .ToList();

        _list.RemoveRange(index, count);
        _changedSource.Emit(
            new(
                ChangeType: ListChangeType.Remove,
                Items: removedItems,
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

    /// <inheritdoc/>
    int IList.Add(object? value) =>
        ((IList)_list).Add(value);

    /// <inheritdoc/>
    bool IList.Contains(object? value) =>
        ((IList)_list).Contains(value);

    /// <inheritdoc/>
    int IList.IndexOf(object? value) =>
        ((IList)_list).IndexOf(value);

    /// <inheritdoc/>
    void IList.Insert(int index, object? value) =>
        ((IList)_list).Insert(index, value);

    /// <inheritdoc/>
    void IList.Remove(object? value) =>
        ((IList)_list).Remove(value);

    /// <inheritdoc/>
    void ICollection.CopyTo(Array array, int index) =>
        ((ICollection)_list).CopyTo(array, index);

    #endregion

    #region Events

    /// <inheritdoc/>
    public Observable<ListChange<TItem>> Changed => _changedSource.Observable;
    private readonly ObservableSource<ListChange<TItem>> _changedSource = new();

    #endregion
}
