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
public class AsyncObserverTests(TestContext testContext)
{
    [TestMethod]
    public async Task ForEach_Runs_As_Expected_Async()
    {
        var observedCount = 0;

        var observableSource = new AsyncObservableSource();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(async () =>
            {
                await Task.Run(
                    action: () => observedCount++,
                    cancellationToken: testContext.CancellationToken
                );
            });

        Assert.AreEqual(0, observedCount);

        await observableSource.EmitAsync();

        Assert.AreEqual(1, observedCount);
        observedCount = 0;

        observer.OnEach(async () =>
        {
            await Task.Run(
                action: () => observedCount++,
                cancellationToken: testContext.CancellationToken
            );
        });

        Assert.AreEqual(0, observedCount);

        await observableSource.EmitAsync();

        Assert.AreEqual(2, observedCount);
        observedCount = 0;

        observer.Dispose();

        await observableSource.EmitAsync();

        Assert.AreEqual(0, observedCount);
    }

    [TestMethod]
    public async Task Select_Runs_As_Expected_Async()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Select(args => Task.Run(() => args.Name))
            .OnEach(async name =>
            {
                await Task.Run(
                    action: () => observedNames.Add(name),
                    cancellationToken: testContext.CancellationToken
                );
            });

        Assert.IsEmpty(observedNames);

        await observableSource.EmitAsync(new("Hello", 0));

        Assert.HasCount(1, observedNames);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        await observableSource.EmitAsync(new("World", 0));

        Assert.HasCount(1, observedNames);
        Assert.AreEqual("World", observedNames[0]);
        observedNames.Clear();

        observer.Dispose();
        await observableSource.EmitAsync(new("Again", 0));

