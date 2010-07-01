using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class ArgumentSpecificationFactory : IArgumentSpecificationFactory
    {
        public IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object[] arguments, Type[] parameterTypes, bool matchAnyArguments)
        {
            if (matchAnyArguments) return parameterTypes.Select(x => (IArgumentSpecification) new ArgumentIsAnythingSpecification(x));

            if (argumentSpecs.Count == arguments.Length) return argumentSpecs; 

            var result = new List<IArgumentSpecification>();
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                result.Add(new ArgumentEqualsSpecification(arguments[i], parameterTypes[i]));
            }
            if (argumentSpecs.Count == 0)
            {
                return result;
            }
            foreach (var argumentSpecification in argumentSpecs)
            {
                if (SingleParameterMatchesType(parameterTypes, argumentSpecification.ForType))
                {
                    var index = IndexOfParameterInfoWithMatchingType(parameterTypes, argumentSpecification.ForType);
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

        int IndexOfParameterInfoWithMatchingType(Type[] parameterTypes, Type type)
        {
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                if (parameterTypes[i] == type)
                {
                    return i;
                }
            }
            throw new Exception("ParameterInfo with matching type not found");
        }

        bool SingleParameterMatchesType(IEnumerable<Type> parameterTypes, Type type)
        {
            return parameterTypes.Count(x => x == type) == 1;
        }
    }
}