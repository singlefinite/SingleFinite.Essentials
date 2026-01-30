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
public class DisposeStateTests
{
    [TestMethod]
    public void ThrowIfDisposed_Throws_If_Disposed()
    {
        var disposeState = new DisposeState(new ExampleDisposable());

        disposeState.ThrowIfDisposed();

        disposeState.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(
            disposeState.ThrowIfDisposed
        );

        Assert.Contains(typeof(ExampleDisposable).FullName ?? "", ex.Message);
    }

    [TestMethod]
    public void IsDisposed_Changes_When_Disposed()
    {
        var observedIsChangedCount = 0;

        var disposeState = new DisposeState(new ExampleDisposable());
        disposeState.Disposed
            .Observe()
            .OnEach(() => observedIsChangedCount++);

        Assert.IsFalse(disposeState.IsDisposed);
        Assert.AreEqual(0, observedIsChangedCount);
        Assert.IsFalse(disposeState.CancellationToken.IsCancellationRequested);

        disposeState.Dispose();

        Assert.IsTrue(disposeState.IsDisposed);
        Assert.AreEqual(1, observedIsChangedCount);
        Assert.IsTrue(disposeState.CancellationToken.IsCancellationRequested);

        disposeState.Dispose();

        Assert.IsTrue(disposeState.IsDisposed);
        Assert.AreEqual(1, observedIsChangedCount);
        Assert.IsTrue(disposeState.CancellationToken.IsCancellationRequested);
    }

    private sealed class ExampleDisposable
    {
    }
}
