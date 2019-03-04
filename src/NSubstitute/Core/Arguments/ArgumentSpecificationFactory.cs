using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Exceptions;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecificationFactory : IArgumentSpecificationFactory
    {
        public IArgumentSpecification Create(object argument, IParameterInfo parameterInfo,
            ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
        {
            return typeof(IEnumerable).IsAssignableFrom(parameterInfo.ParameterType) && parameterInfo.ParameterType != typeof(string)
                ? CreateSpecFromParamsArg((IEnumerable)argument, parameterInfo, suppliedArgumentSpecifications)
                : CreateSpecFromNonParamsArg(argument, parameterInfo, suppliedArgumentSpecifications);
        }

        private IArgumentSpecification CreateSpecFromNonParamsArg(object argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
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

        private IArgumentSpecification CreateSpecFromParamsArg(IEnumerable argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
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

            var arrayArgumentSpecifications = UnwrapParamsArguments(argument.Cast<object>(), UnderlyingCollectionType(parameterInfo), suppliedArgumentSpecifications);
            return new ArgumentSpecification(parameterInfo.ParameterType, new ArrayContentsArgumentMatcher(arrayArgumentSpecifications, parameterInfo.IsParams));
        }

        private static Type UnderlyingCollectionType(IParameterInfo parameterInfo)
        {
            if (parameterInfo.ParameterType.IsArray)
            {
                return parameterInfo.ParameterType.GetElementType();
            }

            if (typeof(IEnumerable<>).IsAssignableFrom(parameterInfo.ParameterType))
            {
                return parameterInfo.ParameterType.GetGenericArguments().First();
            }

            return typeof(object);
        }

        private IEnumerable<IArgumentSpecification> UnwrapParamsArguments(IEnumerable<object> args, Type paramsElementType, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
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