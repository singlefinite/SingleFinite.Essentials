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

        var ex = Assert.ThrowsException<ObjectDisposedException>(
            disposeState.ThrowIfDisposed
        );

        Assert.IsTrue(
            ex.Message.Contains(typeof(ExampleDisposable).FullName ?? "")
        );
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
