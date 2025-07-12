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
/// A list that is observable.
/// </summary>
/// <typeparam name="TItem">The type of item held by the list.</typeparam>
public interface IObservableList<TItem> : IList<TItem>
{
    /// <summary>
    /// Add the given items to the end of the list.
    /// </summary>
    /// <param name="items">The items to add.</param>
    void AddRange(IEnumerable<TItem> items);

    /// <summary>
    /// Insert the given items starting at the given index.
    /// </summary>
    /// <param name="index">The index to insert the items at.</param>
    /// <param name="items">The items to insert.</param>
    void InsertRange(int index, IEnumerable<TItem> items);

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
    void Move(int oldIndex, int newIndex);

    /// <summary>
    /// Move the items within the list.  This is the same as calling the
    /// RemoveAt method for each item followed by the Insert method for each
    /// item but it will result in the Changed observable only being emitted
    /// once with a Move change type.
    /// </summary>
    /// <param name="oldIndex">
    /// The start index of the items that are to be moved.
    /// </param>
    /// <param name="newIndex">
    /// The new start index of the items that are to be moved.
    /// </param>
    /// <param name="count">
    /// The number of items to move.
    /// </param>
    void MoveRange(int oldIndex, int newIndex, int count);

    /// <summary>
    /// Remove the given items from the list.
    /// </summary>
    /// <param name="index">The index to start removing items from.</param>
    /// <param name="count">The number of items to remove.</param>
    void RemoveRange(int index, int count);

    /// <summary>
    /// Observable that emits when the list has been changed.
    /// </summary>
    Observable<ListChange<TItem>> Changed { get; }
}
