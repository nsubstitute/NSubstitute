
namespace NSubstitute.Acceptance.Specs.Infrastructure.DerivedInterfaces;
public interface A
{
    public string X { get; }
}

public interface B : A
{
    new public string X { get; set; }
}

