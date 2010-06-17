using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class ArgumentSpecificationFactory : IArgumentSpecificationFactory
    {
        public IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object[] arguments, ParameterInfo[] parameterInfos)
        {
            if (argumentSpecs.Count == arguments.Length)
            {
                return argumentSpecs;
            }

            var result = new List<IArgumentSpecification>();
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                result.Add(new ArgumentEqualsSpecification(arguments[i], parameterInfos[i].ParameterType));
            }
            if (argumentSpecs.Count == 0)
            {
                return result;
            }
            foreach (var argumentSpecification in argumentSpecs)
            {
                if (SingleParameterMatchesType(parameterInfos, argumentSpecification.ForType))
                {
                    var index = IndexOfParameterInfoWithMatchingType(parameterInfos, argumentSpecification.ForType);
                    result[index] = argumentSpecification;
                }
                else
                {
                    throw new AmbiguousArgumentsException(
                        "Cannot determine argument specifications to use. Please use specifications for all arguments of the same type.");
                }
            }
            return result;
        }

        int IndexOfParameterInfoWithMatchingType(ParameterInfo[] parameterInfos, Type type)
        {
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                if (parameterInfos[i].ParameterType == type)
                {
                    return i;
                }
            }
            throw new Exception("ParameterInfo with matching type not found");
        }

        bool SingleParameterMatchesType(IEnumerable<ParameterInfo> parameterInfos, Type type)
        {
            return parameterInfos.Count(x => x.ParameterType == type) == 1;
        }
    }
}