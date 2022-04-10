namespace SweetBuilders.Test;

using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

public class GenericBuilderTests
{
    [Fact]
    public void ShouldCreateBuilderWithCustomFactory()
    {
        var status = Guid.NewGuid().ToString();
        var foo = GenericBuilder<Foo>.From(() => new Foo(status))
            .Create();

        using (new AssertionScope())
        {
            foo.Id.Should().NotBeNull("the builder should generate all public properties");
            foo.Name.Should().NotBeNull("the builder should generate all public properties");
            foo.Bar.Should().NotBeNull("the builder should generate all public properties");
            foo.Status.Should().Be(status, "the builder should use the provided factory");
        }
    }

    [Fact]
    public void ShouldCreateEmptyBuilder()
    {
        var bar = GenericBuilder<Bar>.Empty
            .Create();

        using (new AssertionScope())
        {
            bar.Id.Should().NotBeNull("it's set by the constructor");
            bar.Name.Should().BeNull("the builder should omit auto properties");
        }
    }

    [Fact]
    public void ShouldCreateUninitializedBuilder()
    {
        var bar = GenericBuilder<Bar>.Uninitalized
            .Create();

        using (new AssertionScope())
        {
            bar.Id.Should().BeNull("the constructor will not be used.");
            bar.Name.Should().BeNull("the builder should omit auto properties");
        }
    }
}
