namespace SweetBuilders.Test;

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

public class BuilderBaseTests
{
    [Fact]
    public void ShouldCreateDefaultBuilder()
    {
        var builder = new BarBuilder();
        var bar = builder.Create();

        using (new AssertionScope())
        {
            bar.Id.Should().NotBeNull("the builder should use the default constructor");
            bar.Status.Should().NotBeNull("the builder should generate all public properties");
            bar.Name.Should().Be(BarBuilder.NAME, "the builder base should use the generic type constructor");
        }
    }

    [Fact]
    public void ShouldUseProvidedFactory()
    {
        var builder = new FooBuilder();
        var foo = builder.Create();

        using (new AssertionScope())
        {
            foo.Id.Should().NotBeNull("the builder should generate all public properties");
            foo.Bar.Should().NotBeNull("the builder should generate all public properties");
            foo.Name.Should().Be(FooBuilder.NAME, "the builder base should use the generic type constructor");
            foo.Status.Should().Be(FooBuilder.STATUS, "the builder should use the provided factory");
        }
    }

    [Fact]
    public void ShouldDoAction()
    {
        var value = Guid.NewGuid().ToString();
        var bar = new BarBuilder()
            .OmitAutoProperties()
            .Do(x => x.Status = value)
            .Create();

        using (new AssertionScope())
        {
            bar.Status.Should().Be(value, "the action should be executed on the created object");
        }
    }

    [Fact]
    public void ShouldOmitAutoProperties()
    {
        var bar = new BarBuilder()
            .OmitAutoProperties()
            .Create();

        using (new AssertionScope())
        {
            bar.Status.Should().BeNull("the builder should omit auto properties");
        }
    }

    [Fact]
    public void ShouldIncludeProperty()
    {
        var foo = new FooBuilder()
            .OmitAutoProperties()
            .With(x => x.Id)
            .Create();

        using (new AssertionScope())
        {
            foo.Bar.Should().BeNull("the builder should omit auto properties");
            foo.Id.Should().NotBeNull("the builder should include specified properties");
        }
    }

    [Fact]
    public void ShouldSetPropertyFromValue()
    {
        var value = new Random().Next();
        var foo = new FooBuilder()
            .OmitAutoProperties()
            .With(x => x.Id, value)
            .Create();

        using (new AssertionScope())
        {
            foo.Bar.Should().BeNull("the builder should omit auto properties");
            foo.Id.Should().Be(value, "the builder should set specified properties");
        }
    }

    [Fact]
    public void ShouldSetPropertyFromValueFactory()
    {
        var factoryCount = 0;

        var foos = new FooBuilder()
            .OmitAutoProperties()
            .With(x => x.Id, () => ++factoryCount)
            .CreateMany(2).ToList();

        using (new AssertionScope())
        {
            foos[0].Bar.Should().BeNull("the builder should omit auto properties");
            foos[1].Bar.Should().BeNull("the builder should omit auto properties");
            foos[0].Id.Should().Be(1, "the builder should use the factory to generate the value");
            foos[1].Id.Should().Be(2, "the builder should use the factory to generate the value");
        }
    }

    [Fact]
    public void ShouldSetPropertyFromValueFactoryWithInput()
    {
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CA1305 // Specify IFormatProvider
        var foos = new FooBuilder()
            .OmitAutoProperties()
            .With<string, int>(x => x.Name, (i) => i.ToString())
            .CreateMany(2).ToList();
#pragma warning restore CA1305 // Specify IFormatProvider
#pragma warning restore CS8603 // Possible null reference return.

        using (new AssertionScope())
        {
            foos[0].Bar.Should().BeNull("the builder should omit auto properties");
            foos[1].Bar.Should().BeNull("the builder should omit auto properties");
            foos[0].Name.Should().NotBe(foos[1].Name, "the builder should use the factory to generate different values");
        }
    }

