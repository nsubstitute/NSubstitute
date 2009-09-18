using System;
using System.Collections.Generic;

namespace NSubstitute
{
    public class CallResults : ICallResults
    {
        Dictionary<IInvocation, object> _results;

        public CallResults()
        {
            _results = new Dictionary<IInvocation, object>();
        }

        public void SetResult<T>(IInvocation invocation, T valueToReturn)
        {
            _results.Add(invocation, valueToReturn);
        }

        public object GetResult(IInvocation invocation)
        {
            object result;
            if (_results.TryGetValue(invocation, out result)) return result;
            return GetDefaultInstanceOf(invocation.GetReturnType());
        }

        object GetDefaultInstanceOf(Type type)
        {
            if (type.IsValueType) return CreateInstanceOfTypeWithNoConstructorArgs(type);
            return null;
        }

        object CreateInstanceOfTypeWithNoConstructorArgs(Type type)
        {
            return Activator.CreateInstance(type);
        }

        
    }
}