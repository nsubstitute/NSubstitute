using System;
using System.Reflection;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecificationCompatibilityTester : IArgumentSpecificationCompatibilityTester
    {
        private readonly IDefaultChecker _defaultChecker;

        public ArgumentSpecificationCompatibilityTester(IDefaultChecker defaultChecker)
        {
            _defaultChecker = defaultChecker;
        }

        public bool IsSpecificationCompatible(IArgumentSpecification specification, object? argumentValue, Type argumentType)
        {
            var typeArgSpecIsFor = specification.ForType;
            return AreTypesCompatible(argumentType, typeArgSpecIsFor)
                   && IsProvidedArgumentTheOneWeWouldGetUsingAnArgSpecForThisType(argumentValue, typeArgSpecIsFor);
        }

        private bool IsProvidedArgumentTheOneWeWouldGetUsingAnArgSpecForThisType(object? argument, Type typeArgSpecIsFor)
        {
            return _defaultChecker.IsDefault(argument, typeArgSpecIsFor);
        }

        private bool AreTypesCompatible(Type argumentType, Type typeArgSpecIsFor)
        {
            return argumentType.IsAssignableFrom(typeArgSpecIsFor) ||
                (argumentType.IsByRef && !typeArgSpecIsFor.IsByRef && argumentType.IsAssignableFrom(typeArgSpecIsFor.MakeByRefType()));
        }
    }
}