namespace SweetBuilders;

using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

/// <summary>
/// .
/// </summary>
internal static class PrivatePropertySetter
{
    private static readonly BindingFlags BindingFlags =
        BindingFlags.Instance |
        BindingFlags.NonPublic |
        BindingFlags.Public;

    /// <summary>
    /// .
    /// </summary>
    /// <typeparam name="TObject">a.</typeparam>
    /// <typeparam name="TBuilder">b.</typeparam>
    /// <typeparam name="TProperty">c.</typeparam>
    /// <typeparam name="TValue">d.</typeparam>
    /// <param name="builder">e.</param>
    /// <param name="propertyPicker">f.</param>
    /// <param name="value">g.</param>
    /// <returns>h.</returns>
    /// <exception cref="ArgumentException">i.</exception>
    public static TBuilder SetPrivate<TObject, TBuilder, TProperty, TValue>(
        this TBuilder builder,
        Expression<Func<TObject, TProperty>> propertyPicker,
        TValue value)
        where TBuilder : BuilderBase<TObject, TBuilder>
    {
        // This is definitely not the best way to do this.
        // Will be improved in the future.
        var expressionParts = propertyPicker.ToString().Split('.').Skip(1).ToList();
        if (!expressionParts.Any())
        {
            throw new ArgumentException("The expression must specify a property or field.", nameof(propertyPicker));
        }

        return builder.WithPrivate(expressionParts.First(), value);
    }

    /// <summary>
    /// .
    /// </summary>
    /// <typeparam name="TObject">a.</typeparam>
    /// <typeparam name="TBuilder">b.</typeparam>
    /// <typeparam name="TValue">d.</typeparam>
    /// <param name="builder">e.</param>
    /// <param name="propertyName">f.</param>
    /// <param name="value">g.</param>
    /// <returns>h.</returns>
    public static TBuilder SetPrivate<TObject, TBuilder, TValue>(this TBuilder builder, string propertyName, TValue value)
        where TBuilder : BuilderBase<TObject, TBuilder>
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

        _ = builder.Do(@object => SetPrivateMemberValue(@object, propertyName, value));
        return builder;
    }

    private static void SetPrivateMemberValue<TObject, TValue>(TObject @object, string propertyName, TValue value)
    {
        var member = typeof(TObject).GetMember(propertyName, BindingFlags).Single();
        switch (member)
        {
            case PropertyInfo propertyInfo when propertyInfo.SetMethod is not null:
                SetPrivatePropertyValue(propertyInfo, @object, value);
                break;
            case PropertyInfo:
                SetBackingFieldValue(propertyName, @object, value);
                break;
            case FieldInfo fieldInfo:
                SetPrivateFieldValue(fieldInfo, @object, value);
                break;
            default:
                throw new InvalidOperationException($"Could not set value of property {propertyName}.");
        }
    }

    private static void SetPrivatePropertyValue<TObject, TValue>(PropertyInfo propertyInfo, TObject @object, TValue value) =>
        propertyInfo.SetMethod.Invoke(@object, BindingFlags, null, new object?[] { value }, CultureInfo.InvariantCulture);

    private static void SetBackingFieldValue<TObject, TValue>(string propertyName, TObject @object, TValue value)
    {
        var backingField = $"<{propertyName}>k__BackingField";
        var backingFieldInfo = typeof(TObject).GetField(backingField, BindingFlags);
        SetPrivateFieldValue(backingFieldInfo, @object, value);
    }

    private static void SetPrivateFieldValue<TObject, TValue>(FieldInfo fieldInfo, TObject @object, TValue value)
        => fieldInfo.SetValue(@object, value, BindingFlags, null, CultureInfo.InvariantCulture);
}
