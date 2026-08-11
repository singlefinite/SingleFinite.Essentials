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

namespace SingleFinite.Essentials.Internal;

/// <summary>
/// Implements the task job interface.
/// </summary>
/// <typeparam name="TResult">The type of result for the task.</typeparam>
internal class TaskJob<TResult> : ITaskJob<TResult>
{
    #region Fields

    /// <summary>
    /// The cancellation token source that provides the token for this job.
    /// </summary>
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    /// <summary>
    /// Holds the task completion source.
    /// </summary>
    private readonly TaskCompletionSource<TResult> _taskCompletionSource = new();

    /// <summary>
    /// Set to true once the Run method has been called.
    /// </summary>
    private bool _isRun = false;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="cancellationToken">
    /// Optional cancellation token to link with the cancellation token for this
    /// job.
    /// </param>
    public TaskJob(CancellationToken cancellationToken = default)
    {
        CancellationToken = TaskScope.CreateLinkedToken(
            cancellationToken,
            _cancellationTokenSource.Token
        );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public Task<TResult> Task => _taskCompletionSource.Task;

    /// <inheritdoc/>
    Task ITaskJob.Task => Task;

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Cancel() => _cancellationTokenSource.Cancel();

    /// <summary>
    /// Call the function to start the task and assign the task results to the
    /// task completion source.
    /// </summary>
    /// <param name="function">The function to start the task.</param>
    /// <returns>The running task.</returns>
    private async Task RunAsync(Func<CancellationToken, Task<TResult>> function)
    {
        try
        {
            var result = await function(CancellationToken);
            _taskCompletionSource.TrySetResult(result);
        }
        catch (OperationCanceledException)
        {
            _taskCompletionSource.SetCanceled();
        }
        catch (Exception ex)
        {
            _taskCompletionSource.SetException(ex);
        }
    }

    /// <summary>
    /// Run the task returned by the function.
    /// </summary>
    /// <param name="function">The function to start the task.</param>
    public void Run(Func<CancellationToken, Task<TResult>> function)
    {
        if (_isRun)
            throw new InvalidOperationException("The job has already been run.");
        _isRun = true;

        _ = RunAsync(function);
    }

    #endregion
}
