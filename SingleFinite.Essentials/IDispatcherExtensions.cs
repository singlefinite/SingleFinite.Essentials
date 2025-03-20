namespace SingleFinite.Essentials;

/// <summary>
/// Functions that extend the <seealso cref="IDispatcher"/> interface.
/// </summary>
public static class IDispatcherExtensions
{
    /// <summary>
    /// Execute the given async function without result.
    /// </summary>
    /// <param name="dispatcher">The dispatcher being extended.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task that runs until the function has completed.</returns>
    public static Task RunAsync(
        this IDispatcher dispatcher,
        Func<Task> func
    ) =>
        dispatcher.RunAsync(
            async () =>
            {
                await func();
                return 0;
            }
        );

    /// <summary>
    /// Execute the given function with result.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result returned by the function.
    /// </typeparam>
    /// <param name="dispatcher">The dispatcher being extended.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task that runs until the function has completed.</returns>
    public static Task<TResult> RunAsync<TResult>(
        this IDispatcher dispatcher,
        Func<TResult> func
    ) =>
        dispatcher.RunAsync(
            () => Task.FromResult(func())
        );

    /// <summary>
    /// Execute the given action.
    /// </summary>
    /// <param name="dispatcher">The dispatcher being extended.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>A task that runs until the action has completed.</returns>
    public static Task RunAsync(
        this IDispatcher dispatcher,
        Action action
    ) =>
        dispatcher.RunAsync(
            () =>
            {
                action();
                return Task.FromResult(0);
            }
        );

    /// <summary>
    /// Execute the given action.
    /// This method will dispatch the action to be executed and return right 
    /// away without waiting for the action to complete execution.
    /// </summary>
    /// <param name="dispatcher">The dispatcher being extended.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="onError">
    /// Callback that is invoked if the action generates an exception.
    /// </param>
    public static void Run(
        this IDispatcher dispatcher,
        Action action,
        Action<Exception>? onError = default
    )
    {
        _ = dispatcher.RunAsync(
            () =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex);
                }

