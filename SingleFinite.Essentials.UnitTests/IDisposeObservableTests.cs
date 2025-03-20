namespace SingleFinite.Essentials.UnitTests;

[TestClass]
public class IDisposeObservableTests
{
    [TestMethod]
    public void Dispose_Calls_DisposeState_Dispose()
    {
        var exampleDisposable = new ExampleDisposable();

        Assert.IsFalse(exampleDisposable.DisposeState.IsDisposed);

        ((IDisposable)exampleDisposable).Dispose();

        Assert.IsTrue(exampleDisposable.DisposeState.IsDisposed);
    }

    [TestMethod]
    public void OnDispose_Is_Called_When_Disposed()
    {
        var exampleDisposable = new ExampleDisposable();

        Assert.AreEqual(0, exampleDisposable.ObservedDisposeCount);

        ((IDisposable)exampleDisposable).Dispose();

        Assert.AreEqual(1, exampleDisposable.ObservedDisposeCount);

        ((IDisposable)exampleDisposable).Dispose();

        Assert.AreEqual(1, exampleDisposable.ObservedDisposeCount);
    }

    private sealed class ExampleDisposable : IDisposeObservable
    {
        public ExampleDisposable()
        {
            DisposeState = new(
                owner: this,
                onDispose: OnDispose
            );
        }

        public int ObservedDisposeCount { get; private set; }

        public DisposeState DisposeState { get; }

        private void OnDispose()
        {
            ObservedDisposeCount++;
        }
    }
}
