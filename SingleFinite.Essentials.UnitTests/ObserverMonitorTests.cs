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
public class ObserverMonitorTests
{
    [TestMethod]
    public void Dispose_Is_Called_When_Owner_Removed()
    {
        var monitor = new ObserverMonitor();

        var owner1 = new object();
        var source1 = new ObservableSource<int>();
        var source2 = new ObservableSource<string>();

        var owner2 = new object();
        var source3 = new ObservableSource<bool>();
        var source4 = new ObservableSource<double>();

        var observedArgs = new List<object>();

        var observer1 = source1.Observable
            .Observe()
            .OnEach(args => observedArgs.Add(args));
        var observer2 = source2.Observable
            .Observe()
            .OnEach(args => observedArgs.Add(args));
        var observer3 = source3.Observable
            .Observe()
            .OnEach(args => observedArgs.Add(args));
        var observer4 = source4.Observable
            .Observe()
            .OnEach(args => observedArgs.Add(args));

        monitor.Add(
            owner: owner1,
            observer1,
            observer2
        );
        monitor.Add(
            owner: owner2,
            observer3,
            observer4
        );

        source1.Emit(9);
        source2.Emit("HI");
        source3.Emit(true);
        source4.Emit(5.7);

        Assert.AreEqual(4, observedArgs.Count);
        Assert.AreEqual(9, observedArgs[0]);
        Assert.AreEqual("HI", observedArgs[1]);
        Assert.AreEqual(true, observedArgs[2]);
        Assert.AreEqual(5.7, observedArgs[3]);

        observedArgs.Clear();

        monitor.Remove(owner1);

        Assert.IsTrue(observer1.IsDisposed);
        Assert.IsTrue(observer2.IsDisposed);
        Assert.IsFalse(observer3.IsDisposed);
        Assert.IsFalse(observer4.IsDisposed);

        source1.Emit(9);
        source2.Emit("HI");
        source3.Emit(true);
        source4.Emit(5.7);

        Assert.AreEqual(2, observedArgs.Count);
        Assert.AreEqual(true, observedArgs[0]);
        Assert.AreEqual(5.7, observedArgs[1]);
    }

    [TestMethod]
    public void Clear_Disposes_Of_All_Observers()
    {
        var monitor = new ObserverMonitor();

        var owner1 = new object();
        var source1 = new ObservableSource<int>();
        var source2 = new ObservableSource<string>();

        var owner2 = new object();
        var source3 = new ObservableSource<bool>();
        var source4 = new ObservableSource<double>();

        var observedArgs = new List<object>();

        var observer1 = source1.Observable
            .Observe()
            .OnEach(args => observedArgs.Add(args));
        var observer2 = source2.Observable
            .Observe()
            .OnEach(args => observedArgs.Add(args));
        var observer3 = source3.Observable
            .Observe()
            .OnEach(args => observedArgs.Add(args));
        var observer4 = source4.Observable
            .Observe()
            .OnEach(args => observedArgs.Add(args));

        monitor.Add(owner1, observer1);
        monitor.Add(owner1, observer2);
        monitor.Add(owner2, observer3);
        monitor.Add(owner2, observer4);

        source1.Emit(9);
        source2.Emit("HI");
        source3.Emit(true);
        source4.Emit(5.7);

        Assert.AreEqual(4, observedArgs.Count);
        Assert.AreEqual(9, observedArgs[0]);
        Assert.AreEqual("HI", observedArgs[1]);
        Assert.AreEqual(true, observedArgs[2]);
        Assert.AreEqual(5.7, observedArgs[3]);

        observedArgs.Clear();

        monitor.Clear();

        Assert.IsTrue(observer1.IsDisposed);
        Assert.IsTrue(observer2.IsDisposed);
        Assert.IsTrue(observer3.IsDisposed);
        Assert.IsTrue(observer4.IsDisposed);

        source1.Emit(9);
        source2.Emit("HI");
        source3.Emit(true);
        source4.Emit(5.7);

        Assert.AreEqual(0, observedArgs.Count);
    }
}
