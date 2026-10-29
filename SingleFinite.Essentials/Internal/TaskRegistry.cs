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
/// Tracks tasks for when they complete.
/// </summary>
internal class TaskRegistry
{
    #region Fields

    /// <summary>
    /// Controls access to the state for this object.
    /// </summary>
    private readonly Lock _stateLock = new();

    /// <summary>
    /// The number of registered tasks that have yet to complete.
    /// </summary>
    private int _uncompletedTasks = 0;

    /// <summary>
    /// Indicates when the registry has been closed.
    /// </summary>
    private bool _isClosed = false;

    /// <summary>
    /// Source for task that is completed when this registry has been closed
    /// and all remaining tasks have been completed.
    /// </summary>
    private readonly TaskCompletionSource _taskCompletionSource = new();

    #endregion

    #region Properties

    /// <summary>
    /// A task that completes when this registry has been closed and all
    /// remaining tasks have been completed.
    /// </summary>
    public Task Task => _taskCompletionSource.Task;

    #endregion

    #region Methods

    /// <summary>
    /// Register a task to keep track of.
    /// </summary>
    /// <param name="task">The task to track.</param>
    /// <returns>
    /// true if the task was registered, false if the task was not registered
    /// because the registry has been closed.
    /// </returns>
    public bool Register(Task task)
    {
        lock (_stateLock)
        {
            if (_isClosed)
                return false;

            _uncompletedTasks++;
            task.ContinueWith(_ => OnTaskCompleted());
            return true;
        }
    }

    /// <summary>
    /// Close this registry.
    /// </summary>
    public void Close()
    {
        lock (_stateLock)
        {
            if (_isClosed)
                return;

            _isClosed = true;
            CheckCompleted();
        }
    }

    /// <summary>
    /// Decrease the uncompleted tasks count by one and set the task for this
    /// registry to completed if all registered tasks have been completed.
    /// </summary>
    private void OnTaskCompleted()
    {
        lock (_stateLock)
        {
            _uncompletedTasks--;
            CheckCompleted();
        }
    }

    /// <summary>
    /// Set the task for this registry to completed if all registered tasks have
    /// been completed.
    /// </summary>
    private void CheckCompleted()
    {
        if (_isClosed && _uncompletedTasks == 0)
            _taskCompletionSource.SetResult();
    }

    #endregion
}
