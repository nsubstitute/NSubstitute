using System;
using System.Collections.Generic;

namespace NSubstitute
{
    public class CallResults : ICallResults
    {
        ICallMatcher _callMatcher;
        Dictionary<ICall, object> _results;

        public CallResults(ICallMatcher callMatcher)
        {
            _callMatcher = callMatcher;
            _results = new Dictionary<ICall, object>();
        }

        public void SetResult<T>(ICall call, T valueToReturn)
        {
            _results.Add(call, valueToReturn);
        }

        public object GetResult(ICall call)
        {
            if (DoesCallReturnVoid(call)) return null;
            foreach (var callResult in _results)
            {
                if (_callMatcher.IsMatch(callResult.Key, call))
                {
                    return callResult.Value;
                }
            }            
            return GetDefaultResultFor(call);
        }

        public object GetDefaultResultFor(ICall call)
        {
            return GetDefaultInstanceOf(call.GetReturnType());
        }

        object GetDefaultInstanceOf(Type type)
        {            
            if (IsVoid(type)) return null;
            if (type.IsValueType) return CreateInstanceOfTypeWithNoConstructorArgs(type);
            return null;
        }

        object CreateInstanceOfTypeWithNoConstructorArgs(Type type)
        {
            return Activator.CreateInstance(type);
        }

        bool DoesCallReturnVoid(ICall call)
        {
            return IsVoid(call.GetReturnType());
        }

        bool IsVoid(Type type)
        {
            return type == typeof (void);
        }
    }
}