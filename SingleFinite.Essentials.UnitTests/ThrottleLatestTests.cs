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
public class ThrottleLatestTests
{
    [TestMethod]
    public async Task Testing()
    {
        var observedThreadId = -1;
        var taskThreadId = -1;
        var otherThreadId = -1;
        var eventWaitHandle = new ManualResetEvent(false);

        Task? task = null;

        await Task.Run(() =>
        {
            taskThreadId = Environment.CurrentManagedThreadId;

            var tcs = new TaskCompletionSource<int>();
            tcs.Task.ContinueWith(_ =>
            {
                observedThreadId = Environment.CurrentManagedThreadId;
                eventWaitHandle.Set();
            });

            task = tcs.Task;

            Task.Run(() =>
            {
                Task.Delay(250);
                otherThreadId = Environment.CurrentManagedThreadId;
                tcs.SetResult(1);
            });
        });

        if (task != null)
            await task;

        eventWaitHandle.WaitOne();

        Assert.AreNotEqual(-1, observedThreadId);
        Assert.AreNotEqual(-1, taskThreadId);
        Assert.AreNotEqual(-1, otherThreadId);

        Assert.AreEqual(taskThreadId, observedThreadId);
        Assert.AreNotEqual(taskThreadId, otherThreadId);
    }

    [TestMethod]
    public async Task Throttle_Invokes_Last_Action()
    {
        var observedItems = new List<string>();
        var observedErrors = 0;
        var dispatcher = new DedicatedThreadDispatcher();
        var throttleLatest = new ThrottlerLatest();
        var limit = TimeSpan.FromMilliseconds(250);

        throttleLatest.Throttle(
            action: () => observedItems.Add("one"),
            limit: limit,
            dispatcher: dispatcher,
            onError: _ => observedErrors++
        );

        throttleLatest.Throttle(
            action: () => observedItems.Add("two"),
            limit: limit,
            dispatcher: dispatcher,
            onError: _ => observedErrors++
        );

        throttleLatest.Throttle(
            action: () => observedItems.Add("three"),
            limit: limit,
            dispatcher: dispatcher,
            onError: _ => observedErrors++
        );

        Assert.AreEqual(1, observedItems.Count);
        Assert.AreEqual("one", observedItems[0]);

        await Task.Delay(1000);

        Assert.AreEqual(2, observedItems.Count);
        Assert.AreEqual("one", observedItems[0]);
        Assert.AreEqual("three", observedItems[1]);

        Assert.AreEqual(0, observedErrors);
    }

    [TestMethod]
    public async Task Throttle_With_Default_Dispatcher_Invokes_Last_Action()
    {
        var observedItems = new List<string>();
        var throttleLatest = new ThrottlerLatest();
        var limit = TimeSpan.FromMilliseconds(250);

        throttleLatest.Throttle(
            action: () => observedItems.Add("one"),
            limit: limit
        );

        throttleLatest.Throttle(
            action: () => observedItems.Add("two"),
            limit: limit
        );

        throttleLatest.Throttle(
            action: () => observedItems.Add("three"),
            limit: limit
        );

        Assert.AreEqual(1, observedItems.Count);
        Assert.AreEqual("one", observedItems[0]);

        await Task.Delay(1000);

        Assert.AreEqual(2, observedItems.Count);
        Assert.AreEqual("one", observedItems[0]);
        Assert.AreEqual("three", observedItems[1]);
    }
}
