using System;
using System.Reflection;

namespace NSubstitute.Specs
{
    public class FakeCall : ICall
    {
        private readonly Type _returnType;
        private readonly MethodInfo _methodInfo;
        private readonly object[] _arguments;

        public FakeCall(Type returnType, MethodInfo methodInfo, object[] arguments)
        {
            _returnType = returnType;
            _methodInfo = methodInfo;
            _arguments = arguments;
        }

        public Type GetReturnType() { return _returnType; }

        public MethodInfo GetMethodInfo() { return _methodInfo; }

        public object[] GetArguments() { return _arguments; }
    }
}