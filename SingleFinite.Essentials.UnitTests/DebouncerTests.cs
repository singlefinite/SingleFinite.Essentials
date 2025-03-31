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
public class DebouncerTests
{
    [TestMethod]
    public async Task Debounce_Works_As_Expected()
    {
        var observedNames = new List<string>();
        var observedErrors = 0;
        var dispatcher = new DedicatedThreadDispatcher();
        var debouncer = new Debouncer();

        debouncer.Debounce(
            action: () => observedNames.Add("One"),
            delay: TimeSpan.FromMilliseconds(100),
            dispatcher: dispatcher,
            onError: _ => observedErrors++
        );

        debouncer.Debounce(
            action: () => observedNames.Add("Two"),
            delay: TimeSpan.FromMilliseconds(100),
            dispatcher: dispatcher,
            onError: _ => observedErrors++
        );

        debouncer.Debounce(
            action: () => observedNames.Add("Three"),
            delay: TimeSpan.FromMilliseconds(100),
            dispatcher: dispatcher,
            onError: _ => observedErrors++
        );

        Assert.AreEqual(0, observedNames.Count);

        await Task.Delay(TimeSpan.FromMilliseconds(200));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Three", observedNames[0]);

        Assert.AreEqual(0, observedErrors);
    }

    [TestMethod]
    public async Task Debounce_With_Default_Dispatcher()
    {
        var observedNames = new List<string>();
        var dispatcher = new DedicatedThreadDispatcher();
        var debouncer = new Debouncer();

        debouncer.Debounce(
            action: () => observedNames.Add("One"),
            delay: TimeSpan.FromMilliseconds(100)
        );

        debouncer.Debounce(
            action: () => observedNames.Add("Two"),
            delay: TimeSpan.FromMilliseconds(100)
        );

        debouncer.Debounce(
            action: () => observedNames.Add("Three"),
            delay: TimeSpan.FromMilliseconds(100)
        );

        Assert.AreEqual(0, observedNames.Count);

        await Task.Delay(TimeSpan.FromMilliseconds(200));

        Assert.AreEqual(1, observedNames.Count);
        Assert.AreEqual("Three", observedNames[0]);
    }

    [TestMethod]
    public async Task Cancel_Ends_Debounce()
    {
        var observedNames = new List<string>();
        var observedErrors = 0;
        var dispatcher = new DedicatedThreadDispatcher();
        var debouncer = new Debouncer();

        debouncer.Debounce(
            action: () => observedNames.Add("One"),
            delay: TimeSpan.FromMilliseconds(100),
            dispatcher: dispatcher,
            onError: _ => observedErrors++
        );

        Assert.AreEqual(0, observedNames.Count);

        debouncer.Cancel();

        await Task.Delay(TimeSpan.FromMilliseconds(200));

        Assert.AreEqual(0, observedNames.Count);

        Assert.AreEqual(0, observedErrors);
    }

    [TestMethod]
    public async Task Dispose_Ends_Debounce()
    {
        var observedNames = new List<string>();
        var observedErrors = 0;
        var dispatcher = new DedicatedThreadDispatcher();
        var debouncer = new Debouncer();

        debouncer.Debounce(
            action: () => observedNames.Add("One"),
            delay: TimeSpan.FromMilliseconds(100),
            dispatcher: dispatcher,
            onError: _ => observedErrors++
        );

        Assert.AreEqual(0, observedNames.Count);

        (debouncer as IDisposable)?.Dispose();

        await Task.Delay(TimeSpan.FromMilliseconds(200));

        Assert.AreEqual(0, observedNames.Count);

        Assert.AreEqual(0, observedErrors);
    }
}
