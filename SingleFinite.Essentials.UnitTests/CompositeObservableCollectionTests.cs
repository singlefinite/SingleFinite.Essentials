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

using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SingleFinite.Essentials.UnitTests;

[TestClass]
public class CompositeObservableCollectionTests
{
    [TestMethod]
    public void EmptyEnumerables()
    {
        var enumerable = new CompositeObservableCollection<int, ObservableCollection<int>>();

        Assert.AreEqual(0, enumerable.Count);
        Assert.AreEqual(0, enumerable.Count);
    }

    [TestMethod]
    public void SingleEnumerable()
    {
        var observedCollectionEvents = new List<NotifyCollectionChangedEventArgs>();

        var list = new ObservableCollection<int> { 1, 2, 3 };

        var enumerable = new CompositeObservableCollection<int, ObservableCollection<int>>(
            list
        );

        enumerable.CollectionChanged += (sender, args) =>
            observedCollectionEvents.Add(args);

        list.Insert(1, 4);

        Assert.AreEqual(1, observedCollectionEvents.Count);
        Assert.AreEqual(NotifyCollectionChangedAction.Add, observedCollectionEvents[0].Action);
        Assert.AreEqual(1, observedCollectionEvents[0].NewItems?.Count);
        Assert.AreEqual(4, observedCollectionEvents[0].NewItems?[0]);
        Assert.AreEqual(1, observedCollectionEvents[0].NewStartingIndex);

        var newList = enumerable.ToList();

        Assert.AreEqual(4, newList.Count);
        Assert.AreEqual(1, newList[0]);
        Assert.AreEqual(4, newList[1]);
        Assert.AreEqual(2, newList[2]);
        Assert.AreEqual(3, newList[3]);
    }

    [TestMethod]
    public void MultipleEnumerables()
    {
        var observedCollectionEvents = new List<NotifyCollectionChangedEventArgs>();

        var list1 = new ObservableCollection<int> { 1, 2, 3 };
        var list2 = new ObservableCollection<int> { 4, 5, 6 };
        var list3 = new ObservableCollection<int> { 7, 8, 9 };

        var enumerable = new CompositeObservableCollection<int, ObservableCollection<int>>(
            list1, list2, list3
        );

        enumerable.CollectionChanged += (sender, args) =>
            observedCollectionEvents.Add(args);

        list2.Insert(1, 0);

        Assert.AreEqual(1, observedCollectionEvents.Count);
        Assert.AreEqual(NotifyCollectionChangedAction.Add, observedCollectionEvents[0].Action);
        Assert.AreEqual(1, observedCollectionEvents[0].NewItems?.Count);
        Assert.AreEqual(0, observedCollectionEvents[0].NewItems?[0]);
        Assert.AreEqual(4, observedCollectionEvents[0].NewStartingIndex);

        var newList = enumerable.ToList();

        Assert.AreEqual(10, newList.Count);
        Assert.AreEqual(1, newList[0]);
        Assert.AreEqual(2, newList[1]);
        Assert.AreEqual(3, newList[2]);
        Assert.AreEqual(4, newList[3]);
        Assert.AreEqual(0, newList[4]);
        Assert.AreEqual(5, newList[5]);
        Assert.AreEqual(6, newList[6]);
        Assert.AreEqual(7, newList[7]);
        Assert.AreEqual(8, newList[8]);
        Assert.AreEqual(9, newList[9]);
    }

    [TestMethod]
    public void ClearRaisesResetEvent()
    {
        var observedCollectionEvents = new List<NotifyCollectionChangedEventArgs>();

        var list1 = new ObservableCollection<int> { 1, 2, 3 };
        var list2 = new ObservableCollection<int> { 4, 5, 6 };
        var list3 = new ObservableCollection<int> { 7, 8, 9 };

        var enumerable = new CompositeObservableCollection<int, ObservableCollection<int>>(
            list1, list2, list3
        );

        enumerable.CollectionChanged += (sender, args) =>
            observedCollectionEvents.Add(args);

        list2.Clear();

        Assert.AreEqual(1, observedCollectionEvents.Count);
        Assert.AreEqual(NotifyCollectionChangedAction.Reset, observedCollectionEvents[0].Action);
        Assert.IsNull(observedCollectionEvents[0].NewItems);
        Assert.IsNull(observedCollectionEvents[0].OldItems);
        Assert.AreEqual(-1, observedCollectionEvents[0].NewStartingIndex);
        Assert.AreEqual(-1, observedCollectionEvents[0].OldStartingIndex);

        var enumerableAsList = enumerable.ToList();

        Assert.AreEqual(6, enumerableAsList.Count);
        Assert.AreEqual(1, enumerableAsList[0]);
        Assert.AreEqual(2, enumerableAsList[1]);
        Assert.AreEqual(3, enumerableAsList[2]);
        Assert.AreEqual(7, enumerableAsList[3]);
        Assert.AreEqual(8, enumerableAsList[4]);
        Assert.AreEqual(9, enumerableAsList[5]);
    }

