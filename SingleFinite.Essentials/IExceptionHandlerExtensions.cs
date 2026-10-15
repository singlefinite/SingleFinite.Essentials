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

using SingleFinite.Essentials.Internal;

namespace SingleFinite.Essentials;

/// <summary>
/// Extension members for IExceptionHandler types.
/// </summary>
public static class IExceptionHandlerExtensions
{
    extension(IExceptionHandler? handler)
    {
        /// <summary>
        /// Combine this handler with the list of handlers into a single
        /// exception handler.  The returned exception handler handles
        /// exceptions by iterating through the list and calling Handle on each
        /// handler until one of them returns true.
        /// </summary>
        /// <param name="handlers">
        /// The handlers to combine with this handler.  This handler will be the
        /// first handler in the list.
        /// </param>
        /// <returns>
        /// The combined exception handlers.  If the resulting list has no non
        /// null handlers null will be returned.
        /// </returns>
        public IExceptionHandler? Combine(
            params IEnumerable<IExceptionHandler?> handlers
        )
        {
            var list = new List<IExceptionHandler?>(handlers);
            list.Insert(0, handler);
            return list.CombineHandlers();
        }
    }

    extension(IEnumerable<IExceptionHandler?> handlers)
    {
        /// <summary>
        /// Combine the list of handlers into a single exception handler.  The
        /// returned exception handler handles exceptions by iterating through
        /// the list and calling Handle on each handler until one of them
        /// returns true.
        /// </summary>
        /// <returns>
        /// The combined exception handlers.  If the list has no non null
        /// handlers null will be returned.
        /// </returns>
        public IExceptionHandler? CombineHandlers()
        {
            var filteredList = handlers
                .OfType<IExceptionHandler>()
                .ToList();

            return filteredList.Count switch
            {
                0 => null,
                1 => filteredList[0],
                _ => new ExceptionHandlerList(filteredList)
            };
        }
    }
}
