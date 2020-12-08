using System;

namespace NSubstitute.Core.Arguments
{
    public interface IArgumentSpecificationCompatibilityTester
    {
        bool IsSpecificationCompatible(IArgumentSpecification specification, object? argumentValue, Type argumentType);
    }
}