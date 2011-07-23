using System;

namespace NSubstitute.Core.Arguments
{
    public interface IArgumentSpecification
    {
        bool IsSatisfiedBy(object argument);
        Type ForType { get; }
        Action<object> Action { get; set; }
    }
}