    [TestMethod]
    public void AddRaisesAddEvent()
    {
        var observedCollectionEvents = new List<NotifyCollectionChangedEventArgs>();

        var list1 = new ObservableCollection<int> { 1, 2, 3 };
        var list2 = new ObservableCollection<int> { 4, 5, 6 };
        var list3 = new ObservableCollection<int> { 7, 8, 9 };

        var enumerable = new CompositeObservableCollection<int, ObservableCollection<int>>(
            list1, list2, list3
        );

        enumerable.CollectionChanged += (sender, args) =>
            observedCollectionEvents.Add(args);

        list2.Add(0);

        Assert.AreEqual(1, observedCollectionEvents.Count);
        Assert.AreEqual(NotifyCollectionChangedAction.Add, observedCollectionEvents[0].Action);
        Assert.AreEqual(1, observedCollectionEvents[0].NewItems?.Count);
        Assert.AreEqual(0, observedCollectionEvents[0].NewItems?[0]);
        Assert.IsNull(observedCollectionEvents[0].OldItems);
        Assert.AreEqual(6, observedCollectionEvents[0].NewStartingIndex);
        Assert.AreEqual(-1, observedCollectionEvents[0].OldStartingIndex);

        var enumerableAsList = enumerable.ToList();

        Assert.AreEqual(10, enumerableAsList.Count);
        Assert.AreEqual(1, enumerableAsList[0]);
        Assert.AreEqual(2, enumerableAsList[1]);
        Assert.AreEqual(3, enumerableAsList[2]);
        Assert.AreEqual(4, enumerableAsList[3]);
        Assert.AreEqual(5, enumerableAsList[4]);
        Assert.AreEqual(6, enumerableAsList[5]);
        Assert.AreEqual(0, enumerableAsList[6]);
        Assert.AreEqual(7, enumerableAsList[7]);
        Assert.AreEqual(8, enumerableAsList[8]);
        Assert.AreEqual(9, enumerableAsList[9]);
    }

    [TestMethod]
    public void RemoveRaisesRemoveEvent()
    {
        var observedCollectionEvents = new List<NotifyCollectionChangedEventArgs>();

        var list1 = new ObservableCollection<int> { 1, 2, 3 };
        var list2 = new ObservableCollection<int> { 4, 5, 6 };
        var list3 = new ObservableCollection<int> { 7, 8, 9 };

        var enumerable = new CompositeObservableCollection<int, ObservableCollection<int>>(
            list1, list2, list3
        );

        enumerable.CollectionChanged += (sender, args) =>
            observedCollectionEvents.Add(args);

        list2.RemoveAt(2);

        Assert.AreEqual(1, observedCollectionEvents.Count);
        Assert.AreEqual(NotifyCollectionChangedAction.Remove, observedCollectionEvents[0].Action);
        Assert.AreEqual(1, observedCollectionEvents[0].OldItems?.Count);
        Assert.AreEqual(6, observedCollectionEvents[0].OldItems?[0]);
        Assert.IsNull(observedCollectionEvents[0].NewItems);
        Assert.AreEqual(-1, observedCollectionEvents[0].NewStartingIndex);
        Assert.AreEqual(5, observedCollectionEvents[0].OldStartingIndex);

        var enumerableAsList = enumerable.ToList();

        Assert.AreEqual(8, enumerableAsList.Count);
        Assert.AreEqual(1, enumerableAsList[0]);
        Assert.AreEqual(2, enumerableAsList[1]);
        Assert.AreEqual(3, enumerableAsList[2]);
        Assert.AreEqual(4, enumerableAsList[3]);
        Assert.AreEqual(5, enumerableAsList[4]);
        Assert.AreEqual(7, enumerableAsList[5]);
        Assert.AreEqual(8, enumerableAsList[6]);
        Assert.AreEqual(9, enumerableAsList[7]);
    }

