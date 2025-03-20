namespace SingleFinite.Essentials;

/// <summary>
/// Implementation of <see cref="IDispatcher"/> that queues execution of 
/// functions and actions to the thread pool using 
/// <see cref="Task.Run(Func{Task?})"/>.
/// </summary>
public class ThreadPoolDispatcher : IDispatcher
{
    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public ThreadPoolDispatcher()
    {
        DisposeState = new(this);
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public DisposeState DisposeState { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Queue the function to run on the thread pool.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result returned by the function.
    /// </typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task that runs until the function has completed.</returns>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this object has been disposed.
    /// </exception>
    public Task<TResult> RunAsync<TResult>(Func<Task<TResult>> func)
    {
        DisposeState.ThrowIfDisposed();
        return Task.Run(func);
    }

    #endregion
}
