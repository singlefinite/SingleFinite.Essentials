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
public class ObservableTests
{
    [TestMethod]
    public void Observe_Method_Invokes_Callback_When_Event_Raised()
    {
        var observedNumber = 0;
        var observableSource = new ObservableSource<int>();
        observableSource.Observable
            .Observe()
            .OnEach(args => observedNumber = args);

        Assert.AreEqual(0, observedNumber);

        observableSource.Emit(81);

        Assert.AreEqual(81, observedNumber);
    }

    [TestMethod]
    public void Dispose_Method_Removes_Callback()
    {
        var observedNumber = 0;
        var observableSource = new ObservableSource<int>();
        var observer = observableSource.Observable
            .Observe()
            .OnEach(args => observedNumber = args);

        Assert.AreEqual(0, observedNumber);

        observableSource.Emit(81);

        Assert.AreEqual(81, observedNumber);

        observer.Dispose();

        observableSource.Emit(99);

        Assert.AreEqual(81, observedNumber);
    }

    [TestMethod]
    public void Observe_Method_Raises_Event_When_Event_Raised()
    {
        var observedNumber = 0;
        var observableSource = new ObservableSource<int>();
        observableSource.Observable.Event += args => observedNumber = args;

        Assert.AreEqual(0, observedNumber);

        observableSource.Emit(81);

        Assert.AreEqual(81, observedNumber);
    }

    [TestMethod]
    public void Combine_Emits_Through()
    {
        var observedNumbers = new List<int>();

        var firstObservableSource = new ObservableSource<int>();
        var secondObservableSource = new ObservableSource<int>();
        var combinedObserver = Observable.Combine(
            firstObservableSource.Observable,
            secondObservableSource.Observable
        );

        combinedObserver.OnEach(observedNumbers.Add);

        firstObservableSource.Emit(9);
        Assert.HasCount(1, observedNumbers);
        Assert.AreEqual(9, observedNumbers[0]);

        observedNumbers.Clear();

        secondObservableSource.Emit(11);
        Assert.HasCount(1, observedNumbers);
        Assert.AreEqual(11, observedNumbers[0]);
    }

    [TestMethod]
    public void Combine_Continues_To_Emit_After_Single_Observer_Disposed()
    {
        var observedNumbers = new List<int>();

        var firstObservableSource = new ObservableSource<int>();
        var secondObservableSource = new ObservableSource<int>();

        var firstObserver = firstObservableSource.Observable.Observe();
        var secondObserver = secondObservableSource.Observable.Observe();

        var combinedObserver = Observable.Combine(
            firstObserver,
            secondObserver
        );

        combinedObserver.OnEach(observedNumbers.Add);

        firstObserver.Dispose();
        Assert.IsFalse(combinedObserver.IsDisposed);

        firstObservableSource.Emit(9);
        Assert.IsEmpty(observedNumbers);

        secondObservableSource.Emit(11);
        Assert.HasCount(1, observedNumbers);
        Assert.AreEqual(11, observedNumbers[0]);
    }

    [TestMethod]
    public void Combine_Dispose_Will_Dispose_Combined_Observers()
    {
        var firstObservableSource = new ObservableSource<int>();
        var secondObservableSource = new ObservableSource<int>();

        var firstObserver = firstObservableSource.Observable.Observe();
        var secondObserver = secondObservableSource.Observable.Observe();

        var combinedObserver = Observable.Combine(
            firstObserver,
            secondObserver
        );

        Assert.IsFalse(combinedObserver.IsDisposed);
        Assert.IsFalse(firstObserver.IsDisposed);
        Assert.IsFalse(secondObserver.IsDisposed);

        combinedObserver.Dispose();

        Assert.IsTrue(combinedObserver.IsDisposed);
        Assert.IsTrue(firstObserver.IsDisposed);
        Assert.IsTrue(secondObserver.IsDisposed);
    }
}
