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

namespace SingleFinite.Essentials;

/// <summary>
/// This class combines multiple enumerators into a single enumerator.
/// </summary>
/// <typeparam name="TItem">The type of items that are enumerated.</typeparam>
/// <remarks>
/// Constructor.
/// </remarks>
/// <param name="enumerators">The enumerators to composite.</param>
public class CompositeEnumerator<TItem>(
    params IEnumerable<IEnumerator<TItem>> enumerators
) : IEnumerator<TItem>
{
    #region Fields

    /// <summary>
    /// Index for the current enumerator.
    /// </summary>
    private int _currentEnumeratorIndex = 0;

    /// <summary>
    /// The enumerators that are combined in this enumerator.
    /// </summary>
    private readonly IEnumerator<TItem>[] _enumerators = [.. enumerators];

    #endregion

    #region Properties

    /// <inheritdoc />
    public TItem Current { get; private set; } = default!;

    /// <inheritdoc />
    object? System.Collections.IEnumerator.Current => Current;

    #endregion

    #region Methods

    /// <inheritdoc />
    public bool MoveNext()
    {
        while (_currentEnumeratorIndex < _enumerators.Length)
        {
            if (!_enumerators[_currentEnumeratorIndex].MoveNext())
            {
                _currentEnumeratorIndex++;
            }
            else
            {
                Current = _enumerators[_currentEnumeratorIndex].Current;
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public void Reset()
    {
        foreach (var enumerator in _enumerators)
            enumerator.Reset();

        _currentEnumeratorIndex = 0;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (var enumerator in _enumerators)
            enumerator.Dispose();
    }

    #endregion
}
