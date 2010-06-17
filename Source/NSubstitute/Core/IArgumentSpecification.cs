using System;

namespace NSubstitute.Core
{
    public interface IArgumentSpecification
    {
        bool IsSatisfiedBy(object argument);
        Type ForType { get; }
    }
}