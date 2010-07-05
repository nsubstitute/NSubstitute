using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class MixedArgumentSpecificationFactory : IMixedArgumentSpecificationFactory
    {
        public IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object[] arguments, Type[] parameterTypes)
        {
            var result = new List<IArgumentSpecification>();
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                result.Add(new ArgumentEqualsSpecification(arguments[i], parameterTypes[i]));
            }

            if (argumentSpecs.Count == 0)
            {
                return result;
            }

            CheckArgumentSpecsDefinedForAllDefaultArguments(argumentSpecs, arguments, parameterTypes);

            var argumentSpecIndex = 0;
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                var haveArgumentSpec = argumentSpecIndex < argumentSpecs.Count;
                if (haveArgumentSpec)
                {
                    var argumentSpec = argumentSpecs[argumentSpecIndex];
                    if (parameterTypes[i] == argumentSpec.ForType)
                    {
                        if (Object.Equals(arguments[i], GetDefaultValue(parameterTypes[i])))
                        {
                            result[i] = argumentSpec;
                            argumentSpecIndex++;
                        }
                    }
                }
                    
            }

            return result;
        }

        private void CheckArgumentSpecsDefinedForAllDefaultArguments(IList<IArgumentSpecification> argumentSpecs, object[] arguments, Type[] parameterTypes)
        {
            var distinctTypes = parameterTypes.Distinct();
            foreach (var type in distinctTypes)
            {
                var defaultValue = GetDefaultValue(type);
                var numberOfArgumentSpecsForType = argumentSpecs.Count(x => x.ForType == type);
                if (numberOfArgumentSpecsForType > 0)
                {
                    var numberOfArgumentsWithDefaultValueForType =
                        arguments.Where((x, index) => parameterTypes[index] == type && Object.Equals(x, defaultValue)).Count();
                    if (numberOfArgumentSpecsForType != numberOfArgumentsWithDefaultValueForType)
                    {
                        throw new AmbiguousArgumentsException(
                            "Cannot determine argument specifications to use. Please use specifications for all arguments of the same type.");
                    }
                }
            }
        }

        static readonly Dictionary<Type, object> DefaultValueCache = new Dictionary<Type, object>();

        static object GetDefaultValue(Type type)
        {
            if (!type.IsValueType)
            {
                return null;
            }
            object result;
            if (DefaultValueCache.TryGetValue(type, out result))
            {
                return result;
            }
            result = Activator.CreateInstance(type);
            DefaultValueCache[type] = result;
            return result;
        }
    }
}