    [Fact]
    public void ShouldThrowExceptionIfExpressionDoesNotSpecifyPrivateMember()
    {
        var builder = new FooBuilder();

        var act = () => builder.WithPrivate(x => x, Guid.NewGuid());

        using (new AssertionScope())
        {
            act.Should().Throw<ArgumentException>()
                .WithMessage("The expression must specify a property or field.*")
                .WithParameterName("propertyPicker");
        }
    }

    [Fact]
    public void ShouldThrowExceptionIfExpressionSpecifiesNestedMember()
    {
        var builder = new FooBuilder();
        var act = () => builder.WithPrivate("Bar.Name", Guid.NewGuid());

        using (new AssertionScope())
        {
            act.Should().Throw<ArgumentException>()
                .WithMessage("The expression must not contain access to a nested property or field.*")
                .WithParameterName("propertyName");
        }
    }

    [Fact]
    public void ShouldSetPrivateField()
    {
        var fieldName = "privateField";
        var value = new Random().Next();
        var foo = new FooBuilder()
            .WithPrivate(fieldName, value)
            .Create();

        using (new AssertionScope())
        {
            var privateField = GetPrivateFieldOrPropertyValue<int>(foo, fieldName);
            privateField.Should().Be(value, "the builder should set the private field");
        }
    }

    [Fact]
    public void ShouldSetPrivateProperty()
    {
        var propertyNameName = "PrivateProperty";
        var value = Guid.NewGuid().ToString();
        var foo = new FooBuilder()
            .WithPrivate(propertyNameName, value)
            .Create();

        using (new AssertionScope())
        {
            var privateField = GetPrivateFieldOrPropertyValue<string>(foo, propertyNameName);
            privateField.Should().Be(value, "the builder should set the private property");
        }
    }

    [Fact]
    public void ShouldSetPublicPropertyWithPrivateSet()
    {
        var value = Guid.NewGuid().ToString();
        var foo = new FooBuilder()
            .WithPrivate(x => x.Status, value)
            .Create();

        using (new AssertionScope())
        {
            foo.Status.Should().Be(value, "the builder should set the property value");
        }
    }

    [Fact]
    public void ShouldIncludeAutoProperties()
    {
        var foo = GenericBuilder<Foo>.Uninitalized
            .WithAutoProperties()
            .Create();

        using (new AssertionScope())
        {
            foo.Id.Should().NotBeNull("the builder should generate all public properties");
            foo.Bar.Should().NotBeNull("the builder should generate all public properties");
            foo.Name.Should().NotBeNull("the builder should generate all public properties");
        }
    }

    [Fact]
    public void ShouldExcludeProperty()
    {
        var foo = GenericBuilder<Foo>.Uninitalized
            .WithAutoProperties()
            .Without(x => x.Name)
            .Create();

        using (new AssertionScope())
        {
            foo.Id.Should().NotBeNull("the builder should generate all public properties");
            foo.Bar.Should().NotBeNull("the builder should generate all public properties");
            foo.Name.Should().BeNull("the builder should exclude specified properties");
        }
    }

    [Fact]
    public void ShouldCreateManyInstancies()
    {
        var foo = GenericBuilder<Foo>.Uninitalized
            .WithAutoProperties()
            .Without(x => x.Name)
            .CreateMany();

        using (new AssertionScope())
        {
            foo.Count().Should().BeGreaterThan(1, "the builder should generate many instances");
        }
    }

    [Fact]
    public void ShouldCreateNInstancies()
    {
        var quantity = new Random().Next(10);
        var foo = GenericBuilder<Foo>.Uninitalized
            .WithAutoProperties()
            .Without(x => x.Name)
            .CreateMany(quantity);

        using (new AssertionScope())
        {
            foo.Count().Should().Be(quantity, "the builder should generate the specified quantity of instances");
        }
    }

    private static T? GetPrivateFieldOrPropertyValue<T>(object @object, string fieldName)
    {
        var flags = BindingFlags.Instance |
                    BindingFlags.GetField |
                    BindingFlags.GetProperty |
                    BindingFlags.NonPublic;

        return (T?)@object.GetType().InvokeMember(fieldName, flags, null, @object, Array.Empty<object>(), CultureInfo.InvariantCulture);
    }
}
