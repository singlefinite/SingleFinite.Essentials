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
public class TaskScopeTests(TestContext testContext)
{
    [TestMethod]
    public void Child_Is_Disposed_When_Parent_Is_Cancelled()
    {
        var parentScope = new TaskScope(
            dispatcher: new ThreadPoolDispatcher()
        );
        var childScope = parentScope.CreateChildScope();

        Assert.IsFalse(parentScope.IsDisposed);
        Assert.IsFalse(childScope.IsDisposed);

        parentScope.Dispose();

        Assert.IsTrue(parentScope.IsDisposed);
        Assert.IsTrue(childScope.IsDisposed);
    }

    [TestMethod]
    public void Parent_Is_Not_Disposed_When_Child_Is_Disposed()
    {
        var parentScope = new TaskScope(
            dispatcher: new ThreadPoolDispatcher()
        );
        var childScope = parentScope.CreateChildScope();

        Assert.IsFalse(parentScope.IsDisposed);
        Assert.IsFalse(childScope.IsDisposed);

        childScope.Dispose();

        Assert.IsFalse(parentScope.IsDisposed);
        Assert.IsTrue(childScope.IsDisposed);
    }

    [TestMethod]
    public async Task Provided_Token_Is_Cancelled_When_Scope_Is_Disposed()
    {
        var testFlag = false;
        var scope = new TaskScope(
            dispatcher: new ThreadPoolDispatcher()
        );

        var task = scope.RunAsync(
            function: async token =>
            {
                await Task.Delay(5000, token);
                testFlag = true;
            },
            cancellationToken: testContext.CancellationToken
        );

        scope.Dispose();

        await Assert.ThrowsExactlyAsync<TaskCanceledException>(() =>
            task.WaitAsync(testContext.CancellationToken)
        );

        Assert.IsFalse(testFlag);
    }

    [TestMethod]
    public async Task Unhandled_Exceptions_Are_Reported()
    {
        var observedUnhandledExceptions = new List<Exception>();

        var dispatcher = new ThreadPoolDispatcher();

        var observer = Dispatcher.UnhandledDispatcherException
            .Observe()
            .Where(args => args.Dispatcher == dispatcher)
            .Select(args => args.Exception)
            .OnEach(observedUnhandledExceptions.Add);

        var scope = new TaskScope(
            dispatcher: dispatcher
        );

        scope.Run(
            action: () => throw new InvalidOperationException("Test Exception"),
            cancellationToken: testContext.CancellationToken
        );

        await Task.Delay(50, testContext.CancellationToken);

        observer.Dispose();

        Assert.ContainsSingle(observedUnhandledExceptions);
        Assert.IsInstanceOfType<InvalidOperationException>(
            observedUnhandledExceptions[0]
        );
        Assert.AreEqual(
            "Test Exception",
            observedUnhandledExceptions[0].Message
        );
    }

    [TestMethod]
    public async Task Handled_Exceptions_Are_Not_Reported()
    {
        var observedUnhandledExceptions = new List<Exception>();

        var dispatcher = new ThreadPoolDispatcher();

        var observer = Dispatcher.UnhandledDispatcherException
            .Observe()
            .Where(args => args.Dispatcher == dispatcher)
            .Select(args => args.Exception)
            .OnEach(observedUnhandledExceptions.Add);

        var scope = new TaskScope(
            dispatcher: dispatcher
        );

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            scope.RunAsync(
                function: async () =>
                {
                    throw new InvalidOperationException("Test Exception");
#pragma warning disable CS0162 // Unreachable code detected
                    return 0;
#pragma warning restore CS0162 // Unreachable code detected
                },
                cancellationToken: testContext.CancellationToken
            )
        );

        observer.Dispose();

        Assert.IsEmpty(observedUnhandledExceptions);
    }

    [TestMethod]
    public async Task Handled_Cancel_Exceptions_Are_Not_Reported()
    {
        var observedUnhandledExceptions = new List<Exception>();

        var dispatcher = new ThreadPoolDispatcher();

        var observer = Dispatcher.UnhandledDispatcherException
            .Observe()
            .Where(args => args.Dispatcher == dispatcher)
            .Select(args => args.Exception)
            .OnEach(observedUnhandledExceptions.Add);

        var scope = new TaskScope(
            dispatcher: dispatcher
        );

        scope.Run(
            function: async token => await Task.Delay(5000, token),
            cancellationToken: testContext.CancellationToken
        );

        scope.Dispose();

        await Task.Delay(50, testContext.CancellationToken);

        observer.Dispose();

        Assert.IsEmpty(observedUnhandledExceptions);
    }
}
