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

using SingleFinite.Essentials.Internal;

namespace SingleFinite.Essentials;

/// <summary>
/// Implementation of <see cref="ITaskScope"/>.
/// </summary>
public sealed class TaskScope : ICancelableTaskScope, IDisposable
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly DisposeState _disposeState;

    /// <summary>
    /// Holds the registration for the parent cancellation token.
    /// </summary>
    private readonly CancellationTokenRegistration _parentDisposeRegistration;

    /// <summary>
    /// The cancellation token source that provides the token for this scope.
    /// </summary>
    private readonly CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Holds the exception handler for this scope.
    /// </summary>
    private readonly IExceptionHandler? _exceptionHandler;

    /// <summary>
    /// Used to track tasks created by this object.
    /// </summary>
    private readonly TaskRegistry _taskRegistry = new();

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="dispatcher">
    /// The default dispatcher to use for tasks created in this scope.
    /// If not provided, a <see cref="ContinuationDispatcher"/> will be used.
    /// </param>
    /// <param name="exceptionHandler">
    /// Optional exception handler used to handle uncaught exceptions.
    /// </param>
    /// <param name = "parentCancellationToken" >
    /// Optional CancellationToken for the parent of this scope if there is one.
    /// </param>
    public TaskScope(
        ITaskDispatcher? dispatcher = default,
        IExceptionHandler? exceptionHandler = default,
        CancellationToken parentCancellationToken = default
    )
    {
        Dispatcher = dispatcher ?? new CurrentContextDispatcher();

        _disposeState = new(
            owner: this,
            onDispose: OnDispose
        );

        _cancellationTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(parentCancellationToken);

        CancellationToken = _cancellationTokenSource.Token;

        _parentDisposeRegistration = parentCancellationToken.Register(Dispose);

        _exceptionHandler = exceptionHandler;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public ITaskDispatcher Dispatcher { get; }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; }

    /// <inheritdoc/>
    public Task Task => _taskRegistry.Task;

    #endregion

    #region Methods

    /// <summary>
    /// Cancel this scope.  This is the same as disposing of the scope.
    /// </summary>
    public void Cancel() => _disposeState.Dispose();

    /// <inheritdoc/>
    public void Dispose() => _disposeState.Dispose();

    /// <inheritdoc/>
    public ICancelableTaskScope CreateChildScope(
        ITaskDispatcher? dispatcher = default,
        IExceptionHandler? exceptionHandler = default
    )
    {
        var childScope = new TaskScope(
            dispatcher: dispatcher ?? Dispatcher,
            exceptionHandler: exceptionHandler.Combine(_exceptionHandler),
            parentCancellationToken: CancellationToken
        );

        if (!_taskRegistry.Register(childScope.Task))
            childScope.Cancel();

        return childScope;
    }

    /// <inheritdoc/>
    public ITaskJob<TResult> Run<TResult>(
        Func<Task<TResult>> function,
        ITaskDispatcher? dispatcher = default
    )
    {
        var job = new TaskJob<TResult>(
            parentCancellationToken: CancellationToken
        );

        if (!_taskRegistry.Register(job.Task))
        {
            job.Cancel();
            return job;
        }

        job.Run(async () =>
        {
            try
            {
                return await (dispatcher ?? Dispatcher).RunAsync(
                    function: () => function(),
                    scope: this,
                    cancellationToken: job.CancellationToken
                );
            }
            catch (Exception ex)
            {
                HandleTaskJobException(ex);
                throw;
            }
        });

        return job;
    }

    /// <summary>
    /// Handles the given exception.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    private void HandleTaskJobException(Exception exception)
    {
        var unwrappedException = exception;
        if (exception is AggregateException aggregate && aggregate.InnerExceptions.Count == 1)
            unwrappedException = aggregate.InnerExceptions[0];

        _exceptionHandler?.Handle(
            exception: unwrappedException,
            message: "An uncaught exception was thrown within a task job."
        );
    }

    /// <summary>
    /// Clean up this object.
    /// </summary>
    private void OnDispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _parentDisposeRegistration.Unregister();
        _taskRegistry.Close();
    }

    #endregion
}
