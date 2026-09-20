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
    public void Child_Is_Canceled_When_Parent_Is_Cancelled()
    {
        var parentScope = new TaskScope(
            dispatcher: new ThreadPoolDispatcher()
        );
        var childScope = parentScope.CreateChildScope();

        Assert.IsFalse(parentScope.CancellationToken.IsCancellationRequested);
        Assert.IsFalse(childScope.CancellationToken.IsCancellationRequested);

        parentScope.Dispose();

        Assert.IsTrue(parentScope.CancellationToken.IsCancellationRequested);
        Assert.IsTrue(childScope.CancellationToken.IsCancellationRequested);
    }

    [TestMethod]
    public void Parent_Is_Not_Canceled_When_Child_Is_Disposed()
    {
        var parentScope = new TaskScope(
            dispatcher: new ThreadPoolDispatcher()
        );
        var childScope = parentScope.CreateChildScope();

        Assert.IsFalse(parentScope.CancellationToken.IsCancellationRequested);
        Assert.IsFalse(childScope.CancellationToken.IsCancellationRequested);

        childScope.Dispose();

        Assert.IsFalse(parentScope.CancellationToken.IsCancellationRequested);
        Assert.IsTrue(childScope.CancellationToken.IsCancellationRequested);
    }

    [TestMethod]
    public async Task Tasks_Are_Not_Run_After_Scope_Is_Canceled()
    {
        var testFlag = false;
        var scope = new TaskScope();

        scope.Cancel();

        var job = scope.Run(
            action: () => testFlag = true
        );

        await Task.Delay(25, testContext.CancellationToken);

        Assert.IsFalse(testFlag);
    }

    [TestMethod]
    public async Task TaskJob_Cancel_Will_Cancel_Task()
    {
        var testFlag = false;
        var scope = new TaskScope(
            dispatcher: new ThreadPoolDispatcher()
        );

        var job = scope.Run(
            function: async () =>
            {
                await Task.Delay(5000, TaskContext.CancellationToken);
                testFlag = true;
            }
        );

        scope.Dispose();

        await Assert.ThrowsExactlyAsync<TaskCanceledException>(() =>
            job.Task.WaitAsync(testContext.CancellationToken)
        );

        Assert.IsFalse(testFlag);
    }

    [TestMethod]
    public async Task TaskJob_Child_Cancel_Will_Not_Cancel_Parents()
    {
        var flag = false;
        var parentScope = new TaskScope(
            parentCancellationToken: testContext.CancellationToken
        );
        var firstChildScope = null as TaskScope;
        var firstChildJob = null as ITaskJob;
        var secondChildScope = null as TaskScope;
        var secondChildJob = null as ITaskJob;

        var parentJob = parentScope.Run(
            function: async () =>
            {
                firstChildScope = parentScope.CreateChildScope();
                firstChildJob = firstChildScope.Run(
                    function: async () =>
                    {
                        secondChildScope = firstChildScope.CreateChildScope();
                        secondChildJob = secondChildScope.Run(
                            function: async () =>
                            {
                                await Task.Delay(5000, TaskContext.CancellationToken);
                                flag = true;
                            }
                        );

                        await Task.Delay(50, testContext.CancellationToken);
                        secondChildScope.Cancel();
                    }
                );
            }
        );

        await Task.Delay(100, testContext.CancellationToken);

        Assert.IsNotNull(firstChildScope);
        Assert.IsNotNull(firstChildJob);
        Assert.IsNotNull(secondChildScope);
        Assert.IsNotNull(secondChildJob);

        Assert.IsFalse(flag);
        Assert.IsFalse(parentScope.CancellationToken.IsCancellationRequested);
        Assert.IsFalse(firstChildScope.CancellationToken.IsCancellationRequested);
        Assert.IsFalse(firstChildJob.CancellationToken.IsCancellationRequested);
        Assert.IsTrue(secondChildScope.CancellationToken.IsCancellationRequested);
        Assert.IsTrue(secondChildJob.CancellationToken.IsCancellationRequested);
    }

    [TestMethod]
    public async Task TaskJob_Parent_Cancel_Will_Cancel_Children()
    {
        var flag = false;
        var parentScope = new TaskScope(
            parentCancellationToken: testContext.CancellationToken
        );
        var firstChildScope = null as TaskScope;
        var firstChildJob = null as ITaskJob;
        var secondChildScope = null as TaskScope;
        var secondChildJob = null as ITaskJob;

        var parentJob = parentScope.Run(
            function: async () =>
            {
                firstChildScope = parentScope.CreateChildScope();
                firstChildJob = firstChildScope.Run(
                    function: async () =>
                    {
                        secondChildScope = firstChildScope.CreateChildScope();
                        secondChildJob = secondChildScope.Run(
                            function: async () =>
                            {
                                await Task.Delay(5000, TaskContext.CancellationToken);
                                flag = true;
                            }
                        );
                    }
                );
            }
        );

        await Task.Delay(50, testContext.CancellationToken);
        parentScope.Cancel();

        Assert.IsNotNull(firstChildScope);
        Assert.IsNotNull(firstChildJob);
        Assert.IsNotNull(secondChildScope);
        Assert.IsNotNull(secondChildJob);

        Assert.IsFalse(flag);
        Assert.IsTrue(parentScope.CancellationToken.IsCancellationRequested);
        Assert.IsTrue(firstChildScope.CancellationToken.IsCancellationRequested);
        Assert.IsTrue(firstChildJob.CancellationToken.IsCancellationRequested);
        Assert.IsTrue(secondChildScope.CancellationToken.IsCancellationRequested);
        Assert.IsTrue(secondChildJob.CancellationToken.IsCancellationRequested);
    }

    [TestMethod]
    public async Task TaskJob_HasResult_WhenCompleted()
    {
        var scope = new TaskScope(
            dispatcher: new ThreadPoolDispatcher()
        );

        var job = scope.Run(
            function: async () =>
            {
                await Task.Delay(25, testContext.CancellationToken);
                return 99;
            }
        );

        var result = await job.Task.WaitAsync(testContext.CancellationToken);

        Assert.AreEqual(99, result);
    }
}
