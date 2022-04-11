namespace SweetBuilders.Test;

using System;
using FluentAssertions;
using Xunit;

public class ObjectFactoryTests
{
    [Fact]
    public void ShouldCreateDefaultInstanceWithPublicConstructor()
    {
        var bar = Factories.Default<Bar>(true);
        bar.Id.Should().NotBeNull();
    }

    [Fact]
    public void ShouldCreateDefaultInstanceWithPrivateConstructor()
    {
        var foo = Factories.Default<Foo>();
        foo.Name.Should().NotBeNull();
    }

    [Fact]
    public void ShouldThrowExceptionIfOnlyPrivateConstructorIsAvailable()
    {
        var act = () => Factories.Default<Foo>(false);
        act.Should().Throw<MissingMethodException>();
    }

    [Fact]
    public void ShouldCreateUninitializedInstance()
    {
        var bar = Factories.Uninitialized<Bar>();
        bar.Id.Should().BeNull();
    }
}
