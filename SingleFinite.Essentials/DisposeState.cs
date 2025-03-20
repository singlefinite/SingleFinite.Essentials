namespace SingleFinite.Essentials;

/// <summary>
/// A convience class that can be used to manage the dispose state of an object.
/// </summary>
/// <param name="owner">
/// The object whose dispose state is being managed by this object.
/// </param>
/// <param name="onDispose">
/// Optional action to invoke when this object is disposed.
/// </param>
public sealed class DisposeState(
    object owner,
    Action? onDispose = default
) : IDisposable
{
    #region Fields

    /// <summary>
    /// Source for Disposed observable.
    /// </summary>
    private readonly ObservableSource _disposedSource = new();

    /// <summary>
    /// Source for CancellationToken.
    /// </summary>
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if this object has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Observable that will notify observers when this object is disposed.
    /// </summary>
    public Observable Disposed =>
        _disposedSource.Observable;

    /// <summary>
    /// A token that is canceled when this object is disposed.
    /// </summary>
    public CancellationToken CancellationToken =>
        _cancellationTokenSource.Token;

    #endregion

    #region Methods

    /// <summary>
    /// Dispose of this object.
    /// </summary>
    public void Dispose()
    {
        if (IsDisposed)
            return;

        onDispose?.Invoke();
        IsDisposed = true;
        _disposedSource.RaiseEvent();
        _cancellationTokenSource.Cancel();
    }

    /// <summary>
    /// Throw an exception if this object has been disposed.
    /// </summary>
    public void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            condition: IsDisposed,
            instance: owner
        );
    }

    #endregion
}
