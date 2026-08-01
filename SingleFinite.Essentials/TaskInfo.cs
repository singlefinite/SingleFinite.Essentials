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

namespace SingleFinite.Essentials;

/// <summary>
/// Implementation of <see cref="ITaskInfo"/>.
/// </summary>
/// <param name="task">The task object.</param>
/// <param name="cancellationTokenSource">The cancellation token source.</param>
public class TaskInfo(
    Task task,
    CancellationTokenSource cancellationTokenSource
) : ITaskInfo
{
    #region Properties

    /// <inheritdoc/>
    public Task Task { get; } = task;

    /// <inheritdoc/>
    public CancellationToken CancellationToken => cancellationTokenSource.Token;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Cancel() => cancellationTokenSource.Cancel();

    /// <summary>
    /// Create a new instance of <see cref="TaskInfo"/> with a task factory.
    /// </summary>
    /// <param name="factory">The task factory.</param>
    /// <returns>The created instance of <see cref="TaskInfo"/>.</returns>
    public static TaskInfo Create(Func<CancellationToken, Task> factory)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var task = factory(cancellationTokenSource.Token);
        return new TaskInfo(task, cancellationTokenSource);
    }

    /// <summary>
    /// Create a new instance of <see cref="TaskInfo{TResult}"/> with a task
    /// factory.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result returned by the task.
    /// </typeparam>
    /// <param name="factory">The task factory.</param>
    /// <returns>The new instance of <see cref="TaskInfo{TResult}"/>.</returns>
    public static TaskInfo<TResult> Create<TResult>(
        Func<CancellationToken, Task<TResult>> factory
    )
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var task = factory(cancellationTokenSource.Token);
        return new TaskInfo<TResult>(task, cancellationTokenSource);
    }

    #endregion
}

/// <summary>
/// Implementation of <see cref="ITaskInfo{TResult}"/>.
/// </summary>
/// <typeparam name="TResult">
/// The type of result returned by the task.
/// </typeparam>
/// <param name="task">The task object.</param>
/// <param name="cancellationTokenSource">The cancellation token source.</param>
public class TaskInfo<TResult>(
    Task<TResult> task,
    CancellationTokenSource cancellationTokenSource
) : TaskInfo(task, cancellationTokenSource), ITaskInfo<TResult>
{
    #region Properties

    /// <inheritdoc/>
    public new Task<TResult> Task { get; } = task;

    #endregion
}
