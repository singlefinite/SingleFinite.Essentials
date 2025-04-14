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
/// Possible types of list changes.
/// </summary>
public enum ListChangeType
{
    /// <summary>
    /// An item was added.
    /// </summary>
    Add,

    /// <summary>
    /// An item was removed.
    /// </summary>
    Remove,

    /// <summary>
    /// An item was moved.
    /// </summary>
    Move
}

/// <summary>
/// Details about a change made to a list.
/// </summary>
/// <typeparam name="TItem">
/// The type of items that are the subject of the change.
/// </typeparam>
/// <param name="ChangeType">The type of change.</param>
/// <param name="Items">The items that are the subject of the change.</param>
/// <param name="OldIndex">
/// When the change is Remove or Move this will be the starting index of the
/// items before the change, otherwise it will be -1.
/// </param>
/// <param name="NewIndex">
/// When the change is Add or Move this will be the starting index of the items
/// after the change, otherwise it will be -1.
/// </param>
public record ListChange<TItem>(
    ListChangeType ChangeType,
    IEnumerable<TItem> Items,
    int OldIndex,
    int NewIndex
);
