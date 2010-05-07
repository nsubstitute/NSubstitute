using System;
using System.Reflection;
using NSubstitute.Core;

namespace NSubstitute.Specs
{
    public class FakeCall : ICall
    {
        private readonly Type _returnType;
        private readonly MethodInfo _methodInfo;
        readonly object _target;
        private readonly object[] _arguments;

        public FakeCall(Type returnType, MethodInfo methodInfo, object target, object[] arguments)
        {
            _returnType = returnType;
            _methodInfo = methodInfo;
            _target = target;
            _arguments = arguments;
        }

        public Type GetReturnType() { return _returnType; }
        public MethodInfo GetMethodInfo() { return _methodInfo; }
        public object[] GetArguments() { return _arguments; }
        public object Target() { return _target; }
    }
}