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
public class DedicatedThreadDispatcherTests
{
    [TestMethod]
    public void Run_Method_Is_Thread_Safe_And_Does_Not_Deadlock()
    {
        var counter = 0;
        var loopCount = 10;
        var runPerLoopCount = 10;
        var handles = new List<List<EventWaitHandle>>();
        var nameSet = new HashSet<string?>();

        // Create handles
        //
        for (var loopIndex = 0; loopIndex < loopCount; loopIndex++)
        {
            var list = new List<EventWaitHandle>();
            handles.Add(list);

            for (var runIndex = 0; runIndex < runPerLoopCount; runIndex++)
                list.Add(new(false, EventResetMode.ManualReset));
        }

        using var dispatcher = new DedicatedThreadDispatcher();

        // Launch tasks
        //
        handles.ForEach(list =>
        {
            list.ForEach(handle =>
            {
                Task.Run(async () =>
                {
                    await dispatcher.RunAsync(() =>
                    {
                        counter++;
                        nameSet.Add(Thread.CurrentThread.Name);
                    });
                    handle.Set();
                });
            });
        });

        // Wait for handles
        //
        handles.ForEach(list =>
        {
            list.ForEach(handle =>
            {
                handle.WaitOne();
            });
        });

        Assert.AreEqual(loopCount * runPerLoopCount, counter);
        Assert.AreEqual(1, nameSet.Count);
        Assert.AreEqual("SingleFinite.Essentials.DedicatedThreadDispatcher", nameSet.First());
    }

    [TestMethod]
    public async Task Run_Method_Propogates_Exceptions_Correctly_When_Thrown()
    {
        var dispatcher = new DedicatedThreadDispatcher();

        // Make sure an uncaught exception doesn't bring down the app.
        //
        dispatcher.Run(() => throw new InvalidOperationException());

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            async () =>
            {
                await dispatcher.RunAsync(() => throw new InvalidOperationException());
            }
        );
    }

    [TestMethod]
    public async Task Run_Method_Throws_If_Disposed()
    {
        var count = 0;
        using var dispatcher = new DedicatedThreadDispatcher();

        (dispatcher as IDisposable).Dispose();

        await Assert.ThrowsExceptionAsync<ObjectDisposedException>(() => dispatcher.RunAsync(() => count++));
        Assert.AreEqual(0, count);

        // Make sure multiple calls to dispose don't cause an exception.
        //
        (dispatcher as IDisposable).Dispose();
    }

    [TestMethod]
    public async Task Run_Method_Supports_Nested_Invokation()
    {
        var count = 0;
        using var dispatcher = new DedicatedThreadDispatcher();

        await dispatcher.RunAsync(
            () => dispatcher.RunAsync(
                () => dispatcher.RunAsync(
                    async () => { await Task.Delay(25); count++; }
                )
            )
        );

        Assert.AreEqual(1, count);
    }

    [TestMethod]
    public void Exceptions_Are_Reported_On_Calling_Thread()
    {
        using var mainDispatcher = new DedicatedThreadDispatcher();

        var mainThreadId = -1;
        var backgroundThreadId = -1;
        var onErrorThreadId = -1;

        var eventWaitHandle = new EventWaitHandle(
            initialState: false,
            mode: EventResetMode.ManualReset
        );

        Exception? observedException = null;

        mainDispatcher.Run(
            func: async () =>
            {
                mainThreadId = Environment.CurrentManagedThreadId;
                await Task.Run(() =>
                {
                    backgroundThreadId = Environment.CurrentManagedThreadId;
                    Thread.Sleep(100);
                    throw new InvalidOperationException("Test Exception");
                });
            },
            onError: ex =>
            {
                onErrorThreadId = Environment.CurrentManagedThreadId;
                observedException = ex;
                eventWaitHandle.Set();
            }
        );

        eventWaitHandle.WaitOne();

        Assert.AreNotEqual(-1, mainThreadId);
        Assert.AreNotEqual(-1, backgroundThreadId);
        Assert.AreNotEqual(-1, onErrorThreadId);

        Assert.AreNotEqual(mainThreadId, backgroundThreadId);
        Assert.AreNotEqual(mainThreadId, onErrorThreadId);

        Assert.IsInstanceOfType<InvalidOperationException>(observedException);
        Assert.AreEqual("Test Exception", observedException.Message);
    }

    [TestMethod]
    public void Exceptions_Are_Reported_On_Dispatcher()
    {
        var eventWaitHandle = new EventWaitHandle(
            initialState: false,
            mode: EventResetMode.ManualReset
        );

        var observedExceptions = new List<Exception>();

        using var mainDispatcher = new DedicatedThreadDispatcher(
            onError: ex =>
            {
                observedExceptions.Add(ex);
                eventWaitHandle.Set();
            }
        );

        mainDispatcher.Run(
            func: async () =>
            {
                await Task.Run(() =>
                {
                    throw new InvalidOperationException("Test Exception");
                });
            }
        );

        eventWaitHandle.WaitOne();

        Assert.AreEqual(1, observedExceptions.Count);
        var observedException = observedExceptions.First();
        Assert.IsInstanceOfType<InvalidOperationException>(observedException);
        Assert.AreEqual("Test Exception", observedException.Message);
    }
}
