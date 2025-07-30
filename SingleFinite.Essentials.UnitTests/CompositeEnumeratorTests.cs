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
public class CompositeEnumeratorTests
{
    [TestMethod]
    public void EmptyEnumerators()
    {
        var enumerator = new CompositeEnumerator<int>();

        Assert.IsFalse(enumerator.MoveNext());
        Assert.AreEqual(default, enumerator.Current);
    }

    [TestMethod]
    public void SingleEnumerator()
    {
        var list = new List<int> { 1, 2, 3 };

        var enumerator = new CompositeEnumerator<int>(
            list.GetEnumerator()
        );

        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual(1, enumerator.Current);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual(2, enumerator.Current);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual(3, enumerator.Current);
        Assert.IsFalse(enumerator.MoveNext());
    }

    [TestMethod]
    public void MultipleEnumerators()
    {
        var list1 = new List<int> { 1, 2 };
        var list2 = new List<int> { 3, 4, 5 };

        var enumerator = new CompositeEnumerator<int>(
            list1.GetEnumerator(),
            list2.GetEnumerator()
        );

        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual(1, enumerator.Current);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual(2, enumerator.Current);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual(3, enumerator.Current);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual(4, enumerator.Current);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual(5, enumerator.Current);
        Assert.IsFalse(enumerator.MoveNext());
    }
}
