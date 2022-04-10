namespace SweetBuilders.Test;

using System;
using FluentAssertions;
using Xunit;

public class ObjectFactoryTests
{
    [Fact]
    public void ShouldCreateEmptyInstanceWithPublicConstructor()
    {
        var bar = Factories.Empty<Bar>(true);
        bar.Id.Should().NotBeNull();
    }

    [Fact]
    public void ShouldCreateEmptyInstanceWithPrivateConstructor()
    {
        var foo = Factories.Empty<Foo>();
        foo.Name.Should().NotBeNull();
    }

    [Fact]
    public void ShouldThrowExceptionIfOnlyPrivateConstructorIsAvailable()
    {
        var act = () => Factories.Empty<Foo>(false);
        act.Should().Throw<MissingMethodException>();
    }

    [Fact]
    public void ShouldCreateUninitializedInstance()
    {
        var bar = Factories.Uninitialized<Bar>();
        bar.Id.Should().BeNull();
    }
}
