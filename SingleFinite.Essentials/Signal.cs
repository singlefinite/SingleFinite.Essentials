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
/// This class provides a generic way for code to wait for something to signal.
/// </summary>
public class Signal
{
    #region Fields

    /// <summary>
    /// The underlying source for the task used to wait.
    /// </summary>
    private TaskCompletionSource _source = new();

    #endregion

    #region Methods

    /// <summary>
    /// Set the signal which will cause code that is watching to proceed.
    /// </summary>
    public void Set()
    {
        _source.TrySetResult();
    }

    /// <summary>
    /// Reset the signal if it has been set.
    /// </summary>
    public void Reset()
    {
        if (!_source.Task.IsCompleted)
            return;

        _source = new();
    }

    /// <summary>
    /// Callers can await this method to wait for the signal to be set.
    /// </summary>
    /// <returns>A task that completes when the signal has been set.</returns>
    public Task WatchAsync() => _source.Task;

    #endregion
}