                return Task.FromResult(0);
            }
        );
    }

    /// <summary>
    /// Execute the given async func.
    /// This method will dispatch the func to be executed and return right 
    /// away without waiting for the func to complete execution.
    /// </summary>
    /// <param name="dispatcher">The dispatcher being extended.</param>
    /// <param name="func">The func to execute.</param>
    /// <param name="onError">
    /// Callback that is invoked if the func generates an exception.
    /// </param>
    public static void Run(
        this IDispatcher dispatcher,
        Func<Task> func,
        Action<Exception>? onError = default
    )
    {
        _ = dispatcher.RunAsync(
            async () =>
            {
                try
                {
                    await func();
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex);
                }

                return Task.FromResult(0);
            }
        );
    }

    /// <summary>
    /// Execute the given cancellable async function with result.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result returned by the function.
    /// </typeparam>
    /// <param name="dispatcher">The dispatcher being extended.</param>
    /// <param name="func">The function to execute.</param>
    /// <param name="cancellationTokens">
    /// Optional cancellation tokens to link with the cancellation token for 
    /// the dispatcher.
    /// </param>
    /// <returns>A task that runs until the function has completed.</returns>
    public static async Task<TResult> RunAsync<TResult>(
        this IDispatcher dispatcher,
        Func<CancellationToken, Task<TResult>> func,
        params IEnumerable<CancellationToken> cancellationTokens
    )
    {
        var allCancellationTokens =
            new List<CancellationToken>(cancellationTokens)
            {
                dispatcher.DisposeState.CancellationToken
            };

        var cancellationToken = allCancellationTokens.Count switch
        {
            0 => CancellationToken.None,
            1 => allCancellationTokens[0],
            _ => CancellationTokenSource
                .CreateLinkedTokenSource([.. allCancellationTokens])
                .Token
        };

        return await dispatcher.RunAsync(
            func: () => func(cancellationToken)
        );
    }

    /// <summary>
    /// Execute the given cancellable async function without result.
    /// </summary>
    /// <param name="dispatcher">The dispatcher being extended.</param>
    /// <param name="func">The function to execute.</param>
    /// <param name="cancellationTokens">
    /// Optional cancellation tokens to link with the cancellation token for 
    /// the dispatcher.
    /// </param>
    /// <returns>A task that runs until the function has completed.</returns>
    public static Task RunAsync(
        this IDispatcher dispatcher,
        Func<CancellationToken, Task> func,
        params IEnumerable<CancellationToken> cancellationTokens
    ) =>
        dispatcher.RunAsync(
            func: async cancellationToken =>
            {
                await func(cancellationToken);
                return 0;
            },
            cancellationTokens: cancellationTokens
        );

    /// <summary>
    /// Execute the given cancellable function with result.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result returned by the function.
    /// </typeparam>
    /// <param name="dispatcher">The dispatcher being extended.</param>
    /// <param name="func">The function to execute.</param>
    /// <param name="cancellationTokens">
    /// Optional cancellation tokens to link with the cancellation token for
    /// the dispatcher.
    /// </param>
    /// <returns>A task that runs until the function has completed.</returns>
    public static Task<TResult> RunAsync<TResult>(
        this IDispatcher dispatcher,
        Func<CancellationToken, TResult> func,
        params IEnumerable<CancellationToken> cancellationTokens
    ) =>
        dispatcher.RunAsync(
            func: cancellationToken => Task.FromResult(func(cancellationToken)),
            cancellationTokens: cancellationTokens
        );

    /// <summary>
    /// Execute the given cancellable action.
    /// </summary>
    /// <param name="dispatcher">The dispatcher being extended.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationTokens">
    /// Optional cancellation tokens to link with the cancellation token for
    /// the dispatcher.
    /// </param>
    /// <returns>A task that runs until the action has completed.</returns>
    public static Task RunAsync(
        this IDispatcher dispatcher,
        Action<CancellationToken> action,
        params IEnumerable<CancellationToken> cancellationTokens
    ) =>
        dispatcher.RunAsync(
            func: cancellationToken =>
            {
                action(cancellationToken);
                return Task.FromResult(0);
            },
            cancellationTokens: cancellationTokens
        );

    /// <summary>
    /// Execute the given cancellable action.
    /// This method will dispatch the action to be executed and return right 
    /// away without waiting for the action to complete execution.
    /// </summary>
    /// <param name="dispatcher">The dispatcher being extended.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationTokens">
    /// Optional cancellation tokens to link with the cancellation token for
    /// the dispatcher.
    /// </param>
    public static void Run(
        this IDispatcher dispatcher,
        Action<CancellationToken> action,
        params IEnumerable<CancellationToken> cancellationTokens
    ) =>
        dispatcher.Run(
            action,
            onError: null,
            cancellationTokens
        );

    /// <summary>
    /// Execute the given cancellable action.
    /// This method will dispatch the action to be executed and return right 
    /// away without waiting for the action to complete execution.
    /// </summary>
    /// <param name="dispatcher">The dispatcher being extended.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="onError">
    /// Callback that is invoked if the action generates an exception.
    /// </param>
    /// <param name="cancellationTokens">
    /// Optional cancellation tokens to link with the cancellation token for
    /// the dispatcher.
    /// </param>
    public static void Run(
        this IDispatcher dispatcher,
        Action<CancellationToken> action,
        Action<Exception>? onError,
        params IEnumerable<CancellationToken> cancellationTokens
    )
    {
        _ = dispatcher.RunAsync(
            func: cancellationToken =>
            {
                try
                {
                    action(cancellationToken);
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex);
                }

                return Task.CompletedTask;
            },
            cancellationTokens: cancellationTokens
        );
    }

    /// <summary>
    /// Execute the given cancellable async function.
    /// This method will dispatch the function to be executed and return right 
    /// away without waiting for the function to complete execution.
    /// </summary>
    /// <param name="dispatcher">The dispatcher being extended.</param>
    /// <param name="func">The async function to execute.</param>
    /// <param name="cancellationTokens">
    /// Optional cancellation tokens to link with the cancellation token for
    /// the dispatcher.
    /// </param>
    public static void Run(
        this IDispatcher dispatcher,
        Func<CancellationToken, Task> func,
        params IEnumerable<CancellationToken> cancellationTokens
    ) =>
        dispatcher.Run(
            func,
            onError: null,
            cancellationTokens
        );

    /// <summary>
    /// Execute the given cancellable async function.
    /// This method will dispatch the function to be executed and return right 
    /// away without waiting for the function to complete execution.
    /// </summary>
    /// <param name="dispatcher">The dispatcher being extended.</param>
    /// <param name="func">The async function to execute.</param>
    /// <param name="onError">
    /// Callback that is invoked if the function generates an exception.
    /// </param>
    /// <param name="cancellationTokens">
    /// Optional cancellation tokens to link with the cancellation token for
    /// the dispatcher.
    /// </param>
    public static void Run(
        this IDispatcher dispatcher,
        Func<CancellationToken, Task> func,
        Action<Exception>? onError,
        params IEnumerable<CancellationToken> cancellationTokens
    )
    {
        _ = dispatcher.RunAsync(
            func: async cancellationToken =>
            {
                try
                {
                    await func(cancellationToken);
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex);
                }

                return Task.CompletedTask;
            },
            cancellationTokens: cancellationTokens
        );
    }
}
