using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class Call : ICall
    {
        private MethodInfo _methodInfo;
        private object[] _arguments;
        private object _target;
        private readonly Type[] _parameterTypes;
        private IList<IArgumentSpecification> _argumentSpecifications;

        public Call(MethodInfo methodInfo, object[] arguments, object target): this(methodInfo, arguments, target, null)
        {}

        public Call(MethodInfo methodInfo, object[] arguments, object target, Type[] parameterTypes)
        {
            _methodInfo = methodInfo;
            _arguments = arguments;
            _target = target;
            _parameterTypes = parameterTypes ?? GetParameterTypesFrom(_methodInfo);
            _argumentSpecifications = SubstitutionContext.Current.DequeueAllArgumentSpecifications();
        }

        private Type[] GetParameterTypesFrom(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters == null) return new Type[0];
            return parameters.Select(x => x.ParameterType).ToArray();
        }

        public Type[] GetParameterTypes()
        {
            return _parameterTypes;
        }

        public IList<IArgumentSpecification> GetArgumentSpecifications()
        {
            return _argumentSpecifications;
        }

        public Type GetReturnType()
        {
            return _methodInfo.ReturnType;
        }

        public MethodInfo GetMethodInfo()
        {
            return _methodInfo;
        }

        public object[] GetArguments()
        {
            return _arguments;
        }

        public object Target()
        {
            return _target;
        }
    }
}