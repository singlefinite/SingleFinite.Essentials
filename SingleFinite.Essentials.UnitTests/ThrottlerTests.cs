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
public class ThrottlerTests
{
    [TestMethod]
    public async Task Throttle_Limits_Calls_Made_Async()
    {
        var observedItems = new List<string>();
        var throttler = new Throttler();
        var limit = TimeSpan.FromMicroseconds(100);

        throttler.Throttle(
            action: () => observedItems.Add("one"),
            limit: limit
        );

        throttler.Throttle(
            action: () => observedItems.Add("two"),
            limit: limit
        );

        throttler.Throttle(
            action: () => observedItems.Add("three"),
            limit: limit
        );

        throttler.Throttle(
            action: () => observedItems.Add("four"),
            limit: limit
        );

        await Task.Delay(100);

        throttler.Throttle(
            action: () => observedItems.Add("five"),
            limit: limit
        );

        throttler.Throttle(
            action: () => observedItems.Add("six"),
            limit: limit
        );

        throttler.Throttle(
            action: () => observedItems.Add("seven"),
            limit: limit
        );

        throttler.Throttle(
            action: () => observedItems.Add("eight"),
            limit: limit
        );

        Assert.AreEqual(2, observedItems.Count);
        Assert.AreEqual("one", observedItems[0]);
        Assert.AreEqual("five", observedItems[1]);
    }
}
