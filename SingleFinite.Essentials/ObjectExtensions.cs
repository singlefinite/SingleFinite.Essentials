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

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SingleFinite.Essentials;

/// <summary>
/// Extensions methods for object types.
/// </summary>
public static class ObjectExtensions
{
    #region Methods

    /// <summary>
    /// Throw an exception if the given item is null, otherwise return the item
    /// ensuring it's not null.
    /// </summary>
    /// <typeparam name="TType">The type of object required.</typeparam>
    /// <param name="item">The item to check.</param>
    /// <param name="name">
    /// The name used in the exception messsage if the item is null.
    /// Leave unset to use the argument expression passed into this method.
    /// </param>
    /// <returns>The item that was checked.</returns>
    /// <exception cref="NullReferenceException">
    /// Thrown if the given item is null.
    /// </exception>
    public static TType ThrowIfNull<TType>(
        [NotNull] this TType? item,
        [CallerArgumentExpression(nameof(item))] string? name = default
    )
    {
        if (item is null)
            throw new NullReferenceException($"{name} is null.");

        return item;
    }

    /// <summary>
    /// Execute the given block of code against the given item and return the
    /// item.
    /// </summary>
    /// <typeparam name="TType">
    /// The type of item to execute the block of code against.
    /// </typeparam>
    /// <param name="item">
    /// The item to execute the block of code against.
    /// </param>
    /// <param name="block">The block of code to execute.</param>
    /// <returns>The item that was passed in.</returns>
    public static TType Also<TType>(this TType item, Action<TType> block)
    {
        block(item);
        return item;
    }

    #endregion
}
