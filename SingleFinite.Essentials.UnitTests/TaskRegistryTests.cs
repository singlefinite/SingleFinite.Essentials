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

using SingleFinite.Essentials.Internal;

namespace SingleFinite.Essentials.UnitTests;

[TestClass]
public class TaskRegistryTests(TestContext testContext)
{
    [TestMethod]
    public async Task Close_When_Empty_Completes_Task()
    {
        var registry = new TaskRegistry();

        Assert.IsFalse(registry.Task.IsCompleted);

        registry.Close();

        Assert.IsTrue(registry.Task.IsCompleted);
    }

    [TestMethod]
    public async Task Close_Completes_Task()
    {
        var signal = new Signal();
        var registry = new TaskRegistry();

        Assert.IsFalse(registry.Task.IsCompleted);

        var task1 = Task.Run(
            function: async () => await signal.WatchAsync(),
            cancellationToken: testContext.CancellationToken
        );

        var task2 = Task.Run(
            function: async () => await signal.WatchAsync(),
            cancellationToken: testContext.CancellationToken
        );

        Assert.IsTrue(registry.Register(task1));
        Assert.IsTrue(registry.Register(task2));

        await Task.Delay(10, testContext.CancellationToken);

        Assert.IsFalse(registry.Task.IsCompleted);

        registry.Close();

        await Task.Delay(10, testContext.CancellationToken);

        Assert.IsFalse(registry.Task.IsCompleted);

        signal.Set();

        await registry.Task;

        Assert.IsTrue(registry.Task.IsCompleted);
    }
}
