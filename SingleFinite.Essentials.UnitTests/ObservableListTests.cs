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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SingleFinite.Essentials.UnitTests;

[TestClass]
public class ObservableListTests
{
    [TestMethod]
    public void Add_Is_Observed()
    {
        var observedChanges = new List<ListChange<int>>();

        var list = new ObservableList<int>();
        list.Changed
            .Observe()
            .OnEach(observedChanges.Add);

        Assert.AreEqual(0, observedChanges.Count);

        list.Add(1);

        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(1, list[0]);

        Assert.AreEqual(1, observedChanges.Count);
        Assert.AreEqual(ListChangeType.Add, observedChanges[0].ChangeType);
        Assert.AreEqual(1, observedChanges[0].Items.Count());
        Assert.AreEqual(1, observedChanges[0].Items.First());
        Assert.AreEqual(-1, observedChanges[0].OldIndex);
        Assert.AreEqual(0, observedChanges[0].NewIndex);

        observedChanges.Clear();

        list.Add(2);

        Assert.AreEqual(2, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);

        Assert.AreEqual(1, observedChanges.Count);
        Assert.AreEqual(ListChangeType.Add, observedChanges[0].ChangeType);
        Assert.AreEqual(1, observedChanges[0].Items.Count());
        Assert.AreEqual(2, observedChanges[0].Items.First());
        Assert.AreEqual(-1, observedChanges[0].OldIndex);
        Assert.AreEqual(1, observedChanges[0].NewIndex);
    }

    [TestMethod]
    public void AddRange_Is_Observed()
    {
        var observedChanges = new List<ListChange<int>>();

        var list = new ObservableList<int>();
        list.Changed
            .Observe()
            .OnEach(observedChanges.Add);

        Assert.AreEqual(0, observedChanges.Count);

        list.AddRange([1, 2, 3]);

        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);

        Assert.AreEqual(1, observedChanges.Count);
        Assert.AreEqual(ListChangeType.Add, observedChanges[0].ChangeType);
        Assert.AreEqual(3, observedChanges[0].Items.Count());
        Assert.AreEqual(1, observedChanges[0].Items.First());
        Assert.AreEqual(2, observedChanges[0].Items.Skip(1).First());
        Assert.AreEqual(3, observedChanges[0].Items.Skip(2).First());
        Assert.AreEqual(-1, observedChanges[0].OldIndex);
        Assert.AreEqual(0, observedChanges[0].NewIndex);

        observedChanges.Clear();

        list.AddRange([4, 5, 6]);

        Assert.AreEqual(6, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);
        Assert.AreEqual(4, list[3]);
        Assert.AreEqual(5, list[4]);
        Assert.AreEqual(6, list[5]);

