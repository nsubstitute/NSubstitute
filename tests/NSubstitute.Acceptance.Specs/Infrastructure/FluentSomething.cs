namespace NSubstitute.Acceptance.Specs.Infrastructure;

public class FluentSomething : IFluentSomething
{
    public IFluentSomething Chain()
    {
        return this;
    }

    public IFluentSomething Me()
    {
        return this;
    }

    public IFluentSomething Together()
    {
        return this;
    }

    public ISomething SorryNoChainingHere()
    {
        return null;
    }

    public ISomething SorryNoChainingHereEither()
    {
        return null;
    }
}