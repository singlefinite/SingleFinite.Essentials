// MIT License
// Copyright (c) 2026 Single Finite
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
public class EventObservableTests
{
    [TestMethod]
    public void Observe_Method_Invokes_Callback_When_Event_Raised()
    {
        var observedNumber = 0;
        var observableSource = new EventObservableSource<int>();
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
        var observableSource = new EventObservableSource<int>();
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
        var observableSource = new EventObservableSource<int>();
        observableSource.Observable.Event += args => observedNumber = args;

        Assert.AreEqual(0, observedNumber);

        observableSource.Emit(81);

        Assert.AreEqual(81, observedNumber);
    }

    [TestMethod]
    public void Combine_Emits_Through()
    {
        var observedNumbers = new List<int>();

        var firstEventObservableSource = new EventObservableSource<int>();
        var secondEventObservableSource = new EventObservableSource<int>();
        var combinedEventObserver = EventObservable.Combine(
            firstEventObservableSource.Observable,
            secondEventObservableSource.Observable
        );

        combinedEventObserver.OnEach(observedNumbers.Add);

        firstEventObservableSource.Emit(9);
        Assert.HasCount(1, observedNumbers);
        Assert.AreEqual(9, observedNumbers[0]);

        observedNumbers.Clear();

        secondEventObservableSource.Emit(11);
        Assert.HasCount(1, observedNumbers);
        Assert.AreEqual(11, observedNumbers[0]);
    }

    [TestMethod]
    public void Combine_Dispose_Does_Not_Dispose_Of_Original_Observers()
    {
        var observedCombinedNumbers = new List<int>();

        var firstEventObservableSource = new EventObservableSource<int>();
        var secondEventObservableSource = new EventObservableSource<int>();

        var combinedEventObserver = EventObservable.Combine(
            firstEventObservableSource.Observable,
            secondEventObservableSource.Observable
        );

        combinedEventObserver.OnEach(observedCombinedNumbers.Add);

        firstEventObservableSource.Emit(9);

        Assert.HasCount(1, observedCombinedNumbers);
        Assert.AreEqual(9, observedCombinedNumbers[0]);

        observedCombinedNumbers.Clear();

        combinedEventObserver.Dispose();

        firstEventObservableSource.Emit(8);

        Assert.IsEmpty(observedCombinedNumbers);
    }

    [TestMethod]
    public void Combine_Args_With_SubTypes()
    {
        var observedArgs = new List<object?>();

        var firstEventObservableSource = new EventObservableSource<Parent>();
        var secondEventObservableSource = new EventObservableSource<Child>();

        var firstCombinedEventObserver = EventObservable.Combine(
            firstEventObservableSource.Observable,
            secondEventObservableSource.Observable
        );

        firstCombinedEventObserver.OnEach(observedArgs.Add);

        firstEventObservableSource.Emit(new Parent());
        secondEventObservableSource.Emit(new Child());

        Assert.HasCount(2, observedArgs);
        Assert.IsInstanceOfType<Parent>(observedArgs[0]);
        Assert.IsInstanceOfType<Child>(observedArgs[1]);

        observedArgs.Clear();
        firstCombinedEventObserver.Dispose();

        var secondCombinedEventObserver = EventObservable.Combine(
            firstEventObservableSource.Observable,
            secondEventObservableSource.Observable
        );

        secondCombinedEventObserver.OnEach(observedArgs.Add);

        firstEventObservableSource.Emit(new Parent());
        secondEventObservableSource.Emit(new Child());

        Assert.HasCount(2, observedArgs);
        Assert.IsInstanceOfType<Parent>(observedArgs[0]);
        Assert.IsInstanceOfType<Child>(observedArgs[1]);

        observedArgs.Clear();
        secondCombinedEventObserver.Dispose();
    }

    #region Types

    private class Parent
    {
    }

    private class Child : Parent
    {
    }

    #endregion
}
