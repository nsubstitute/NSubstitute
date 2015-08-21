using System;

namespace NSubstitute.Acceptance.Specs.Infrastructure
{
    internal interface IFluentSomething
    {
        IFluentSomething Chain();
        IFluentSomething Me();
        IFluentSomething Together();
        ISomething SorryNoChainingHere();
    }
}