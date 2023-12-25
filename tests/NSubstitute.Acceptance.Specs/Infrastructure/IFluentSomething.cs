namespace NSubstitute.Acceptance.Specs.Infrastructure;

public interface IFluentSomething
{
    IFluentSomething Chain();
    IFluentSomething Me();
    IFluentSomething Together();
    ISomething SorryNoChainingHere();
    ISomething SorryNoChainingHereEither();
}