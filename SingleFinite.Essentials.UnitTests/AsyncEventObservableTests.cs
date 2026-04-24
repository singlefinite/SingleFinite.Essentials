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
public class AsyncEventObservableTests(TestContext testContext)
{
    [TestMethod]
    public async Task Observe_Method_Invokes_Callback_When_Event_Raised_Async()
    {
        var observedNumber = 0;
        var observableSource = new AsyncEventObservableSource<int>();
        observableSource.Observable
            .Observe()
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () => observedNumber = args,
                    cancellationToken: testContext.CancellationToken
                );
            });

        Assert.AreEqual(0, observedNumber);

        await observableSource.EmitAsync(81);

        Assert.AreEqual(81, observedNumber);
    }

    [TestMethod]
    public async Task Dispose_Method_Removes_Callback_Async()
    {
        var observedNumber = 0;
        var observableSource = new AsyncEventObservableSource<int>();
        var observer = observableSource.Observable
            .Observe()
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () => observedNumber = args,
                    cancellationToken: testContext.CancellationToken
                );
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
        var observableSource = new AsyncEventObservableSource<int>();
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

        var observableSource = new AsyncEventObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Debounce(
                dispatcher: dispatcher,
                delay: TimeSpan.FromSeconds(1)
            )
            .OnEach(args => observedNames.Add(args.Name));

        Assert.IsEmpty(observedNames);

        await observableSource.EmitAsync(new("One", 0));
        await observableSource.EmitAsync(new("Two", 0));
        await observableSource.EmitAsync(new("Three", 0));

        Assert.IsEmpty(observedNames);

        await Task.Delay(
            delay: TimeSpan.FromSeconds(1.1),
            cancellationToken: testContext.CancellationToken
        );

        Assert.HasCount(1, observedNames);
        Assert.AreEqual("Three", observedNames[0]);
    }

    [TestMethod]
    public async Task Combine_Emits_Through()
    {
        var observedNumbers = new List<int>();

        var firstEventObservableSource = new AsyncEventObservableSource<int>();
        var secondEventObservableSource = new AsyncEventObservableSource<int>();
        var combinedEventObserver = AsyncEventObservable.Combine(
            firstEventObservableSource.Observable,
            secondEventObservableSource.Observable
        );

        combinedEventObserver.OnEach(observedNumbers.Add);

        await firstEventObservableSource.EmitAsync(9);
        Assert.HasCount(1, observedNumbers);
        Assert.AreEqual(9, observedNumbers[0]);

        observedNumbers.Clear();

        await secondEventObservableSource.EmitAsync(11);
        Assert.HasCount(1, observedNumbers);
        Assert.AreEqual(11, observedNumbers[0]);
    }

    [TestMethod]
    public async Task Combine_Dispose_Will_Dispose_Combined_EventObservers()
    {
        var observedNumbers = new List<int>();

        var firstEventObservableSource = new AsyncEventObservableSource<int>();
        var secondEventObservableSource = new AsyncEventObservableSource<int>();

        var combinedEventObserver = AsyncEventObservable.Combine(
            firstEventObservableSource.Observable,
            secondEventObservableSource.Observable
        );

        combinedEventObserver.OnEach(observedNumbers.Add);

        combinedEventObserver.Dispose();

        await firstEventObservableSource.EmitAsync(9);
        Assert.IsEmpty(observedNumbers);
    }

    [TestMethod]
    public async Task Combine_Args_With_SubTypes()
    {
        var observedArgs = new List<object?>();

        var firstEventObservableSource = new AsyncEventObservableSource<Parent>();
        var secondEventObservableSource = new AsyncEventObservableSource<Child>();

        var firstCombinedEventObserver = AsyncEventObservable.Combine(
            firstEventObservableSource.Observable,
            secondEventObservableSource.Observable
        );

        firstCombinedEventObserver.OnEach(observedArgs.Add);

        await firstEventObservableSource.EmitAsync(new Parent());
        await secondEventObservableSource.EmitAsync(new Child());

        Assert.HasCount(2, observedArgs);
        Assert.IsInstanceOfType<Parent>(observedArgs[0]);
        Assert.IsInstanceOfType<Child>(observedArgs[1]);

        observedArgs.Clear();
        firstCombinedEventObserver.Dispose();

        var secondCombinedEventObserver = AsyncEventObservable.Combine(
            firstEventObservableSource.Observable,
            secondEventObservableSource.Observable
        );

        secondCombinedEventObserver.OnEach(observedArgs.Add);

        await firstEventObservableSource.EmitAsync(new Parent());
        await secondEventObservableSource.EmitAsync(new Child());

        Assert.HasCount(2, observedArgs);
        Assert.IsInstanceOfType<Parent>(observedArgs[0]);
        Assert.IsInstanceOfType<Child>(observedArgs[1]);

        observedArgs.Clear();
        secondCombinedEventObserver.Dispose();
    }

    #region Types

    private record ExampleArgs(
        string Name,
        int Age
    );

    private class Parent
    {
    }

    private class Child : Parent
    {
    }

    #endregion
}
