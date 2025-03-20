namespace SingleFinite.Essentials;

/// <summary>
/// An abstraction of a platform specific dispatcher that is used to execute
/// code on a different thread.
/// </summary>
public interface IDispatcher : IDisposeObservable
{
    /// <summary>
    /// Execute the given async function.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result returned by the function.
    /// </typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task that runs until the function has completed.</returns>
    Task<TResult> RunAsync<TResult>(Func<Task<TResult>> func);
}
