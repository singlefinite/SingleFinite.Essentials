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
/// Implementation of <see cref="ITaskScope"/>.
/// </summary>
public sealed class TaskScope : ITaskScope, IDisposable
{
    #region Fields

    /// <summary>
    /// Holds the dispose state for this object.
    /// </summary>
    private readonly DisposeState _disposeState;

    /// <summary>
    /// Holds the registration for the parent cancellation token.
    /// </summary>
    private readonly CancellationTokenRegistration? _parentDisposeRegistration;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="dispatcher">
    /// The default dispatcher to use for tasks created in this scope.
    /// </param>
    /// <param name="parentCancellationToken">
    /// Optional CancellationToken for the parent of this scope if there is one.
    /// </param>
    public TaskScope(
        IDispatcher dispatcher,
        CancellationToken parentCancellationToken = default
    )
    {
        Dispatcher = dispatcher;

        _disposeState = new(owner: this);

        CancellationToken = CreateLinkedToken(
            parentCancellationToken,
            _disposeState.CancellationToken
        );

        if (parentCancellationToken != CancellationToken.None)
        {
            _parentDisposeRegistration =
                parentCancellationToken.Register(Dispose);
        }
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsDisposed => _disposeState.IsDisposed;

    /// <inheritdoc/>
    public IDispatcher Dispatcher { get; }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void Dispose()
    {
        _parentDisposeRegistration?.Unregister();
        _disposeState.Dispose();
    }

    /// <inheritdoc/>
    public TaskScope CreateChildScope(IDispatcher? dispatcher = default) => new
    (
        dispatcher: dispatcher ?? Dispatcher,
        parentCancellationToken: CancellationToken
    );

    /// <inheritdoc/>
    public Task<TResult> RunAsync<TResult>(
        Func<Task<TResult>> function,
        IDispatcher? dispatcher = default,
        CancellationToken cancellationToken = default
    ) => (dispatcher ?? Dispatcher).RunAsync(
        function: function,
        cancellationToken: CreateLinkedToken(
            CancellationToken,
            cancellationToken
        )
    );

    /// <inheritdoc/>
    public Task<TResult> RunAsync<TResult>(
        Func<CancellationToken, Task<TResult>> function,
        IDispatcher? dispatcher = default,
        CancellationToken cancellationToken = default
    )
    {
        var linkedCancellationToken = CreateLinkedToken(
            CancellationToken,
            cancellationToken
        );

        return (dispatcher ?? Dispatcher).RunAsync(
            function: () => function(linkedCancellationToken),
            cancellationToken: linkedCancellationToken
        );
    }

    /// <summary>
    /// Create a single CancellationToken from the given tokens.  The
    /// resulting CancellationToken will be cancelled when any of the given
    /// tokens are cancelled.
    /// </summary>
    /// <param name="cancellationTokens">
    /// The CancellationTokens to link.
    /// </param>
    /// <returns>
    /// The single CancellationToken linked to the given tokens.
    /// </returns>
    public static CancellationToken CreateLinkedToken(
        params IEnumerable<CancellationToken> cancellationTokens
    )
    {
        var filteredTokens = cancellationTokens
            .Where(token => token != CancellationToken.None)
            .ToList();

        return filteredTokens.Count switch
        {
            0 => CancellationToken.None,
            1 => filteredTokens[0],
            _ => CancellationTokenSource.CreateLinkedTokenSource(
                    tokens: [.. filteredTokens]
                ).Token
        };
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public Observable Disposed => _disposeState.Disposed;

    #endregion
}
