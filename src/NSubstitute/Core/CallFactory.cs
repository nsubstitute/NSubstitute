using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class CallFactory : ICallFactory
    {
        public ICall Create(MethodInfo methodInfo, object[] arguments, object target, IList<IArgumentSpecification> argumentSpecifications, Func<object> baseMethod)
        {
            var parameterInfos = GetParameterInfoFromMethod(methodInfo);
            return new Call(methodInfo, arguments, target, argumentSpecifications, parameterInfos, baseMethod);
        }

        public ICall Create(MethodInfo methodInfo, object[] arguments, object target, IList<IArgumentSpecification> argumentSpecifications)
        {
            var parameterInfos = GetParameterInfoFromMethod(methodInfo);
            return new Call(methodInfo, arguments, target, argumentSpecifications, parameterInfos, baseMethod: null);
        }

        private static IParameterInfo[] GetParameterInfoFromMethod(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().Select(x => new ParameterInfoWrapper(x)).ToArray();
        }
    }
}