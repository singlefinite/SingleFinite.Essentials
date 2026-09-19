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
public class DedicatedThreadDispatcherTests(TestContext testContext)
{
    [TestMethod]
    public void Run_Method_Is_Thread_Safe_And_Does_Not_Deadlock()
    {
        var counter = 0;
        var loopCount = 10;
        var runPerLoopCount = 10;
        var handles = new List<List<EventWaitHandle>>();
        var nameSet = new HashSet<string?>();
        var scope = new TaskScope(
            parentCancellationToken: testContext.CancellationToken
        );

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
                Task.Run(
                    function: async () =>
                    {
                        await dispatcher.RunAsync(
                            function: () =>
                            {
                                counter++;
                                nameSet.Add(Thread.CurrentThread.Name);
                                return Task.FromResult(0);
                            },
                            context: scope
                        );
                        handle.Set();
                    },
                    cancellationToken: testContext.CancellationToken
                );
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
        Assert.HasCount(1, nameSet);
        Assert.AreEqual("SingleFinite.Essentials.DedicatedThreadDispatcher", nameSet.First());
    }

    [TestMethod]
    public async Task Run_Method_Propogates_Exceptions_Correctly_When_Thrown()
    {
        var scope = new TaskScope(
            parentCancellationToken: testContext.CancellationToken
        );

        // Make sure an uncaught exception doesn't bring down the app.
        //
        _ = scope.Dispatcher.RunAsync(
            function: () =>
            {
                throw new InvalidOperationException();
#pragma warning disable CS0162 // Unreachable code detected
                return Task.FromResult(0);
#pragma warning restore CS0162 // Unreachable code detected
            },
            context: scope
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await scope.Dispatcher.RunAsync(
                    function: () =>
                    {
                        throw new InvalidOperationException();
#pragma warning disable CS0162 // Unreachable code detected
                        return Task.FromResult(0);
#pragma warning restore CS0162 // Unreachable code detected
                    },
                    context: scope
                );
            }
        );
    }

    [TestMethod]
    public async Task Run_Method_Throws_If_Disposed()
    {
        var count = 0;
        var scope = new TaskScope(
            parentCancellationToken: testContext.CancellationToken
        );
        using var dispatcher = new DedicatedThreadDispatcher();

        dispatcher.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(
            () => dispatcher.RunAsync(
                function: () =>
                {
                    count++;
                    return Task.FromResult(0);
                },
                context: scope
            )
        );
        Assert.AreEqual(0, count);

        // Make sure multiple calls to dispose don't cause an exception.
        //
        dispatcher.Dispose();
    }

    [TestMethod]
    public async Task Run_Method_Supports_Nested_Invokation()
    {
        var count = 0;
        var scope = new TaskScope(
            parentCancellationToken: testContext.CancellationToken
        );
        using var dispatcher = new DedicatedThreadDispatcher();

        await dispatcher.RunAsync(
            function: () => dispatcher.RunAsync(
                function: () => dispatcher.RunAsync(
                    function: async () =>
                    {
                        await Task.Delay(
                            millisecondsDelay: 25,
                            cancellationToken: testContext.CancellationToken
                        );
                        count++;
                        return 0;
                    },
                    context: scope
                ),
                context: scope
            ),
            context: scope
        );

        Assert.AreEqual(1, count);
    }
}
