namespace SweetBuilders;

/// <summary>
/// Provides a generic builder of <typeparamref name="TObject"/>.
/// </summary>
/// <typeparam name="TObject">The type to create a builder of.</typeparam>
public class GenericBuilder<TObject> : BuilderBase<TObject, GenericBuilder<TObject>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericBuilder{TObject}"/> class.
    /// </summary>
    public GenericBuilder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericBuilder{TObject}"/> class.
    /// </summary>
    /// <param name="factory">A factory of <typeparamref name="TObject"/>.</param>
    public GenericBuilder(Func<TObject> factory)
        : base(factory)
    {
    }

#pragma warning disable CA1000 // Do not declare static members on generic types
    /// <summary>
    /// Gets a new instance of the <see cref="GenericBuilder{TObject}"/> class
    /// with the default object factory.
    /// </summary>
    public static GenericBuilder<TObject> Default => new();

    /// <summary>
    /// Gets a new instance of the <see cref="GenericBuilder{TObject}"/> class
    /// with an empty object factory. The parameterless constructor will be used.
    /// </summary>
    public static GenericBuilder<TObject> Empty
        => new GenericBuilder<TObject>(Factories.Empty<TObject>).OmitAutoProperties();

    /// <summary>
    /// Gets a new instance of the <see cref="GenericBuilder{TObject}"/> class with an
    /// uninitialized object factory (does not use a constructor).
    /// </summary>
    public static GenericBuilder<TObject> Uninitalized
        => new GenericBuilder<TObject>(Factories.Uninitialized<TObject>).OmitAutoProperties();

    /// <summary>
    /// Provides a generic builder with a specific factory.
    /// </summary>
    /// <param name="factory">A factory of <typeparamref name="TObject"/>.</param>
    /// <returns>A new instance of the <see cref="GenericBuilder{TObject}"/> class.</returns>
    public static GenericBuilder<TObject> From(Func<TObject> factory) => new(factory);
#pragma warning restore CA1000 // Do not declare static members on generic types
}