        Assert.AreEqual(1, observedChanges.Count);
        Assert.AreEqual(ListChangeType.Add, observedChanges[0].ChangeType);
        Assert.AreEqual(3, observedChanges[0].Items.Count());
        Assert.AreEqual(4, observedChanges[0].Items.First());
        Assert.AreEqual(5, observedChanges[0].Items.Skip(1).First());
        Assert.AreEqual(6, observedChanges[0].Items.Skip(2).First());
        Assert.AreEqual(-1, observedChanges[0].OldIndex);
        Assert.AreEqual(3, observedChanges[0].NewIndex);
    }

    [TestMethod]
    public void Move_Is_Observed()
    {
        var observedChanges = new List<ListChange<int>>();

        var list = new ObservableList<int>([1, 4, 2, 3, 5]);
        list.Changed
            .Observe()
            .OnEach(observedChanges.Add);

        Assert.AreEqual(0, observedChanges.Count);

        list.Move(1, 3);

        Assert.AreEqual(5, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);
        Assert.AreEqual(4, list[3]);
        Assert.AreEqual(5, list[4]);

        Assert.AreEqual(1, observedChanges.Count);
        Assert.AreEqual(ListChangeType.Move, observedChanges[0].ChangeType);
        Assert.AreEqual(1, observedChanges[0].Items.Count());
        Assert.AreEqual(4, observedChanges[0].Items.First());
        Assert.AreEqual(1, observedChanges[0].OldIndex);
        Assert.AreEqual(3, observedChanges[0].NewIndex);
    }

    [TestMethod]
    public void MoveRange_Is_Observed()
    {
        var observedChanges = new List<ListChange<int>>();

        var list = new ObservableList<int>([1, 4, 2, 3, 5]);
        list.Changed
            .Observe()
            .OnEach(observedChanges.Add);

        Assert.AreEqual(0, observedChanges.Count);

        list.MoveRange(
            oldIndex: 1,
            newIndex: 2,
            count: 2
        );

        Assert.AreEqual(5, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(3, list[1]);
        Assert.AreEqual(4, list[2]);
        Assert.AreEqual(2, list[3]);
        Assert.AreEqual(5, list[4]);

        Assert.AreEqual(1, observedChanges.Count);
        Assert.AreEqual(ListChangeType.Move, observedChanges[0].ChangeType);
        Assert.AreEqual(2, observedChanges[0].Items.Count());
        Assert.AreEqual(4, observedChanges[0].Items.First());
        Assert.AreEqual(2, observedChanges[0].Items.Skip(1).First());
        Assert.AreEqual(1, observedChanges[0].OldIndex);
        Assert.AreEqual(2, observedChanges[0].NewIndex);

        observedChanges.Clear();

        list.MoveRange(
            oldIndex: 5,
            newIndex: 6,
            count: 1
        );

        Assert.AreEqual(0, observedChanges.Count);
    }

    [TestMethod]
    public void RemoveAt_Is_Observed()
    {
        var observedChanges = new List<ListChange<int>>();

        var list = new ObservableList<int>([1, 2, 3, 4, 5]);
        list.Changed
            .Observe()
            .OnEach(observedChanges.Add);

        Assert.AreEqual(0, observedChanges.Count);

        list.RemoveAt(3);

        Assert.AreEqual(4, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);
        Assert.AreEqual(5, list[3]);

        Assert.AreEqual(1, observedChanges.Count);
        Assert.AreEqual(ListChangeType.Remove, observedChanges[0].ChangeType);
        Assert.AreEqual(1, observedChanges[0].Items.Count());
        Assert.AreEqual(4, observedChanges[0].Items.First());
        Assert.AreEqual(3, observedChanges[0].OldIndex);
        Assert.AreEqual(-1, observedChanges[0].NewIndex);
    }

    [TestMethod]
    public void RemoveRange_Is_Observed()
    {
        var observedChanges = new List<ListChange<int>>();

        var list = new ObservableList<int>([1, 2, 3, 4, 5]);
        list.Changed
            .Observe()
            .OnEach(observedChanges.Add);

        Assert.AreEqual(0, observedChanges.Count);

        list.RemoveRange(index: 1, count: 2);

        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(4, list[1]);
        Assert.AreEqual(5, list[2]);

        Assert.AreEqual(1, observedChanges.Count);
        Assert.AreEqual(ListChangeType.Remove, observedChanges[0].ChangeType);
        Assert.AreEqual(2, observedChanges[0].Items.Count());
        Assert.AreEqual(2, observedChanges[0].Items.First());
        Assert.AreEqual(3, observedChanges[0].Items.Skip(1).First());
        Assert.AreEqual(1, observedChanges[0].OldIndex);
        Assert.AreEqual(-1, observedChanges[0].NewIndex);

        observedChanges.Clear();

        list.RemoveRange(index: 3, count: 1);

        Assert.AreEqual(0, observedChanges.Count);
    }

    [TestMethod]
    public void Remove_Is_Observed()
    {
        var observedChanges = new List<ListChange<int>>();

        var list = new ObservableList<int>([1, 2, 3, 4, 5]);
        list.Changed
            .Observe()
            .OnEach(observedChanges.Add);

        Assert.AreEqual(0, observedChanges.Count);

        list.Remove(4);

        Assert.AreEqual(4, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);
        Assert.AreEqual(5, list[3]);

        Assert.AreEqual(1, observedChanges.Count);
        Assert.AreEqual(ListChangeType.Remove, observedChanges[0].ChangeType);
        Assert.AreEqual(1, observedChanges[0].Items.Count());
        Assert.AreEqual(4, observedChanges[0].Items.First());
        Assert.AreEqual(3, observedChanges[0].OldIndex);
        Assert.AreEqual(-1, observedChanges[0].NewIndex);

        observedChanges.Clear();

        list.Remove(4);

        Assert.AreEqual(0, observedChanges.Count);
    }

    [TestMethod]
    public void Indexer_Is_Observed()
    {
        var observedChanges = new List<ListChange<int>>();

        var list = new ObservableList<int>([1, 2, 3, 4, 5]);
        list.Changed
            .Observe()
            .OnEach(observedChanges.Add);

        Assert.AreEqual(0, observedChanges.Count);

        list[2] = 6;

        Assert.AreEqual(5, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(6, list[2]);
        Assert.AreEqual(4, list[3]);
        Assert.AreEqual(5, list[4]);

        Assert.AreEqual(1, observedChanges.Count);
        Assert.AreEqual(ListChangeType.Replace, observedChanges[0].ChangeType);
        Assert.AreEqual(1, observedChanges[0].Items.Count());
        Assert.AreEqual(6, observedChanges[0].Items.First());
        Assert.AreEqual(1, observedChanges[0].ReplacedItems.Count());
        Assert.AreEqual(3, observedChanges[0].ReplacedItems.First());
        Assert.AreEqual(2, observedChanges[0].OldIndex);
        Assert.AreEqual(2, observedChanges[0].NewIndex);
    }

    [TestMethod]
    public void Clear_Is_Observed()
    {
        var observedChanges = new List<ListChange<int>>();

        var list = new ObservableList<int>([1, 2, 3, 4, 5]);
        list.Changed
            .Observe()
            .OnEach(observedChanges.Add);

        Assert.AreEqual(0, observedChanges.Count);

        list.Clear();

        Assert.AreEqual(0, list.Count);

        Assert.AreEqual(1, observedChanges.Count);
        Assert.AreEqual(5, observedChanges[0].Items.Count());
        Assert.AreEqual(1, observedChanges[0].Items.First());
        Assert.AreEqual(2, observedChanges[0].Items.Skip(1).First());
        Assert.AreEqual(3, observedChanges[0].Items.Skip(2).First());
        Assert.AreEqual(4, observedChanges[0].Items.Skip(3).First());
        Assert.AreEqual(5, observedChanges[0].Items.Skip(4).First());
        Assert.AreEqual(0, observedChanges[0].OldIndex);
        Assert.AreEqual(-1, observedChanges[0].NewIndex);

        observedChanges.Clear();

        list.Clear();

        Assert.AreEqual(0, observedChanges.Count);
    }

    [TestMethod]
    public void NotifyBehavior_Matches_ObservableCollection()
    {
        var observedListPropertyChanges = new List<string?>();
        var observedListCollectionChanges = new List<NotifyCollectionChangedEventArgs>();

        var observedCollectionPropertyChanges = new List<string?>();
        var observedCollectionCollectionChanges = new List<NotifyCollectionChangedEventArgs>();

        var list = new ObservableList<int>([1, 2, 3, 4, 5]);
        var collection = new ObservableCollection<int>(list);

        list.PropertyChanged += (sender, e) =>
            observedListPropertyChanges.Add(e.PropertyName);
        list.CollectionChanged += (sender, e) =>
            observedListCollectionChanges.Add(e);

        ((INotifyPropertyChanged)collection).PropertyChanged += (sender, e) =>
            observedCollectionPropertyChanges.Add(e.PropertyName);
        collection.CollectionChanged += (sender, e) =>
            observedCollectionCollectionChanges.Add(e);

        list.Add(6);
        collection.Add(6);

        Assert.AreEqual(2, observedListPropertyChanges.Count);
        Assert.AreEqual(2, observedCollectionPropertyChanges.Count);
        Assert.AreEqual(observedListPropertyChanges[0], observedCollectionPropertyChanges[0]);
        Assert.AreEqual(observedListPropertyChanges[1], observedCollectionPropertyChanges[1]);

        Assert.AreEqual(1, observedListCollectionChanges.Count);
        Assert.AreEqual(1, observedCollectionCollectionChanges.Count);
        Assert.AreEqual(observedListCollectionChanges[0].Action, observedCollectionCollectionChanges[0].Action);
        AssertItemsAreEqual(observedListCollectionChanges[0].OldItems, observedCollectionCollectionChanges[0].OldItems);
        Assert.AreEqual(observedListCollectionChanges[0].OldStartingIndex, observedCollectionCollectionChanges[0].OldStartingIndex);
        AssertItemsAreEqual(observedListCollectionChanges[0].NewItems, observedCollectionCollectionChanges[0].NewItems);
        Assert.AreEqual(observedListCollectionChanges[0].NewStartingIndex, observedCollectionCollectionChanges[0].NewStartingIndex);

        observedListPropertyChanges.Clear();
        observedListCollectionChanges.Clear();
        observedCollectionPropertyChanges.Clear();
        observedCollectionCollectionChanges.Clear();

        list.Remove(3);
        collection.Remove(3);

        Assert.AreEqual(2, observedListPropertyChanges.Count);
        Assert.AreEqual(2, observedCollectionPropertyChanges.Count);
        Assert.AreEqual(observedListPropertyChanges[0], observedCollectionPropertyChanges[0]);
        Assert.AreEqual(observedListPropertyChanges[1], observedCollectionPropertyChanges[1]);

        Assert.AreEqual(1, observedListCollectionChanges.Count);
        Assert.AreEqual(1, observedCollectionCollectionChanges.Count);
        Assert.AreEqual(observedListCollectionChanges[0].Action, observedCollectionCollectionChanges[0].Action);
        AssertItemsAreEqual(observedListCollectionChanges[0].OldItems, observedCollectionCollectionChanges[0].OldItems);
        Assert.AreEqual(observedListCollectionChanges[0].OldStartingIndex, observedCollectionCollectionChanges[0].OldStartingIndex);
        AssertItemsAreEqual(observedListCollectionChanges[0].NewItems, observedCollectionCollectionChanges[0].NewItems);
        Assert.AreEqual(observedListCollectionChanges[0].NewStartingIndex, observedCollectionCollectionChanges[0].NewStartingIndex);

        observedListPropertyChanges.Clear();
        observedListCollectionChanges.Clear();
        observedCollectionPropertyChanges.Clear();
        observedCollectionCollectionChanges.Clear();

        list.Move(4, 0);
        collection.Move(4, 0);

        Assert.AreEqual(1, observedListPropertyChanges.Count);
        Assert.AreEqual(1, observedCollectionPropertyChanges.Count);
        Assert.AreEqual(observedListPropertyChanges[0], observedCollectionPropertyChanges[0]);

        Assert.AreEqual(1, observedListCollectionChanges.Count);
        Assert.AreEqual(1, observedCollectionCollectionChanges.Count);
        Assert.AreEqual(observedListCollectionChanges[0].Action, observedCollectionCollectionChanges[0].Action);
        AssertItemsAreEqual(observedListCollectionChanges[0].OldItems, observedCollectionCollectionChanges[0].OldItems);
        Assert.AreEqual(observedListCollectionChanges[0].OldStartingIndex, observedCollectionCollectionChanges[0].OldStartingIndex);
        AssertItemsAreEqual(observedListCollectionChanges[0].NewItems, observedCollectionCollectionChanges[0].NewItems);
        Assert.AreEqual(observedListCollectionChanges[0].NewStartingIndex, observedCollectionCollectionChanges[0].NewStartingIndex);

        observedListPropertyChanges.Clear();
        observedListCollectionChanges.Clear();
        observedCollectionPropertyChanges.Clear();
        observedCollectionCollectionChanges.Clear();

        list[0] = 9;
        collection[0] = 9;

        Assert.AreEqual(1, observedListPropertyChanges.Count);
        Assert.AreEqual(1, observedCollectionPropertyChanges.Count);
        Assert.AreEqual(observedListPropertyChanges[0], observedCollectionPropertyChanges[0]);

        Assert.AreEqual(1, observedListCollectionChanges.Count);
        Assert.AreEqual(1, observedCollectionCollectionChanges.Count);
        Assert.AreEqual(observedListCollectionChanges[0].Action, observedCollectionCollectionChanges[0].Action);
        AssertItemsAreEqual(observedListCollectionChanges[0].OldItems, observedCollectionCollectionChanges[0].OldItems);
        Assert.AreEqual(observedListCollectionChanges[0].OldStartingIndex, observedCollectionCollectionChanges[0].OldStartingIndex);
        AssertItemsAreEqual(observedListCollectionChanges[0].NewItems, observedCollectionCollectionChanges[0].NewItems);
        Assert.AreEqual(observedListCollectionChanges[0].NewStartingIndex, observedCollectionCollectionChanges[0].NewStartingIndex);

        observedListPropertyChanges.Clear();
        observedListCollectionChanges.Clear();
        observedCollectionPropertyChanges.Clear();
        observedCollectionCollectionChanges.Clear();

        list.Clear();
        collection.Clear();

        Assert.AreEqual(2, observedListPropertyChanges.Count);
        Assert.AreEqual(2, observedCollectionPropertyChanges.Count);
        Assert.AreEqual(observedListPropertyChanges[0], observedCollectionPropertyChanges[0]);
        Assert.AreEqual(observedListPropertyChanges[1], observedCollectionPropertyChanges[1]);

        Assert.AreEqual(1, observedListCollectionChanges.Count);
        Assert.AreEqual(1, observedCollectionCollectionChanges.Count);
        Assert.AreEqual(observedListCollectionChanges[0].Action, observedCollectionCollectionChanges[0].Action);
        AssertItemsAreEqual(observedListCollectionChanges[0].OldItems, observedCollectionCollectionChanges[0].OldItems);
        Assert.AreEqual(observedListCollectionChanges[0].OldStartingIndex, observedCollectionCollectionChanges[0].OldStartingIndex);
        AssertItemsAreEqual(observedListCollectionChanges[0].NewItems, observedCollectionCollectionChanges[0].NewItems);
        Assert.AreEqual(observedListCollectionChanges[0].NewStartingIndex, observedCollectionCollectionChanges[0].NewStartingIndex);

        observedListPropertyChanges.Clear();
        observedListCollectionChanges.Clear();
        observedCollectionPropertyChanges.Clear();
        observedCollectionCollectionChanges.Clear();
    }

    private static void AssertItemsAreEqual(IList? expected, IList? actual)
    {
        if (expected == null && actual == null)
            return;
        if (expected == null || actual == null)
            Assert.Fail("One of the lists is null.");

        Assert.AreEqual(expected.Count, actual.Count);
        for (var i = 0; i < expected.Count; i++)
            Assert.AreEqual(expected[i], actual[i]);
    }
}
