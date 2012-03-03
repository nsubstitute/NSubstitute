using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class ArgumentParamsExtractorFactory : IArgumentParamsExtractorFactory
    {
        public IArgumentParamsExtractor Create(MethodInfo methodInfo, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            if(arguments.Any() && HasParamsArgument(methodInfo))
            {
                return CreateParamsExtractor(arguments);
            }

            return new PassthroughArgumentParamsExtractor();
        }

        private IArgumentParamsExtractor CreateParamsExtractor(IEnumerable<object> arguments)
        {
            if(HasArgumentSpecifications(arguments))
            {
                return new ArgumentSpecificationParamsExtractor();
            }

            return new ObjectArgumentParamsExtractor();
        }

        private bool HasArgumentSpecifications(IEnumerable<object> arguments)
        {
            Type argumentType = arguments.First().GetType();
            
            return argumentType.GetInterfaces().Contains(typeof (IArgumentSpecification));
        }

        private bool HasParamsArgument(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().Any(p => p.IsParams());
        }
    }
}