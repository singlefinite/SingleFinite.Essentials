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
    /// <returns>The given item if it's not null.</returns>
    /// <exception cref="NullReferenceException">
    /// Thrown if the given item is null.
    /// </exception>
    public static TType ThrowIfNull<TType>(
        this TType? item,
        [CallerArgumentExpression(nameof(item))] string? name = default
    ) =>
        item ?? throw new NullReferenceException($"{name} is null.");

    #endregion
}
