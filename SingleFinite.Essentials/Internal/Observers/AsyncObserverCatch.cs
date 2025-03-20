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
/// An observer that catches exceptions thrown in the observer chain.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
/// <param name="callback">
/// The callback to invoke whenever an exception is caught.  If the callback
/// returns false the exception will continue up the chain.  If the callback
/// returns true the exception will not continue up the chain.
/// </param>
internal class AsyncObserverCatch(
    IAsyncObserver parent,
    Func<Exception, Task<bool>> callback
) : AsyncObserverBase(parent)
{
    #region Methods

    /// <summary>
    /// Catch any exceptions that are thrown below this observer in the chain
    /// and pass them to the callback function.
    /// </summary>
    /// <returns>This method always returns false.</returns>
    protected async override Task<bool> OnEventAsync()
    {
        try
        {
            await RaiseNextEventAsync();
        }
        catch (Exception ex)
        {
            if (!(await callback(ex)))
                throw;
        }

        return false;
    }

    #endregion
}

/// <summary>
/// An observer that catches exceptions thrown in the observer chain.
/// </summary>
/// <typeparam name="TArgs">
/// The type of arguments passed with observed events.
/// </typeparam>
/// <param name="parent">The parent to this observer.</param>
/// <param name="callback">
/// The callback to invoke whenever an exception is caught.  If the callback
/// returns false the exception will continue up the chain.  If the callback
/// returns true the exception will not continue up the chain.
/// </param>
internal class AsyncObserverCatch<TArgs>(
    IAsyncObserver<TArgs> parent,
    Func<TArgs, Exception, Task<bool>> callback
) : AsyncObserverBase<TArgs>(parent)
{
    #region Methods

    /// <summary>
    /// Catch any exceptions that are thrown below this observer in the chain
    /// and pass them to the callback function.
    /// </summary>
    /// <param name="args">
    /// Arguments passed with the observed event that are passed to the
    /// callback.
    /// </param>
    /// <returns>This method always returns false.</returns>
    protected async override Task<bool> OnEventAsync(TArgs args)
    {
        try
        {
            await RaiseNextEventAsync(args);
        }
        catch (Exception ex)
        {
            if (!(await callback(args, ex)))
                throw;
        }

        return false;
    }

    #endregion
}
