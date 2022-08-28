namespace SweetBuilders.Test;

using System;
using AutoFixture;

#pragma warning disable CS0169, IDE0051 // Remove unused private members
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1402 // File may only contain a single type
internal class Foo
{
    private readonly int? privateField;

    public Foo(string status) => this.Status = status;

    private Foo() => this.Name = Guid.NewGuid().ToString();

    public int? Id { get; set; }

    public string? Name { get; set; }

    public string? Status { get; private set; }

    public string? Other { get; }

    public Bar? Bar { get; set; }

    private string? PrivateProperty { get; set; }
}

internal class Bar
{
    public Bar() => this.Id = Guid.NewGuid().ToString();

    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }
}

internal class FooBuilder : BuilderBase<Foo, FooBuilder>
{
    internal static readonly string NAME = Guid.NewGuid().ToString();
    internal static readonly string STATUS = Guid.NewGuid().ToString();

    public FooBuilder()
        : base(Factory) => this.With(x => x.Name, NAME);

    private static Func<Foo> Factory => () => new Foo(STATUS);
}

internal class BarBuilder : BuilderBase<Bar, BarBuilder>
{
    internal static readonly string NAME = Guid.NewGuid().ToString();

    public BarBuilder() => this.With(x => x.Name, NAME);
}

internal class FooBuilderCustomFixture : BuilderBase<Foo, FooBuilderCustomFixture>
{
    internal static readonly string NAME = Guid.NewGuid().ToString();

    public FooBuilderCustomFixture()
        : base(CreateFixture())
    {
    }

    private static Fixture CreateFixture()
    {
        var fixture = new Fixture
        {
            OmitAutoProperties = false,
        };
        fixture.Customize<Bar>(bar => bar.With(x => x.Name, NAME));
        return fixture;
    }
}
#pragma warning restore CS0169, IDE0051 // Remove unused private members
#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore SA1402 // File may only contain a single type
