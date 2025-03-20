namespace SingleFinite.Essentials;

/// <summary>
/// An object whose dispose state can be observed.
/// </summary>
public interface IDisposeObservable : IDisposable
{
    /// <summary>
    /// The dispose state for the object.
    /// </summary>
    DisposeState DisposeState { get; }

    /// <summary>
    /// Invoke the Dispose method on the DisposeState property.
    /// </summary>
    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
        DisposeState.Dispose();
    }
}