        Assert.IsEmpty(observedNames);
    }

    [TestMethod]
    public async Task SelectNone_Runs_As_Expected_Async()
    {
        var observedNamesCount = 0;

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Select()
            .OnEach(() => observedNamesCount++);

        Assert.AreEqual(0, observedNamesCount);

        await observableSource.EmitAsync(new("Hello", 0));

        Assert.AreEqual(1, observedNamesCount);

        await observableSource.EmitAsync(new("World", 0));

        Assert.AreEqual(2, observedNamesCount);

        observer.Dispose();
        await observableSource.EmitAsync(new("Again", 0));

        Assert.AreEqual(2, observedNamesCount);
    }

    [TestMethod]
    public async Task Where_Runs_As_Expected_Async()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Where(args => Task.Run(() => args.Name.Length > 3))
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () => observedNames.Add(args.Name),
                    cancellationToken: testContext.CancellationToken
                );
            });

        Assert.IsEmpty(observedNames);

        await observableSource.EmitAsync(new("Hello", 0));

        Assert.HasCount(1, observedNames);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        await observableSource.EmitAsync(new("Hi", 0));

        Assert.IsEmpty(observedNames);

        observer.Dispose();

        await observableSource.EmitAsync(new("Howdy", 0));

        Assert.IsEmpty(observedNames);
    }

    [TestMethod]
    public async Task Until_Runs_As_Expected_Async()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () => observedNames.Add(args.Name),
                    cancellationToken: testContext.CancellationToken
                );
            })
            .Until(args => Task.Run(
                function: () => args.Name == "stop",
                cancellationToken: testContext.CancellationToken
            ))
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () => observedNames.Add(args.Name),
                    cancellationToken: testContext.CancellationToken
                );
            });

        Assert.IsEmpty(observedNames);

        await observableSource.EmitAsync(new("Hello", 0));

        Assert.HasCount(2, observedNames);
        Assert.AreEqual("Hello", observedNames[0]);
        Assert.AreEqual("Hello", observedNames[1]);
        observedNames.Clear();

        await observableSource.EmitAsync(new("stop", 0));

        Assert.HasCount(1, observedNames);
        Assert.AreEqual("stop", observedNames[0]);
        observedNames.Clear();

        await observableSource.EmitAsync(new("World", 0));

        Assert.IsEmpty(observedNames);
    }

    [TestMethod]
    public async Task On_DisposeObservable_Runs_As_Expected_Async()
    {
        var disposable = new ExampleDisposable();

        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(args => observedNames.Add(args.Name))
            .On(disposable);

        Assert.IsEmpty(observedNames);

        await observableSource.EmitAsync(new("Hello", 0));

        Assert.HasCount(1, observedNames);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        disposable.Dispose();

        await observableSource.EmitAsync(new("World", 0));

        Assert.IsEmpty(observedNames);
    }

    [TestMethod]
    public async Task On_CancellationToken_Runs_As_Expected_Async()
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(args => observedNames.Add(args.Name))
            .On(cancellationTokenSource.Token);

        Assert.IsEmpty(observedNames);

        await observableSource.EmitAsync(new("Hello", 0));

        Assert.HasCount(1, observedNames);
        Assert.AreEqual("Hello", observedNames[0]);
        observedNames.Clear();

        cancellationTokenSource.Cancel();

        await observableSource.EmitAsync(new("World", 0));

        Assert.IsEmpty(observedNames);
    }

    [TestMethod]
    public async Task Until_Continue_On_Disposed_Runs_As_Expected_Async()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () => observedNames.Add(args.Name),
                    cancellationToken: testContext.CancellationToken
                );
            })
            .Until(
                args => Task.Run(
                    function: () => args.Name == "stop",
                    cancellationToken: testContext.CancellationToken
                ),
                continueOnDispose: true
            )
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () => observedNames.Add($"{args.Name}!"),
                    cancellationToken: testContext.CancellationToken
                );
            });

        Assert.IsEmpty(observedNames);

        await observableSource.EmitAsync(new("Hello", 0));

        Assert.HasCount(2, observedNames);
        Assert.AreEqual("Hello", observedNames[0]);
        Assert.AreEqual("Hello!", observedNames[1]);
        observedNames.Clear();

        await observableSource.EmitAsync(new("stop", 0));

        Assert.HasCount(2, observedNames);
        Assert.AreEqual("stop", observedNames[0]);
        Assert.AreEqual("stop!", observedNames[1]);
        observedNames.Clear();

        await observableSource.EmitAsync(new("World", 0));

        Assert.IsEmpty(observedNames);
    }

    [TestMethod]
    public async Task Of_Type_Runs_As_Expected_Async()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OfType<SubExampleArgs>()
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () => observedNames.Add(args.SubName),
                    cancellationToken: testContext.CancellationToken
                );
            });

        Assert.IsEmpty(observedNames);

        await observableSource.EmitAsync(new("Hello", 0));

        Assert.IsEmpty(observedNames);

        await observableSource.EmitAsync(new SubExampleArgs(SubName: "Hi"));

        Assert.HasCount(1, observedNames);
        Assert.AreEqual("Hi", observedNames[0]);
    }

    [TestMethod]
    public async Task Once_Runs_As_Expected_Async()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () => observedNames.Add(args.Name),
                    cancellationToken: testContext.CancellationToken
                );
            })
            .Once()
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () => observedNames.Add($"{args.Name}!"),
                    cancellationToken: testContext.CancellationToken
                );
            });

        Assert.IsEmpty(observedNames);

        await observableSource.EmitAsync(new("Hello", 0));

        Assert.HasCount(2, observedNames);
        Assert.AreEqual("Hello", observedNames[0]);
        Assert.AreEqual("Hello!", observedNames[1]);
        observedNames.Clear();

        await observableSource.EmitAsync(new("World", 0));

        Assert.IsEmpty(observedNames);
    }

    [TestMethod]
    public async Task Dispatch_Runs_As_Expected()
    {
        var dispatcherThreadId = -1;
        var dispatcher = new DedicatedThreadDispatcher();
        await dispatcher.RunAsync(
            action: () => dispatcherThreadId = Environment.CurrentManagedThreadId,
            cancellationTokens: testContext.CancellationToken
        );

        var currentThreadId = Environment.CurrentManagedThreadId;

        var observedThreadIdsBeforeDispatch = new List<int>();
        var observedThreadIdsAfterDispatch = new List<int>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .OnEach(_ => observedThreadIdsBeforeDispatch.Add(Environment.CurrentManagedThreadId))
            .Dispatch(dispatcher)
            .OnEach(_ => observedThreadIdsAfterDispatch.Add(Environment.CurrentManagedThreadId));

        Assert.IsEmpty(observedThreadIdsAfterDispatch);

        await observableSource.EmitAsync(new("Hello", 0));

        Assert.HasCount(1, observedThreadIdsBeforeDispatch);
        Assert.AreEqual(currentThreadId, observedThreadIdsBeforeDispatch[0]);

        Assert.HasCount(1, observedThreadIdsAfterDispatch);
        Assert.AreEqual(dispatcherThreadId, observedThreadIdsAfterDispatch[0]);
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
                delay: TimeSpan.FromSeconds(1),
                dispatcher: dispatcher
            )
            .OnEach(async args =>
            {
                await Task.Delay(
                    millisecondsDelay: 5,
                    cancellationToken: testContext.CancellationToken
                );
                observedNames.Add(args.Name);
            });

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
    public async Task Throttle_Runs_AsExpected()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Throttle(
                limit: TimeSpan.FromMilliseconds(500)
            )
            .OnEach(args => observedNames.Add(args.Name));

        Assert.IsEmpty(observedNames);

        await observableSource.EmitAsync(new("One", 0));
        await observableSource.EmitAsync(new("Two", 0));
        await observableSource.EmitAsync(new("Three", 0));

        await Task.Delay(
            delay: TimeSpan.FromSeconds(1),
            cancellationToken: testContext.CancellationToken
        );

        await observableSource.EmitAsync(new("Four", 0));
        await observableSource.EmitAsync(new("Five", 0));
        await observableSource.EmitAsync(new("Six", 0));

        Assert.HasCount(2, observedNames);
        Assert.AreEqual("One", observedNames[0]);
        Assert.AreEqual("Four", observedNames[1]);
    }

    [TestMethod]
    public async Task ThrottleLatest_Runs_AsExpected()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;
        var dispatcher = new DedicatedThreadDispatcher();

        var observer = observable
            .Observe()
            .ThrottleLatest(
                limit: TimeSpan.FromMilliseconds(500),
                dispatcher: dispatcher
            )
            .OnEach(args => observedNames.Add(args.Name));

        Assert.IsEmpty(observedNames);

        await observableSource.EmitAsync(new("One", 0));
        await observableSource.EmitAsync(new("Two", 0));
        await observableSource.EmitAsync(new("Three", 0));

        Assert.HasCount(1, observedNames);
        Assert.AreEqual("One", observedNames[0]);

        await Task.Delay(
            delay: TimeSpan.FromSeconds(1),
            cancellationToken: testContext.CancellationToken
        );

        Assert.HasCount(2, observedNames);
        Assert.AreEqual("One", observedNames[0]);
        Assert.AreEqual("Three", observedNames[1]);
    }

    [TestMethod]
    public async Task Catch_Catches_Exceptions_Async()
    {
        var observedExceptions = new List<Exception>();
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Catch(async (_, ex) =>
            {
                await Task.Run(
                    action: () => observedExceptions.Add(ex),
                    cancellationToken: testContext.CancellationToken
                );
                return true;
            })
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () =>
                    {
                        if (args.Age == 99)
                            throw new InvalidOperationException("err");
                    },
                    cancellationToken: testContext.CancellationToken
                );
            })
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () => observedNames.Add(args.Name),
                    cancellationToken: testContext.CancellationToken
                );
            });

        await observableSource.EmitAsync(new ExampleArgs("Hello", 99));

        Assert.HasCount(1, observedExceptions);
        Assert.IsInstanceOfType<InvalidOperationException>(observedExceptions[0]);
        Assert.IsEmpty(observedNames);

        observedExceptions.Clear();
        observedNames.Clear();

        await observableSource.EmitAsync(new ExampleArgs("Hi", 10));

        Assert.IsEmpty(observedExceptions);
        Assert.HasCount(1, observedNames);
        Assert.AreEqual("Hi", observedNames[0]);
    }

    [TestMethod]
    public async Task Catch_Moves_Past_When_Not_Handled_Async()
    {
        var observedUnhandledExceptions = new List<Exception>();
        var observedExceptions = new List<Exception>();
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Catch(async (_, ex) =>
            {
                await Task.Run(
                    action: () => observedUnhandledExceptions.Add(ex),
                    cancellationToken: testContext.CancellationToken
                );
                return true;
            })
            .Catch((args, ex) =>
            {
                return Task.Run(
                    function: () =>
                    {
                        observedExceptions.Add(ex);
                        return args.Age > 50;
                    },
                    cancellationToken: testContext.CancellationToken
                );
            })
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () =>
                    {
                        if (args.Age == 99 || args.Age == 11)
                            throw new InvalidOperationException("err");
                    },
                    cancellationToken: testContext.CancellationToken
                );
            })
            .OnEach(async args =>
            {
                await Task.Run(
                    action: () => observedNames.Add(args.Name),
                    cancellationToken: testContext.CancellationToken
                );
            });

        await observableSource.EmitAsync(new ExampleArgs("Hello", 99));

        Assert.IsEmpty(observedUnhandledExceptions);
        Assert.HasCount(1, observedExceptions);
        Assert.IsInstanceOfType<InvalidOperationException>(observedExceptions[0]);
        Assert.IsEmpty(observedNames);

        observedUnhandledExceptions.Clear();
        observedExceptions.Clear();
        observedNames.Clear();

        await observableSource.EmitAsync(new ExampleArgs("World", 11));

        Assert.HasCount(1, observedUnhandledExceptions);
        Assert.IsInstanceOfType<InvalidOperationException>(observedUnhandledExceptions[0]);
        Assert.HasCount(1, observedExceptions);
        Assert.IsInstanceOfType<InvalidOperationException>(observedExceptions[0]);
        Assert.IsEmpty(observedNames);

        observedUnhandledExceptions.Clear();
        observedExceptions.Clear();
        observedNames.Clear();

        await observableSource.EmitAsync(new ExampleArgs("Hi", 10));

        Assert.IsEmpty(observedUnhandledExceptions);
        Assert.IsEmpty(observedExceptions);
        Assert.HasCount(1, observedNames);
        Assert.AreEqual("Hi", observedNames[0]);
    }

    [TestMethod]
    public async Task Limit_Limits_The_Number_Of_Executing_Observers()
    {
        var observedNames = new List<string>();

        var observableSource = new AsyncObservableSource<ExampleArgs>();
        var observable = observableSource.Observable;

        var observer = observable
            .Observe()
            .Limit(
                maxConcurrent: 1,
                maxBuffer: 0
            )
            .OnEach(async args =>
            {
                await Task.Delay(
                    millisecondsDelay: 500,
                    cancellationToken: testContext.CancellationToken
                );
                observedNames.Add(args.Name);
            });

        Assert.IsEmpty(observedNames);

        var firstEventTask = observableSource.EmitAsync(new("Hello", 0));
        var secondEventTask = observableSource.EmitAsync(new("World", 0));

        await firstEventTask;
        await secondEventTask;

        Assert.HasCount(1, observedNames);
        Assert.AreEqual("Hello", observedNames[0]);
    }

    [TestMethod]
    public async Task ToSync_Observes()
    {
        var observedCount = 0;

        var observableSource = new AsyncObservableSource();
        var observer = observableSource.Observable
            .Observe()
            .ToSync()
            .OnEach(() => observedCount++);

        Assert.AreEqual(0, observedCount);

        await observableSource.EmitAsync();

        Assert.AreEqual(1, observedCount);

        observer.Dispose();
        await observableSource.EmitAsync();

        Assert.AreEqual(1, observedCount);
    }

    [TestMethod]
    public async Task ToSync_WithArgs_Observes()
    {
        var observedText = "";

        var observableSource = new AsyncObservableSource<String>();
        var observer = observableSource.Observable
            .Observe()
            .ToSync()
            .OnEach(text => observedText = text);

        Assert.AreEqual("", observedText);

        await observableSource.EmitAsync("Hello");

        Assert.AreEqual("Hello", observedText);

        observer.Dispose();
        await observableSource.EmitAsync("World");

        Assert.AreEqual("Hello", observedText);
    }

    [TestMethod]
    public async Task ToObservable_Observes()
    {
        var observed1Count = 0;
        var observed2Count = 0;

        var observableSource = new AsyncObservableSource();

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

        await observableSource.EmitAsync();

        Assert.AreEqual(1, observed1Count);
        Assert.AreEqual(1, observed2Count);

        observer2.Dispose();
        await observableSource.EmitAsync();

        Assert.AreEqual(2, observed1Count);
        Assert.AreEqual(1, observed2Count);
    }

    [TestMethod]
    public async Task ToObservable_Stops_Observing_When_Parent_Is_Disposed()
    {
        var observed1Count = 0;
        var observed2Count = 0;

        var observableSource = new AsyncObservableSource();

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

        await observableSource.EmitAsync();

        Assert.AreEqual(1, observed1Count);
        Assert.AreEqual(1, observed2Count);

        observer1.Dispose();
        await observableSource.EmitAsync();

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

    private class ExampleSender;

    private class SubExampleSender : ExampleSender;

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

    #endregion
}
