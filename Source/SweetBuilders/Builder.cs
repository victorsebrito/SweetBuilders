namespace SweetBuilders;

/// <summary>
/// Provides a generic builder of <typeparamref name="TObject"/>.
/// </summary>
/// <typeparam name="TObject">The type to create a builder of.</typeparam>
public class Builder<TObject> : BuilderBase<TObject, Builder<TObject>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Builder{TObject}"/> class.
    /// </summary>
    public Builder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Builder{TObject}"/> class.
    /// </summary>
    /// <param name="factory">A factory of <typeparamref name="TObject"/>.</param>
    public Builder(Func<TObject> factory)
        : base(factory)
    {
    }

#pragma warning disable CA1000 // Do not declare static members on generic types
    /// <summary>
    /// Gets a new instance of the <see cref="Builder{TObject}"/> class
    /// with the default object factory.
    /// </summary>
    public static Builder<TObject> Default => new();

    /// <summary>
    /// Gets a new instance of the <see cref="Builder{TObject}"/> class
    /// with an empty object factory. The parameterless constructor will be used.
    /// </summary>
    public static Builder<TObject> Empty
        => new Builder<TObject>(Factories.Empty<TObject>).OmitAutoProperties();

    /// <summary>
    /// Gets a new instance of the <see cref="Builder{TObject}"/> class with an
    /// uninitialized object factory (does not use a constructor).
    /// </summary>
    public static Builder<TObject> Uninitalized
        => new Builder<TObject>(Factories.Uninitialized<TObject>).OmitAutoProperties();

    /// <summary>
    /// Provides a generic builder with a specific factory.
    /// </summary>
    /// <param name="factory">A factory of <typeparamref name="TObject"/>.</param>
    /// <returns>A new instance of the <see cref="Builder{TObject}"/> class.</returns>
    public static Builder<TObject> From(Func<TObject> factory) => new(factory);
#pragma warning restore CA1000 // Do not declare static members on generic types
}
