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
public class CurrentThreadDispatcherTests(TestContext testContext)
{
    [TestMethod]
    public async Task RunAsync_Invokes_On_Calling_Thread()
    {
        var dispatcher = new CurrentThreadDispatcher();

        var threadId1 = Environment.CurrentManagedThreadId;
        var observedThreadId1 = -1;
        await dispatcher.RunAsync(
            function: () =>
            {
                observedThreadId1 = Environment.CurrentManagedThreadId;
                return Task.FromResult(0);
            },
            cancellationToken: testContext.CancellationToken
        );
        Assert.AreEqual(threadId1, observedThreadId1);

        await Task.Run(
            function: async () =>
            {
                var threadId2 = Environment.CurrentManagedThreadId;
                var observedThreadId2 = -1;
                await dispatcher.RunAsync(
                    function: () =>
                    {
                        observedThreadId2 = Environment.CurrentManagedThreadId;
                        return Task.FromResult(0);
                    },
                    cancellationToken: testContext.CancellationToken
                );
                Assert.AreEqual(threadId2, observedThreadId2);
            },
            cancellationToken: testContext.CancellationToken
        );
    }
}
