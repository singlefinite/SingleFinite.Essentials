// MIT License
// Copyright (c) 2025 Single Finite
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


namespace SingleFinite.Essentials.Internal.Observers;

/// <summary>
/// Invoke the next observers using the provided dispatcher.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
/// <param name="dispatcher">
/// The dispatcher to invoke the next observers with.
/// </param>
internal class AsyncObserverDispatch(
    IAsyncObserver parent,
    IDispatcher dispatcher
) : AsyncObserverBase(parent)
{
    #region Methods

    /// <summary>
    /// Raise next event using dispatcher.
    /// </summary>
    /// <returns>Always return false.</returns>
    protected override async Task<bool> OnEventAsync()
    {
        await dispatcher.RunAsync(
            func: RaiseNextEventAsync
        );

        return false;
    }

    #endregion
}

/// <summary>
/// Invoke the next observers using the provided dispatcher.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="dispatcher">
/// The dispatcher to invoke the next observers with.
/// </param>
internal class AsyncObserverDispatch<TArgs>(
    IAsyncObserver<TArgs> parent,
    IDispatcher dispatcher
) : AsyncObserverBase<TArgs>(parent)
{
    #region Methods

    /// <summary>
    /// Raise next event using dispatcher.
    /// </summary>
    /// <param name="args">Arguments passed with the observed event.</param>
    /// <returns>Always return false.</returns>
    protected override async Task<bool> OnEventAsync(TArgs args)
    {
        await dispatcher.RunAsync(
            func: () => RaiseNextEventAsync(args)
        );

        return false;
    }

    #endregion
}
