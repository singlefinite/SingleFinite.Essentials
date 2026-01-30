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
public class ThrottleLatestTests(TestContext testContext)
{
    [TestMethod]
    public async Task Throttle_Invokes_Last_Action()
    {
        var observedItems = new List<string>();
        var dispatcher = new DedicatedThreadDispatcher();
        var throttleLatest = new ThrottlerLatest();
        var limit = TimeSpan.FromMilliseconds(250);

        throttleLatest.Throttle(
            action: () => observedItems.Add("one"),
            limit: limit,
            dispatcher: dispatcher
        );

        throttleLatest.Throttle(
            action: () => observedItems.Add("two"),
            limit: limit,
            dispatcher: dispatcher
        );

        throttleLatest.Throttle(
            action: () => observedItems.Add("three"),
            limit: limit,
            dispatcher: dispatcher
        );

        Assert.HasCount(1, observedItems);
        Assert.AreEqual("one", observedItems[0]);

        await Task.Delay(
            millisecondsDelay: 1000,
            cancellationToken: testContext.CancellationToken
        );

        Assert.HasCount(2, observedItems);
        Assert.AreEqual("one", observedItems[0]);
        Assert.AreEqual("three", observedItems[1]);
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

        Assert.HasCount(1, observedItems);
        Assert.AreEqual("one", observedItems[0]);

        await Task.Delay(
            millisecondsDelay: 1000,
            cancellationToken: testContext.CancellationToken
        );

        Assert.HasCount(2, observedItems);
        Assert.AreEqual("one", observedItems[0]);
        Assert.AreEqual("three", observedItems[1]);
    }
}
