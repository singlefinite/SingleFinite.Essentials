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

namespace SingleFinite.Essentials.Internal.Observers;

/// <summary>
/// Invoke the next observers using the provided dispatcher.
/// </summary>
/// <param name="parent">The parent to this observer.</param>
/// <param name="dispatcher">
/// The dispatcher to invoke the next observers with.
/// </param>
internal class ObserverDispatch(
    IObserver parent,
    IDispatcher dispatcher
) : ObserverBase(parent)
{
    #region Methods

    /// <summary>
    /// Raise next event using dispatcher.
    /// </summary>
    /// <returns>Always return false.</returns>
    protected override bool OnEvent()
    {
        dispatcher.Run(
            action: RaiseNextEvent
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
internal class ObserverDispatch<TArgs>(
    IObserver<TArgs> parent,
    IDispatcher dispatcher
) : ObserverBase<TArgs>(parent)
{
    #region Methods

    /// <summary>
    /// Raise next event using dispatcher.
    /// </summary>
    /// <param name="args">Arguments passed with the observed event.</param>
    /// <returns>Always return false.</returns>
    protected override bool OnEvent(TArgs args)
    {
        dispatcher.Run(
            action: () => RaiseNextEvent(args)
        );

        return false;
    }

    #endregion
}
