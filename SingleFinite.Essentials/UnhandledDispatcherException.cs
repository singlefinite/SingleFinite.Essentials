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
/// Arguments for an unhandled dispatcher exception event.
/// </summary>
/// <remarks>
/// Constructor.
/// </remarks>
/// <param name="dispatcher">
/// The dispatcher that the unhandled exception occured in.
/// </param>
/// <param name="exception">The unhandled exception.</param>
public class UnhandledDispatcherException(
    IDispatcher dispatcher,
    Exception exception
) : EventArgs
{
    #region Properties

    /// <summary>
    /// The dispatcher that the unhandled exception occured in.
    /// </summary>
    public IDispatcher Dispatcher { get; } = dispatcher;

    /// <summary>
    /// The unhandled exception.
    /// </summary>
    public Exception Exception { get; } = exception;

    #endregion
}
