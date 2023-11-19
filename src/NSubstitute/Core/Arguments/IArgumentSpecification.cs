using System;

namespace NSubstitute.Core.Arguments
{
    public interface IArgumentSpecification
    {
        bool IsSatisfiedBy(object? argument);
        Type ForType { get; }
        IArgumentSpecification CreateCopyMatchingAnyArgOfType(Type requiredType);
        bool HasAction { get; }
        void RunAction(object? argument);
        string DescribeNonMatch(object? argument);
        string FormatArgument(object? argument);
    }
}