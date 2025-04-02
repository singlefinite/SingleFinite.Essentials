namespace SingleFinite.Essentials.UnitTests;

[TestClass]
public class IDisposeObservableTests
{
    [TestMethod]
    public void Dispose_Calls_DisposeState_Dispose()
    {
        var exampleDisposable = new ExampleDisposable();

        Assert.IsFalse(exampleDisposable.IsDisposed);

        ((IDisposable)exampleDisposable).Dispose();

        Assert.IsTrue(exampleDisposable.IsDisposed);
    }

    [TestMethod]
    public void OnDispose_Is_Called_When_Disposed()
    {
        var exampleDisposable = new ExampleDisposable();

        Assert.AreEqual(0, exampleDisposable.ObservedDisposeCount);

        exampleDisposable.Dispose();

        Assert.AreEqual(1, exampleDisposable.ObservedDisposeCount);

        exampleDisposable.Dispose();

        Assert.AreEqual(1, exampleDisposable.ObservedDisposeCount);
    }

    private sealed class ExampleDisposable : IDisposable, IDisposeObservable
    {
        private readonly DisposeState _disposeState;

        public ExampleDisposable()
        {
            _disposeState = new(
                owner: this,
                onDispose: OnDispose
            );
        }

        public int ObservedDisposeCount { get; private set; }

        public bool IsDisposed => _disposeState.IsDisposed;

        public void Dispose() => _disposeState.Dispose();

        private void OnDispose()
        {
            ObservedDisposeCount++;
        }

        public Observable Disposed => _disposeState.Disposed;
    }
}
