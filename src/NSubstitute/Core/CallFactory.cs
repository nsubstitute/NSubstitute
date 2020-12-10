using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class CallFactory : ICallFactory
    {
        public ICall Create(MethodInfo methodInfo, object?[] arguments, object target, IList<IArgumentSpecification> argumentSpecifications, Func<object>? baseMethod)
        {
            return new Call(methodInfo, arguments, target, argumentSpecifications , baseMethod);
        }

        public ICall Create(MethodInfo methodInfo, object?[] arguments, object target, IList<IArgumentSpecification> argumentSpecifications)
        {
            return new Call(methodInfo, arguments, target, argumentSpecifications, baseMethod: null);
        }
    }
}