    [TestMethod]
    public void MoveRaisesMoveEvent()
    {
        var observedCollectionEvents = new List<NotifyCollectionChangedEventArgs>();

        var list1 = new ObservableCollection<int> { 1, 2, 3 };
        var list2 = new ObservableCollection<int> { 4, 5, 6 };
        var list3 = new ObservableCollection<int> { 7, 8, 9 };

        var enumerable = new CompositeObservableCollection<int, ObservableCollection<int>>(
            list1, list2, list3
        );

        enumerable.CollectionChanged += (sender, args) =>
            observedCollectionEvents.Add(args);

        list2.Move(0, 2);

        Assert.AreEqual(1, observedCollectionEvents.Count);
        Assert.AreEqual(NotifyCollectionChangedAction.Move, observedCollectionEvents[0].Action);
        Assert.AreEqual(1, observedCollectionEvents[0].OldItems?.Count);
        Assert.AreEqual(4, observedCollectionEvents[0].OldItems?[0]);
        Assert.AreEqual(1, observedCollectionEvents[0].NewItems?.Count);
        Assert.AreEqual(4, observedCollectionEvents[0].NewItems?[0]);
        Assert.AreEqual(5, observedCollectionEvents[0].NewStartingIndex);
        Assert.AreEqual(3, observedCollectionEvents[0].OldStartingIndex);

        var enumerableAsList = enumerable.ToList();

        Assert.AreEqual(9, enumerableAsList.Count);
        Assert.AreEqual(1, enumerableAsList[0]);
        Assert.AreEqual(2, enumerableAsList[1]);
        Assert.AreEqual(3, enumerableAsList[2]);
        Assert.AreEqual(5, enumerableAsList[3]);
        Assert.AreEqual(6, enumerableAsList[4]);
        Assert.AreEqual(4, enumerableAsList[5]);
        Assert.AreEqual(7, enumerableAsList[6]);
        Assert.AreEqual(8, enumerableAsList[7]);
        Assert.AreEqual(9, enumerableAsList[8]);
    }

    [TestMethod]
    public void ReplaceRaisesReplaceEvent()
    {
        var observedCollectionEvents = new List<NotifyCollectionChangedEventArgs>();

        var list1 = new ObservableCollection<int> { 1, 2, 3 };
        var list2 = new ObservableCollection<int> { 4, 5, 6 };
        var list3 = new ObservableCollection<int> { 7, 8, 9 };

        var enumerable = new CompositeObservableCollection<int, ObservableCollection<int>>(
            list1, list2, list3
        );

        enumerable.CollectionChanged += (sender, args) =>
            observedCollectionEvents.Add(args);

        list2[1] = 0;

        Assert.AreEqual(1, observedCollectionEvents.Count);
        Assert.AreEqual(NotifyCollectionChangedAction.Replace, observedCollectionEvents[0].Action);
        Assert.AreEqual(1, observedCollectionEvents[0].OldItems?.Count);
        Assert.AreEqual(5, observedCollectionEvents[0].OldItems?[0]);
        Assert.AreEqual(1, observedCollectionEvents[0].NewItems?.Count);
        Assert.AreEqual(0, observedCollectionEvents[0].NewItems?[0]);
        Assert.AreEqual(4, observedCollectionEvents[0].NewStartingIndex);
        Assert.AreEqual(4, observedCollectionEvents[0].OldStartingIndex);

        var enumerableAsList = enumerable.ToList();

        Assert.AreEqual(9, enumerableAsList.Count);
        Assert.AreEqual(1, enumerableAsList[0]);
        Assert.AreEqual(2, enumerableAsList[1]);
        Assert.AreEqual(3, enumerableAsList[2]);
        Assert.AreEqual(4, enumerableAsList[3]);
        Assert.AreEqual(0, enumerableAsList[4]);
        Assert.AreEqual(6, enumerableAsList[5]);
        Assert.AreEqual(7, enumerableAsList[6]);
        Assert.AreEqual(8, enumerableAsList[7]);
        Assert.AreEqual(9, enumerableAsList[8]);
    }
}
