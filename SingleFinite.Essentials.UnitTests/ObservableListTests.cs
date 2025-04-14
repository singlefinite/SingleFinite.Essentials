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
    }
}
