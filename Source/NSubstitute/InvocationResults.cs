using System;
using System.Collections.Generic;

namespace NSubstitute
{
    public class InvocationResults : IInvocationResults
    {
        IInvocationMatcher _invocationMatcher;
        Dictionary<IInvocation, object> _results;

        public InvocationResults(IInvocationMatcher invocationMatcher)
        {
            _invocationMatcher = invocationMatcher;
            _results = new Dictionary<IInvocation, object>();
        }

        public void SetResult<T>(IInvocation invocation, T valueToReturn)
        {
            _results.Add(invocation, valueToReturn);
        }

        public object GetResult(IInvocation invocation)
        {
            if (DoesInvocationReturnVoid(invocation)) return null;
            foreach (var invocationResult in _results)
            {
                if (_invocationMatcher.IsMatch(invocationResult.Key, invocation))
                {
                    return invocationResult.Value;
                }
            }            
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

        bool DoesInvocationReturnVoid(IInvocation invocation)
        {
            return invocation.GetReturnType() == typeof (void);
        }
    }
}