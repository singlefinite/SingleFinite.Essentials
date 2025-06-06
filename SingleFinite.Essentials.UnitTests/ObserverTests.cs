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
public class ObserverTests
{
    [TestMethod]
    public void ForEach_Runs_As_Expected()
    {
        var observedCount = 0;

        var observableSource = new ObservableSource();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(() => observedCount++);

        Assert.AreEqual(0, observedCount);

        observableSource.Emit();

        Assert.AreEqual(1, observedCount);
        observedCount = 0;

        observer.OnEach(() => observedCount++);

        Assert.AreEqual(0, observedCount);

        observableSource.Emit();

        Assert.AreEqual(2, observedCount);
        observedCount = 0;

        observer.Dispose();

        observableSource.Emit();

        Assert.AreEqual(0, observedCount);
    }

    [TestMethod]
    public void Select_Runs_As_Expected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Select(args => args.Name)
            .OnEach(observedNames.Add);

        Assert.AreEqual(0, observedNames.Count);

        observableSource.Emit(new("Hello", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        observableSource.Emit(new("World", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("World", observedNames[0]);
        observedNames.Clear();

        observer.Dispose();
        observableSource.Emit(new("Again", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public void Where_Runs_As_Expected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Where(args => args.Name.Length > 3)
            .OnEach(args => observedNames.Add(args.Name));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.Emit(new("Hello", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        observableSource.Emit(new("Hi", 0));

        Assert.AreEqual(0, observedNames.Count);

        observer.Dispose();

        observableSource.Emit(new("Howdy", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public void Until_Runs_As_Expected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(args => observedNames.Add(args.Name))
            .Until(args => args.Name == "stop")
            .OnEach(args => observedNames.Add(args.Name));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.Emit(new("Hello", 0));

        Assert.AreEqual(2, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        Assert.AreEqual("Hello", observedNames[1]);
        observedNames.Clear();

        observableSource.Emit(new("stop", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("stop", observedNames[0]);
        observedNames.Clear();

        observableSource.Emit(new("World", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public void Until_Continue_On_Disposed_Runs_As_Expected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(args => observedNames.Add(args.Name))
            .Until(args => args.Name == "stop", continueOnDispose: true)
            .OnEach(args => observedNames.Add($"{args.Name}!"));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.Emit(new("Hello", 0));

        Assert.AreEqual(2, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        Assert.AreEqual("Hello!", observedNames[1]);
        observedNames.Clear();

        observableSource.Emit(new("stop", 0));

        Assert.AreEqual(2, observedNames.Count);
        Assert.AreEqual("stop", observedNames[0]);
        Assert.AreEqual("stop!", observedNames[1]);
        observedNames.Clear();

        observableSource.Emit(new("World", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public void On_DisposeObservable_Runs_As_Expected()
    {
        var disposable = new ExampleDisposable();

        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(args => observedNames.Add(args.Name))
            .On(disposable);

        Assert.AreEqual(0, observedNames.Count);

        observableSource.Emit(new("Hello", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        disposable.Dispose();

        observableSource.Emit(new("World", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public void On_CancellationToken_Runs_As_Expected()
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(args => observedNames.Add(args.Name))
            .On(cancellationTokenSource.Token);

        Assert.AreEqual(0, observedNames.Count);

        observableSource.Emit(new("Hello", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        cancellationTokenSource.Cancel();

        observableSource.Emit(new("World", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public void Of_Type_Runs_As_Expected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OfType<SubExampleArgs>()
            .OnEach(args => observedNames.Add(args.SubName));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.Emit(new("Hello", 0));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.Emit(new SubExampleArgs(SubName: "Hi"));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hi", observedNames[0]);
    }

    [TestMethod]
    public void Once_Runs_As_Expected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(args => observedNames.Add(args.Name))
            .Once()
            .OnEach(args => observedNames.Add($"{args.Name}!"));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.Emit(new("Hello", 0));

        Assert.AreEqual(2, observedNames.Count);
        Assert.AreEqual("Hello", observedNames[0]);
        Assert.AreEqual("Hello!", observedNames[1]);
        observedNames.Clear();

        observableSource.Emit(new("World", 0));

        Assert.AreEqual(0, observedNames.Count);
    }

    [TestMethod]
    public async Task Dispatch_Runs_As_Expected()
    {
        var dispatcherThreadId = -1;
        var dispatcher = new DedicatedThreadDispatcher();
        await dispatcher.RunAsync(
            action: () => dispatcherThreadId = Environment.CurrentManagedThreadId
        );

        var currentThreadId = Environment.CurrentManagedThreadId;

        var observedThreadIdsBeforeDispatch = new List<int>();
        var observedThreadIdsAfterDispatch = new List<int>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var eventWaitHandle = new EventWaitHandle(
            initialState: false,
            mode: EventResetMode.ManualReset
        );

        var observer = observable
            .Observe()
            .OnEach(_ => observedThreadIdsBeforeDispatch.Add(Environment.CurrentManagedThreadId))
            .Dispatch(dispatcher)
            .OnEach(_ =>
            {
                observedThreadIdsAfterDispatch.Add(Environment.CurrentManagedThreadId);
                eventWaitHandle.Set();
            });

        Assert.AreEqual(0, observedThreadIdsAfterDispatch.Count);

        observableSource.Emit(new("Hello", 0));
        eventWaitHandle.WaitOne();

        Assert.AreEqual(1, observedThreadIdsBeforeDispatch.Count);
        Assert.AreEqual(currentThreadId, observedThreadIdsBeforeDispatch[0]);

        Assert.AreEqual(1, observedThreadIdsAfterDispatch.Count);
        Assert.AreEqual(dispatcherThreadId, observedThreadIdsAfterDispatch[0]);
    }

    [TestMethod]
    public async Task Debounce_Runs_AsExpected()
    {
        var observedNames = new List<string>();

        var observedErrors = 0;

        var dispatcher = new DedicatedThreadDispatcher();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Debounce(
                delay: TimeSpan.FromSeconds(1),
                dispatcher: dispatcher
            )
            .OnEach(args => observedNames.Add(args.Name));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.Emit(new("One", 0));
        observableSource.Emit(new("Two", 0));
        observableSource.Emit(new("Three", 0));

        Assert.AreEqual(0, observedNames.Count);

        await Task.Delay(TimeSpan.FromSeconds(1.1));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Three", observedNames[0]);

        Assert.AreEqual(0, observedErrors);
    }

    [TestMethod]
    public async Task Throttle_Runs_AsExpected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Throttle(
                limit: TimeSpan.FromMilliseconds(500)
            )
            .OnEach(args => observedNames.Add(args.Name));

        Assert.AreEqual(0, observedNames.Count);

        observableSource.Emit(new("One", 0));
        observableSource.Emit(new("Two", 0));
        observableSource.Emit(new("Three", 0));

        await Task.Delay(TimeSpan.FromSeconds(1));

        observableSource.Emit(new("Four", 0));
        observableSource.Emit(new("Five", 0));
        observableSource.Emit(new("Six", 0));

        Assert.AreEqual(2, observedNames.Count);
        Assert.AreEqual("One", observedNames[0]);
        Assert.AreEqual("Four", observedNames[1]);
    }

    [TestMethod]
    public async Task ThrottleLatest_Runs_AsExpected()
    {
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;
        var dispatcher = new DedicatedThreadDispatcher();

        var observer = observable
            .Observe()
            .ThrottleLatest(
                limit: TimeSpan.FromMilliseconds(500),
                dispatcher: dispatcher
            )
            .OnEach(args => observedNames.Add(args.Name)
            );

        Assert.AreEqual(0, observedNames.Count);

        observableSource.Emit(new("One", 0));
        observableSource.Emit(new("Two", 0));
        observableSource.Emit(new("Three", 0));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("One", observedNames[0]);

        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.AreEqual(2, observedNames.Count);
        Assert.AreEqual("One", observedNames[0]);
        Assert.AreEqual("Three", observedNames[1]);
    }

    [TestMethod]
    public void Catch_Catches_Exceptions()
    {
        var observedExceptions = new List<Exception>();
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Catch((_, ex) =>
            {
                observedExceptions.Add(ex);
                return true;
            })
            .OnEach(args =>
            {
                if (args.Age == 99)
                    throw new InvalidOperationException("err");
            })
            .OnEach(args => observedNames.Add(args.Name));

        observableSource.Emit(new ExampleArgs("Hello", 99));

        Assert.AreEqual(1, observedExceptions.Count);
        Assert.IsInstanceOfType<InvalidOperationException>(observedExceptions[0]);
        Assert.AreEqual(0, observedNames.Count);

        observedExceptions.Clear();
        observedNames.Clear();

        observableSource.Emit(new ExampleArgs("Hi", 10));

        Assert.AreEqual(0, observedExceptions.Count);
        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hi", observedNames[0]);
    }

    [TestMethod]
    public void Catch_Moves_Past_When_Not_Handled()
    {
        var observedUnhandledExceptions = new List<Exception>();
        var observedExceptions = new List<Exception>();
        var observedNames = new List<string>();

        var observableSource = new ObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Catch((_, ex) =>
            {
                observedUnhandledExceptions.Add(ex);
                return true;
            })
            .Catch((args, ex) =>
            {
                observedExceptions.Add(ex);
                return args.Age > 50;
            })
            .OnEach(args =>
            {
                if (args.Age == 99 || args.Age == 11)
                    throw new InvalidOperationException("err");
            })
            .OnEach(args => observedNames.Add(args.Name));

        observableSource.Emit(new ExampleArgs("Hello", 99));

        Assert.AreEqual(0, observedUnhandledExceptions.Count);
        Assert.AreEqual(1, observedExceptions.Count);
        Assert.IsInstanceOfType<InvalidOperationException>(observedExceptions[0]);
        Assert.AreEqual(0, observedNames.Count);

        observedUnhandledExceptions.Clear();
        observedExceptions.Clear();
        observedNames.Clear();

        observableSource.Emit(new ExampleArgs("World", 11));

        Assert.AreEqual(1, observedUnhandledExceptions.Count);
        Assert.IsInstanceOfType<InvalidOperationException>(observedUnhandledExceptions[0]);
        Assert.AreEqual(1, observedExceptions.Count);
        Assert.IsInstanceOfType<InvalidOperationException>(observedExceptions[0]);
        Assert.AreEqual(0, observedNames.Count);

        observedUnhandledExceptions.Clear();
        observedExceptions.Clear();
        observedNames.Clear();

        observableSource.Emit(new ExampleArgs("Hi", 10));

        Assert.AreEqual(0, observedUnhandledExceptions.Count);
        Assert.AreEqual(0, observedExceptions.Count);
        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Hi", observedNames[0]);
    }

    [TestMethod]
    public async Task ToAsync_Runs_As_Expected_Async()
    {
        var observedCount = 0;

        var observableSource = new ObservableSource();
        var observable = observableSource.Observable;
        var dispatcher = new DedicatedThreadDispatcher();

        var waitHandle = new ManualResetEvent(false);

        var observer = observable
            .Observe()
            .ToAsync(dispatcher)
            .OnEach(async () =>
            {
                await Task.Run(() => observedCount++);
                waitHandle.Set();
            });

        Assert.AreEqual(0, observedCount);

        observableSource.Emit();
        waitHandle.WaitOne();
        waitHandle.Reset();

        Assert.AreEqual(1, observedCount);
        observedCount = 0;

        observer.Dispose();

        observableSource.Emit();
        await Task.Delay(100);

        Assert.AreEqual(0, observedCount);
    }

    [TestMethod]
    public void ToAsync_Catches_Exceptions()
    {
        var observedCount = 0;
        var observedErrors = new List<Exception>();

        var observableSource = new ObservableSource();
        var observable = observableSource.Observable;
        var dispatcher = new ThreadPoolDispatcher();

        var waitHandle = new ManualResetEvent(false);

        var observer = observable
            .Observe()
            .ToAsync(
                dispatcher: dispatcher,
                onError: ex =>
                {
                    observedErrors.Add(ex);
                    waitHandle.Set();
                }
            )
            .OnEach(async () =>
            {
                await Task.Run(() => observedCount++);
                throw new InvalidOperationException("Test");
            });

        observableSource.Emit();

        waitHandle.WaitOne(1000);

        Assert.AreEqual(1, observedCount);
        Assert.AreEqual(1, observedErrors.Count);
        Assert.IsInstanceOfType<InvalidOperationException>(observedErrors[0]);
    }

    [TestMethod]
    public void Observer_Generic_Event_Runs_As_Expected()
    {
        var exampleObject = new ExampleWithEvent();
        var cancellationTokenSource = new CancellationTokenSource();

        var observedCount = 0;

        var observer = Observable
            .Observe<Action>(
                register: handler => exampleObject.ExampleEvent += handler,
                unregister: handler => exampleObject.ExampleEvent -= handler,
                handler: nextEvent => () => nextEvent()
            )
            .OnEach(() => observedCount++)
            .On(cancellationTokenSource.Token);

        Assert.AreEqual(0, observedCount);

        exampleObject.RaiseEvent();

        Assert.AreEqual(1, observedCount);

        cancellationTokenSource.Cancel();

        exampleObject.RaiseEvent();

        Assert.AreEqual(1, observedCount);
    }

    [TestMethod]
    public void ToObservable_Observes()
    {
        var observed1Count = 0;
        var observed2Count = 0;

        var observableSource = new ObservableSource();

        var observable1 = observableSource.Observable;
        var observer1 = observable1
            .Observe()
            .OnEach(() => observed1Count++);

        var observable2 = observer1.ToObservable();
        var observer2 = observable2
            .Observe()
            .OnEach(() => observed2Count++);

        Assert.AreEqual(0, observed1Count);
        Assert.AreEqual(0, observed2Count);

        observableSource.Emit();

        Assert.AreEqual(1, observed1Count);
        Assert.AreEqual(1, observed2Count);

        observer2.Dispose();
        observableSource.Emit();

        Assert.AreEqual(2, observed1Count);
        Assert.AreEqual(1, observed2Count);
    }

    [TestMethod]
    public void ToObservable_Stops_Observing_When_Parent_Is_Disposed()
    {
        var observed1Count = 0;
        var observed2Count = 0;

        var observableSource = new ObservableSource();

        var observable1 = observableSource.Observable;
        var observer1 = observable1
            .Observe()
            .OnEach(() => observed1Count++);

        var observable2 = observer1.ToObservable();
        var observer2 = observable2
            .Observe()
            .OnEach(() => observed2Count++);

        Assert.AreEqual(0, observed1Count);
        Assert.AreEqual(0, observed2Count);

        observableSource.Emit();

        Assert.AreEqual(1, observed1Count);
        Assert.AreEqual(1, observed2Count);

        observer1.Dispose();
        observableSource.Emit();

        Assert.AreEqual(1, observed1Count);
        Assert.AreEqual(1, observed2Count);
    }

    #region Types

    private record ExampleArgs(
        string Name,
        int Age
    );

    private record SubExampleArgs(
        string SubName
    ) : ExampleArgs(
        Name: "Sub",
        Age: 0
    );

    private class ExampleDisposable : IDisposable, IDisposeObservable
    {
        private readonly DisposeState _disposeState;

        public ExampleDisposable()
        {
            _disposeState = new(this);
        }

        public bool IsDisposed => _disposeState.IsDisposed;

        public void Dispose() => _disposeState.Dispose();

        public Observable Disposed => _disposeState.Disposed;
    }

    private class ExampleSender;

    private class SubExampleSender : ExampleSender;

    private class ExampleWithEvent
    {
        public void RaiseEvent() => ExampleEvent?.Invoke();

        public event Action? ExampleEvent;
    }

    #endregion
}
