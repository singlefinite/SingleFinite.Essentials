﻿// MIT License
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

namespace SingleFinite.Essentials;

/// <summary>
/// A convience class that can be used to manage the dispose state of an object
/// and is thread safe.
/// </summary>
/// <param name="owner">
/// The object whose dispose state is being managed by this object.
/// </param>
/// <param name="onDispose">
/// Optional action to invoke when this object is disposed.
/// </param>
public sealed class ConcurrentDisposeState(
    object owner,
    Action? onDispose = default
) : DisposeStateBase(owner, onDispose)
{
    #region Fields

    /// <summary>
    /// Used to prevent concurrent execution of the StartDispose method.
    /// </summary>
    private readonly Lock _startDisposeLock = new();

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override bool StartDispose()
    {
        lock (_startDisposeLock)
        {
            if (IsDisposed)
                return false;

            IsDisposed = true;
            return true;
        }
    }

    #endregion
}
