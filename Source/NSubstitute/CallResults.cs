using System;
using System.Collections.Generic;

namespace NSubstitute
{
    public class CallResults : ICallResults
    {
        Dictionary<ICallSpecification, object> _results;

        public CallResults()
        {
            _results = new Dictionary<ICallSpecification, object>();
        }

        public void SetResult<T>(ICallSpecification callSpecification, T valueToReturn)
        {
            _results.Add(callSpecification, valueToReturn);
        }

        public object GetResult(ICall call)
        {
            if (ReturnsVoidFrom(call)) return null;
            foreach (var callResult in _results)
            {
                var callSpecification = callResult.Key;
                if (callSpecification.IsSatisfiedBy(call))
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

        bool ReturnsVoidFrom(ICall call)
        {
            return IsVoid(call.GetReturnType());
        }

        bool IsVoid(Type type)
        {
            return type == typeof (void);
        }
    }
}