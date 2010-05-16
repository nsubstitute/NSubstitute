using System;
using System.Reflection;

namespace NSubstitute.Core
{
    public class Call : ICall
    {
        private MethodInfo _methodInfo;
        private object[] _arguments;
        private object _target;

        public Call(MethodInfo methodInfo, object[] arguments, object target)
        {
            _methodInfo = methodInfo;
            _arguments = arguments;
            _target = target;
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