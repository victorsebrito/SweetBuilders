namespace SweetBuilders;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using AutoFixture;
using AutoFixture.Dsl;

/// <summary>
/// Provides the base implementation of a builder.
/// </summary>
/// <typeparam name="TObject">The type of the objects that will be built.</typeparam>
/// <typeparam name="TBuilder">The type of the builder that implements <see cref="BuilderBase{TObject, TBuilder}"/>.</typeparam>
public abstract class BuilderBase<TObject, TBuilder>
    where TBuilder : BuilderBase<TObject, TBuilder>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BuilderBase{TObject, TBuilder}"/> class.
    /// </summary>
    protected BuilderBase() => this.Composer = this.Fixture.Build<TObject>();

    /// <summary>
    /// Initializes a new instance of the <see cref="BuilderBase{TObject, TBuilder}"/> class.
    /// </summary>
    /// <param name="factory">A factory of the <typeparamref name="TObject"/> class.</param>
    protected BuilderBase(Func<TObject> factory)
    {
        if (factory == null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        var customizationComposer = this.Fixture.Build<TObject>();
        this.Composer = customizationComposer.FromFactory(factory);
    }

    private Fixture Fixture { get; } = new Fixture();

    private IPostprocessComposer<TObject> Composer { get; set; }

    private TBuilder Builder => (TBuilder)this;

    /// <summary>
    /// Performs the specified action on a specimen.
    /// </summary>
    /// <param name="action">The action to perform.</param>
    /// <returns>
    /// A <typeparamref name="TBuilder"/> which can be used to further customize the
    /// post-processing of created specimens.
    /// </returns>
    public TBuilder Do(Action<TObject> action)
    {
        this.Composer = this.Composer.Do(action);
        return this.Builder;
    }

    /// <summary>
    /// Disables auto-properties for a type of specimen.
    /// </summary>
    /// <returns>
    /// A <typeparamref name="TBuilder"/> which can be used to further customize the
    /// post-processing of created specimens.
    /// </returns>
    public TBuilder OmitAutoProperties()
    {
        this.Composer = this.Composer.OmitAutoProperties();
        return this.Builder;
    }

    /// <summary>
    /// Registers that a writable property or field should be assigned an anonymous value as
    /// part of specimen post-processing.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property or field.</typeparam>
    /// <param name="propertyPicker">
    /// An expression that identifies the property or field that will should have a value
    /// assigned.
    /// </param>
    /// <returns>
    /// A <typeparamref name="TBuilder"/> which can be used to further customize the
    /// post-processing of created specimens.
    /// </returns>
    public TBuilder With<TProperty>(Expression<Func<TObject, TProperty>> propertyPicker)
    {
        this.Composer = this.Composer.With(propertyPicker);
        return this.Builder;
    }

    /// <summary>
    /// Registers that a writable property or field should be assigned a specific value as
    /// part of specimen post-processing.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property or field.</typeparam>
    /// <param name="propertyPicker">
    /// An expression that identifies the property or field that will have
    /// <paramref name="value"/> assigned.
    /// </param>
    /// <param name="value">
    /// The value to assign to the property or field identified by
    /// <paramref name="propertyPicker"/>.
    /// </param>
    /// <returns>
    /// A <typeparamref name="TBuilder"/> which can be used to further customize the
    /// post-processing of created specimens.
    /// </returns>
    public TBuilder With<TProperty>(Expression<Func<TObject, TProperty>> propertyPicker, TProperty value)
    {
        this.Composer = this.Composer.With(propertyPicker, value);
        return this.Builder;
    }

    /// <summary>
    /// Registers that a writable property or field should be assigned generated value as a part of specimen post-processing.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property or field.</typeparam>
    /// <param name="propertyPicker">
    /// An expression that identifies the property or field that will have <paramref name="valueFactory"/> result assigned.
    /// </param>
    /// <param name="valueFactory">
    /// The factory of value to assign to the property or field identified by <paramref name="propertyPicker"/>.
    /// </param>
    /// <returns>
    /// A <typeparamref name="TBuilder"/> which can be used to further customize the
    /// post-processing of created specimens.
    /// </returns>
    public TBuilder With<TProperty>(Expression<Func<TObject, TProperty>> propertyPicker, Func<TProperty> valueFactory)
    {
        this.Composer = this.Composer.With(propertyPicker, valueFactory);
        return this.Builder;
    }

    /// <summary>
    /// Registers that a writable property or field should be assigned generated value as a part of specimen post-processing.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property or field.</typeparam>
    /// <typeparam name="TInput">The type of the <paramref name="valueFactory"/> input.</typeparam>
    /// <param name="propertyPicker">
    /// An expression that identifies the property or field that will have <paramref name="valueFactory"/> result assigned.
    /// </param>
    /// <param name="valueFactory">
    /// The factory of value to assign to the property or field identified by <paramref name="propertyPicker"/>.
    /// </param>
    /// <returns>
    /// A <typeparamref name="TBuilder"/> which can be used to further customize the
    /// post-processing of created specimens.
    /// </returns>
    public TBuilder With<TProperty, TInput>(Expression<Func<TObject, TProperty>> propertyPicker, Func<TInput, TProperty> valueFactory)
    {
        this.Composer = this.Composer.With(propertyPicker, valueFactory);
        return this.Builder;
    }

    /// <summary>
    /// Registers that a writable property or field should be assigned a specific value as
    /// part of specimen post-processing.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property or field.</typeparam>
    /// <typeparam name="TValue">The type of the value that will be assigned to the property or field.</typeparam>
    /// <param name="propertyPicker">
    /// An expression that identifies the property or field that will have
    /// <paramref name="value"/> assigned.
    /// </param>
    /// <param name="value">
    /// The value to assign to the property or field identified by
    /// <paramref name="propertyPicker"/>.
    /// </param>
    /// <returns>
    /// A <typeparamref name="TBuilder"/> which can be used to further customize the
    /// post-processing of created specimens.
    /// </returns>
    public TBuilder WithPrivate<TProperty, TValue>(Expression<Func<TObject, TProperty>> propertyPicker, TValue value)
    {
        // This is definitely not the best way to do this.
        // Will be improved in the future.
        var expressionParts = propertyPicker.ToString().Split('.').Skip(1).ToList();
        if (!expressionParts.Any())
        {
            throw new ArgumentException("The expression must specify a property or field.", nameof(propertyPicker));
        }

        return this.WithPrivate(expressionParts.First(), value);
    }

    /// <summary>
    /// Registers that a writable property or field should be assigned a specific value as
    /// part of specimen post-processing.
    /// </summary>
    /// <typeparam name="TValue">The type of the value that will be assigned to the property or field.</typeparam>
    /// <param name="propertyName">
    /// A string that identifies the property or field that will have <paramref name="value"/> assigned.
    /// </param>
    /// <param name="value">
    /// The value to assign to the property or field identified by
    /// <paramref name="propertyName"/>.
    /// </param>
    /// <returns>
    /// A <typeparamref name="TBuilder"/> which can be used to further customize the
    /// post-processing of created specimens.
    /// </returns>
    public TBuilder WithPrivate<TValue>(string propertyName, TValue value)
    {
        if (propertyName == null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        var propertyParts = propertyName.Split('.');
        if (propertyParts.Length > 1)
        {
            throw new ArgumentException("The expression must not contain access to a nested property or field.", nameof(propertyName));
        }

        this.Composer = this.Composer.Do(x =>
        {
            // We need to include BindingFlags.Public to be able to retrieve
            // public properties with private sets. This ends up making this
            // method able to set public properties and fields also.
            // Preventing that would be costly, so I won't, for now. :)
            var flags = BindingFlags.Instance |
                        BindingFlags.SetField |
                        BindingFlags.SetProperty |
                        BindingFlags.NonPublic |
                        BindingFlags.Public;

            _ = typeof(TObject).InvokeMember(propertyName, flags, null, x, new object?[] { value }, CultureInfo.InvariantCulture);
        });
        return this.Builder;
    }

    /// <summary>
    /// Enables auto-properties for a type of specimen.
    /// </summary>
    /// <returns>
    /// A <typeparamref name="TBuilder"/> which can be used to further customize the
    /// post-processing of created specimens.
    /// </returns>
    public TBuilder WithAutoProperties()
    {
        this.Composer = this.Composer.WithAutoProperties();
        return this.Builder;
    }

    /// <summary>
    /// Registers that a writable property should not be assigned any automatic value as
    /// part of specimen post-processing.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property or field to ignore.</typeparam>
    /// <param name="propertyPicker">
    /// An expression that identifies the property or field to be ignored.
    /// </param>
    /// <returns>
    /// A <typeparamref name="TBuilder"/> which can be used to further customize the
    /// post-processing of created specimens.
    /// </returns>
    public TBuilder Without<TProperty>(Expression<Func<TObject, TProperty>> propertyPicker)
    {
        this.Composer = this.Composer.Without(propertyPicker);
        return this.Builder;
    }

    /// <summary>
    /// Creates an anonymous variable of the requested type.
    /// </summary>
    /// <typeparamref name="TObject">The type of object to create.</typeparamref>
    /// <returns>An anonymous object of type <typeparamref name="TObject"/>.</returns>
    public TObject Create() => this.Composer.Create();

    /// <summary>
    /// Creates many anonymous objects.
    /// </summary>
    /// <typeparamref name="TObject">The type of objects to create.</typeparamref>
    /// <returns>A sequence of anonymous objects of type <typeparamref name="TObject"/>.</returns>
    public IEnumerable<TObject> CreateMany() => this.Composer.CreateMany();

    /// <summary>
    /// Creates many anonymous objects.
    /// </summary>
    /// <typeparamref name="TObject">The type of objects to create.</typeparamref>
    /// <param name="count">The number of objects to create.</param>
    /// <returns>A sequence of anonymous objects of type <typeparamref name="TObject"/>.</returns>
    public IEnumerable<TObject> CreateMany(int count) => this.Composer.CreateMany(count);
}
