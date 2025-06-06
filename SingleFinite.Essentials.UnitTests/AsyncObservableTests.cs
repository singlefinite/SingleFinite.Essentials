﻿// MIT License
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
public class AsyncObservableTests
{
    [TestMethod]
    public async Task Observe_Method_Invokes_Callback_When_Event_Raised_Async()
    {
        var observedNumber = 0;
        var observableSource = new AsyncObservableSource<int>();
        observableSource.Observable
            .Observe()
            .OnEach(async args =>
            {
                await Task.Run(() => observedNumber = args);
            });

        Assert.AreEqual(0, observedNumber);

        await observableSource.EmitAsync(81);

        Assert.AreEqual(81, observedNumber);
    }

    [TestMethod]
    public async Task Dispose_Method_Removes_Callback_Async()
    {
        var observedNumber = 0;
        var observableSource = new AsyncObservableSource<int>();
        var observer = observableSource.Observable
            .Observe()
            .OnEach(async args =>
            {
                await Task.Run(() => observedNumber = args);
            });

        Assert.AreEqual(0, observedNumber);

        await observableSource.EmitAsync(81);

        Assert.AreEqual(81, observedNumber);

        observer.Dispose();

        await observableSource.EmitAsync(99);

        Assert.AreEqual(81, observedNumber);
    }

    [TestMethod]
    public async Task Observe_Method_Raises_Event_When_Event_Raised_Async()
    {
        var observedNumber = 0;
        var observableSource = new AsyncObservableSource<int>();
        observableSource.Observable.Event += async args =>
        {
            await Task.Run(() => observedNumber = args);
        };

        Assert.AreEqual(0, observedNumber);

        await observableSource.EmitAsync(81);

        Assert.AreEqual(81, observedNumber);
    }

    [TestMethod]
    public async Task Debounce_Runs_AsExpected()
    {
        var observedNames = new List<string>();

        var dispatcher = new DedicatedThreadDispatcher();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Debounce(
                dispatcher: dispatcher,
                delay: TimeSpan.FromSeconds(1)
            )
            .OnEach(args => observedNames.Add(args.Name));

        Assert.AreEqual(0, observedNames.Count);

        await observableSource.EmitAsync(new("One", 0));
        await observableSource.EmitAsync(new("Two", 0));
        await observableSource.EmitAsync(new("Three", 0));

        Assert.AreEqual(0, observedNames.Count);

        await Task.Delay(TimeSpan.FromSeconds(1.1));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Three", observedNames[0]);
    }

    [TestMethod]
    public async Task Combine_Emits_Through()
    {
        var observedNumbers = new List<int>();

        var firstObservableSource = new AsyncObservableSource<int>();
        var secondObservableSource = new AsyncObservableSource<int>();
        var combinedObserver = AsyncObservable.Combine(
            firstObservableSource.Observable,
            secondObservableSource.Observable
        );

        combinedObserver.OnEach(observedNumbers.Add);

        await firstObservableSource.EmitAsync(9);
        Assert.AreEqual(1, observedNumbers.Count);
        Assert.AreEqual(9, observedNumbers[0]);

        observedNumbers.Clear();

        await secondObservableSource.EmitAsync(11);
        Assert.AreEqual(1, observedNumbers.Count);
        Assert.AreEqual(11, observedNumbers[0]);
    }

    [TestMethod]
    public async Task Combine_Continues_To_Emit_After_Single_Observer_Disposed()
    {
        var observedNumbers = new List<int>();

        var firstObservableSource = new AsyncObservableSource<int>();
        var secondObservableSource = new AsyncObservableSource<int>();

        var firstObserver = firstObservableSource.Observable.Observe();
        var secondObserver = secondObservableSource.Observable.Observe();

        var combinedObserver = AsyncObservable.Combine(
            firstObserver,
            secondObserver
        );

        combinedObserver.OnEach(observedNumbers.Add);

        firstObserver.Dispose();
        Assert.IsFalse(combinedObserver.IsDisposed);

        await firstObservableSource.EmitAsync(9);
        Assert.AreEqual(0, observedNumbers.Count);

        await secondObservableSource.EmitAsync(11);
        Assert.AreEqual(1, observedNumbers.Count);
        Assert.AreEqual(11, observedNumbers[0]);
    }

    [TestMethod]
    public void Combine_Dispose_Will_Dispose_Combined_Observers()
    {
        var firstObservableSource = new AsyncObservableSource<int>();
        var secondObservableSource = new AsyncObservableSource<int>();

        var firstObserver = firstObservableSource.Observable.Observe();
        var secondObserver = secondObservableSource.Observable.Observe();

        var combinedObserver = AsyncObservable.Combine(
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

    #region Types

    private record ExampleArgs(
        string Name,
        int Age
    );

    #endregion
}
