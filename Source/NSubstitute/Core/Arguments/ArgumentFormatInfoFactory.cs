using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentFormatInfoFactory : IArgumentFormatInfoFactory
    {
        public IEnumerable<IArgumentFormatInfo> CreateArgumentFormatInfos(MethodInfo methodInfoOfCall, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            if (arguments.Any() && HasParamsArgument(methodInfoOfCall))
            {
                return CreateArgumentFormatInfosWithParamsExpanded(arguments, argumentsToHighlight);
            }
            
            return CreateArgumentFormatInfos(arguments, argumentsToHighlight);
        }

        private IEnumerable<IArgumentFormatInfo> CreateArgumentFormatInfos(IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            foreach (object argument in arguments)
            {
                bool isHighlighted = argumentsToHighlight.Contains(arguments.ToList().IndexOf(argument));
                yield return CreateArgumentFormatInfo(argument, isHighlighted);
            }
        }

        private IEnumerable<IArgumentFormatInfo> CreateArgumentFormatInfosWithParamsExpanded(IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            List<IArgumentFormatInfo> argumentFormatInfos = CreateArgumentFormatInfos(arguments, argumentsToHighlight).ToList();

            argumentFormatInfos.Remove(argumentFormatInfos.Last());

            bool isHighlighted = argumentsToHighlight.Contains(arguments.Count() - 1);
            argumentFormatInfos.Add(CreateParamsArgumentFormatInfo(arguments.Last(), isHighlighted));

            return argumentFormatInfos;
        }

        private IArgumentFormatInfo CreateParamsArgumentFormatInfo(object argument, bool isHighlighted)
        {
            if (argument is IArgumentSpecification)
            {
                return CreateArgumentSpecificationParamsArrayFormatInfo(argument as IArgumentSpecification, isHighlighted);
            }
            
            if(argument is object[])
            {
                return CreateObjectArrayArgumentFormatInfo(argument as object[], isHighlighted);
            }

            throw new ArgumentException("Argument is not a IArgumentSpecification nor object[].", "argument");
        }

        private IArgumentFormatInfo CreateObjectArrayArgumentFormatInfo(object[] arguments, bool isHighlighted)
        {
            return new ObjectParamsArrayArgumentFormatInfo(arguments, isHighlighted);
        }

        private IArgumentFormatInfo CreateArgumentSpecificationParamsArrayFormatInfo(IArgumentSpecification argumentSpecification, bool isHighlighted)
        {
            return new ArgumentSpecificationParamsArrayFormatInfo(argumentSpecification, isHighlighted);
        }

        private IArgumentFormatInfo CreateArgumentFormatInfo(object argument, bool isHighlighted)
        {
            if(argument is IArgumentSpecification)
            {
                return CreateArgumentSpecificationFormatInfo(argument as IArgumentSpecification, isHighlighted);
            }

            return CreateObjectArgumentFormatInfo(argument, isHighlighted);
        }

        private IArgumentFormatInfo CreateObjectArgumentFormatInfo(object argument, bool isHighlighted)
        {
            return new ObjectArgumentFormatInfo(argument, isHighlighted);
        }

        private IArgumentFormatInfo CreateArgumentSpecificationFormatInfo(IArgumentSpecification argumentSpecification, bool isHighlighted)
        {
            return new ArgumentSpecificationFormatInfo(argumentSpecification, isHighlighted);
        }

        private bool HasParamsArgument(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().Any(p => p.IsParams());
        }
    }
}