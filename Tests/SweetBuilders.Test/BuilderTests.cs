namespace SweetBuilders.Test;

using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

public class BuilderTests
{
    [Fact]
    public void ShouldCreateBuilderWithCustomFactory()
    {
        var status = Guid.NewGuid().ToString();
        var foo = Builder<Foo>.From(() => new Foo(status))
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
    public void ShouldCreateDefaultBuilder()
    {
        var builder = Builder<Bar>.Default;
        var bar = builder.Create();

        using (new AssertionScope())
        {
            bar.Id.Should().NotBeNull("it's set by the constructor");
            bar.Name.Should().NotBeNull("the builder should generate all public properties");
            bar.Status.Should().NotBeNull("the builder should generate all public properties");
        }
    }

    [Fact]
    public void ShouldCreateEmptyBuilder()
    {
        var bar = Builder<Bar>.Empty
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
        var bar = Builder<Bar>.Uninitialized
            .Create();

        using (new AssertionScope())
        {
            bar.Id.Should().BeNull("the constructor will not be used.");
            bar.Name.Should().BeNull("the builder should omit auto properties");
        }
    }
}
