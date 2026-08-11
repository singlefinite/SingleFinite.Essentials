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
/// Implements the task job interfaces.
/// </summary>
/// <typeparam name="TResult">The type of result for the task.</typeparam>
/// <param name="scope">
/// The scope that will be canceled when the job is canceled or when the job
/// is completed.
/// </param>
/// <param name="cancellationToken">The cancellation token used.</param>
internal class TaskJob<TResult>(
    ITaskScope scope,
    CancellationToken cancellationToken
) : ITaskJob<TResult>
{
    #region Fields

    /// <summary>
    /// Holds the task completion source.
    /// </summary>
    private readonly TaskCompletionSource<TResult> _taskCompletionSource = new();

    #endregion

    #region Properties

    /// <inheritdoc/>
    public Task<TResult> Task => _taskCompletionSource.Task;

    /// <inheritdoc/>
    Task ITaskJob.Task => Task;

    /// <inheritdoc/>
    public CancellationToken CancellationToken => cancellationToken;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Cancel() => scope.Cancel();

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

        scope.Cancel();
    }

    /// <summary>
    /// Run the task returned by the function.
    /// </summary>
    /// <param name="function">The function to start the task.</param>
    public void Run(Func<CancellationToken, Task<TResult>> function)
    {
        _ = RunAsync(function);
    }

    #endregion
}
