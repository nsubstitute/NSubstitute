using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class CallResults : ICallResults
    {
        IList<ResultForCallSpec> _results;

        public CallResults()
        {
            _results = new List<ResultForCallSpec>();
        }

        public void SetResult<T>(ICallSpecification callSpecification, T valueToReturn)
        {
            _results.Add(new ResultForCallSpec(callSpecification, valueToReturn));
        }

        public object GetResult(ICall call)
        {
            if (ReturnsVoidFrom(call)) return null;
            foreach (var callResult in _results.Reverse())
            {
                var callSpecification = callResult.CallSpecification;
                if (callSpecification.IsSatisfiedBy(call))
                {
                    return callResult.Result;
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

        class ResultForCallSpec
        {
            public ResultForCallSpec(ICallSpecification callSpecification, object result)
            {
                CallSpecification = callSpecification;
                Result = result;
            }

            public ICallSpecification CallSpecification { get; private set; }
            public object Result { get; private set; }
        }
    }
}