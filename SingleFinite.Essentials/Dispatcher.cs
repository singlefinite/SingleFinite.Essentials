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
/// Provides static members for reporting and observing unhandled exceptions
/// that occur within dispatchers.
/// </summary>
/// <remarks>
/// The Dispatcher class enables applications to monitor unhandled exceptions
/// that arise from tasks associated with a dispatcher. This can be useful for
/// logging, diagnostics, or implementing custom error handling strategies in
/// applications that use dispatchers for task execution.
/// </remarks>
public static class Dispatcher
{
    #region Methods

    /// <summary>
    /// Emit on the UnhandledDispatcherException observable if the given task
    /// results in an exception.
    /// </summary>
    /// <param name="task">The task to check for an exception.</param>
    /// <param name="dispatcher">The dispatcher the task is running on.</param>
    internal static void ReportOnException(
        this Task task,
        IDispatcher dispatcher
    )
    {
        task.ContinueWith(
            continuationAction: result =>
            {
                var normalizedException = NormalizeUnhandledException(
                    result.Exception
                );

                if (
                    normalizedException is null ||
                    ShouldIgnoreUnhandledException(normalizedException)
                )
                {
                    return;
                }

                s_unhandledExceptionSource.Emit(
                    new(
                        dispatcher: dispatcher,
                        exception: normalizedException
                    )
                );
            },
            continuationOptions: TaskContinuationOptions.OnlyOnFaulted
        );
    }

    /// <summary>
    /// Normalize the given unhandled exception.
    /// </summary>
    /// <param name="exception">The exception to normalize.</param>
    /// <returns>The normalized exception.</returns>
    private static Exception? NormalizeUnhandledException(Exception? exception)
    {
        if (
            exception is AggregateException aggregate &&
            aggregate.InnerException is Exception inner
        )
            return inner;

        if (exception is Exception ex)
            return ex;

        return null;
    }

    /// <summary>
    /// Check if the given exception should be ignored.
    /// </summary>
    /// <param name="exception">The exception to check.</param>
    /// <returns>
    /// true if the exception should be ignored, false otherwise.
    /// </returns>
    private static bool ShouldIgnoreUnhandledException(Exception exception) =>
        exception switch
        {
            TaskCanceledException => true,
            OperationCanceledException => true,
            _ => false
        };

    #endregion

    #region Events

    /// <summary>
    /// Observable that emits when an unhandled exception occurs in a
    /// dispatcher.
    /// </summary>
    public static Observable<UnhandledDispatcherException>
        UnhandledDispatcherException => s_unhandledExceptionSource.Observable;
    private static readonly ObservableSource<UnhandledDispatcherException>
        s_unhandledExceptionSource = new();

    #endregion
}
