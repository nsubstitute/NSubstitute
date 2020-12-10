using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecificationFactory : IArgumentSpecificationFactory
    {
        public IArgumentSpecification Create(object? argument, IParameterInfo parameterInfo,
            ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
        {
            return parameterInfo.IsParams
                ? CreateSpecFromParamsArg(argument, parameterInfo, suppliedArgumentSpecifications)
                : CreateSpecFromNonParamsArg(argument, parameterInfo, suppliedArgumentSpecifications);
        }

        private IArgumentSpecification CreateSpecFromNonParamsArg(object? argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
        {
            if (suppliedArgumentSpecifications.IsNextFor(argument, parameterInfo.ParameterType))
            {
                return suppliedArgumentSpecifications.Dequeue();
            }

            bool isAmbiguousSpecificationPresent = suppliedArgumentSpecifications.AnyFor(argument, parameterInfo.ParameterType);
            if (!isAmbiguousSpecificationPresent || parameterInfo.IsOptional || parameterInfo.IsOut)
            {
                return new ArgumentSpecification(parameterInfo.ParameterType, new EqualsArgumentMatcher(argument));
            }

            throw new AmbiguousArgumentsException();
        }

        private IArgumentSpecification CreateSpecFromParamsArg(object? argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
        {
            // Next specification is for the whole params array.
            if (suppliedArgumentSpecifications.IsNextFor(argument, parameterInfo.ParameterType))
            {
                return suppliedArgumentSpecifications.Dequeue();
            }

            // Check whether the specification ambiguity could happen.
            bool isAmbiguousSpecificationPresent = suppliedArgumentSpecifications.AnyFor(argument, parameterInfo.ParameterType);
            if (isAmbiguousSpecificationPresent)
            {
                throw new AmbiguousArgumentsException();
            }

            // User passed "null" as the params array value.
            if (argument == null)
            {
                return new ArgumentSpecification(parameterInfo.ParameterType, new EqualsArgumentMatcher(null));
            }

            // User specified arguments using the native params syntax.
            var arrayArg = argument as Array;
            if (arrayArg == null)
            {
                throw new SubstituteInternalException($"Expected to get array argument, but got argument of '{argument.GetType().FullName}' type.");
            }

            var arrayArgumentSpecifications = UnwrapParamsArguments(arrayArg.Cast<object?>(), parameterInfo.ParameterType.GetElementType()!, suppliedArgumentSpecifications);
            return new ArgumentSpecification(parameterInfo.ParameterType, new ArrayContentsArgumentMatcher(arrayArgumentSpecifications));
        }

        private IEnumerable<IArgumentSpecification> UnwrapParamsArguments(IEnumerable<object?> args, Type paramsElementType, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
        {
            var fakeParameterInfo = new ParameterInfoFromType(paramsElementType);
            var result = new List<IArgumentSpecification>();
            foreach (var arg in args)
            {
                try
                {
                    result.Add(CreateSpecFromNonParamsArg(arg, fakeParameterInfo, suppliedArgumentSpecifications));
                }
                catch (AmbiguousArgumentsException ex)
                {
                    ex.Data[AmbiguousArgumentsException.NonReportedResolvedSpecificationsKey] = result;
                    throw;
                }
            }

            return result;
        }

        private class ParameterInfoFromType : IParameterInfo
        {
            public ParameterInfoFromType(Type parameterType)
            {
                ParameterType = parameterType;
            }

            public Type ParameterType { get; }

            public bool IsParams => false;

            public bool IsOptional => false;

            public bool IsOut => false;
        }
    }
}