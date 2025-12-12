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
public class ThreadPoolDispatcherTests
{
    [TestMethod]
    public void Cancel_Passed_In_Cancellation_Token_Cancels_Dispatcher_Provided_Cancellation_Token()
    {
        var counter = 0;
        var waitHandle = new ManualResetEvent(false);
        var startWaitHandle = new ManualResetEvent(false);
        using var cancellationTokenSource = new CancellationTokenSource();

        var dispatcher = new ThreadPoolDispatcher();

        dispatcher.Run(
            action: cancellationToken =>
            {
                startWaitHandle.Set();
                cancellationToken.WaitHandle.WaitOne();
                counter++;
                waitHandle.Set();
            },
            cancellationTokens: cancellationTokenSource.Token
        );

        startWaitHandle.WaitOne();
        cancellationTokenSource.Cancel();

        waitHandle.WaitOne();

        Assert.AreEqual(1, counter);
        Assert.IsTrue(cancellationTokenSource.Token.IsCancellationRequested);
        Assert.IsFalse(dispatcher.CancellationToken.IsCancellationRequested);
    }
}
