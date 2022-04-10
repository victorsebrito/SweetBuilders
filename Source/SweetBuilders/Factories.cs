namespace SweetBuilders;

using System;
using System.Runtime.Serialization;

/// <summary>
/// Provides helper methods to create an instance of a type.
/// </summary>
public static class Factories
{
    /// <summary>
    /// Creates an instance of <typeparamref name="T"/> using the default
    /// constructor, including nonpublic.
    /// </summary>
    /// <typeparam name="T">The type to create an instance of.</typeparam>
    /// <returns>An instance of <typeparamref name="T"/>.</returns>
    public static T Empty<T>() => Empty<T>(true);

    /// <summary>
    /// Creates an instance of <typeparamref name="T"/> using the default
    /// constructor.
    /// </summary>
    /// <typeparam name="T">The type to create an instance of.</typeparam>
    /// <param name="nonPublic">true if a public or nonpublic default constructor can match;
    /// false if only a public default constructor can match.</param>
    /// <returns>An instance of <typeparamref name="T"/>.</returns>
    public static T Empty<T>(bool nonPublic) => (T)Activator.CreateInstance(typeof(T), nonPublic);

    /// <summary>
    /// Creates an uninitialized instance of <typeparamref name="T"/>.
    /// Does not use a constructor.
    /// </summary>
    /// <typeparam name="T">The type to create an instance of.</typeparam>
    /// <returns>An instance of <typeparamref name="T"/>.</returns>
    public static T Uninitialized<T>() => (T)FormatterServices.GetUninitializedObject(typeof(T));
}
