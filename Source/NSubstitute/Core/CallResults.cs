using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class CallResults : ICallResults
    {
        private readonly ICallInfoFactory _callInfoFactory;
        IList<ResultForCallSpec> _results;

        public CallResults(ICallInfoFactory callInfoFactory)
        {
            _callInfoFactory = callInfoFactory;
            _results = new List<ResultForCallSpec>();
        }

        public void SetResult(ICallSpecification callSpecification, IReturn result)
        {
            _results.Add(new ResultForCallSpec(callSpecification, result));
        }

        public object GetResult(ICall call)
        {
            if (ReturnsVoidFrom(call)) return null;
            foreach (var callResult in _results.Reverse())
            {
                var callSpecification = callResult.CallSpecification;
                if (callSpecification.IsSatisfiedBy(call))
                {
                    var callInfo = _callInfoFactory.Create(call);
                    return callResult.ResultToReturn.ReturnFor(callInfo);
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
            public ResultForCallSpec(ICallSpecification callSpecification, IReturn resultToReturn)
            {
                CallSpecification = callSpecification;
                ResultToReturn = resultToReturn;
            }

            public ICallSpecification CallSpecification { get; private set; }
            public IReturn ResultToReturn { get; private set; }
        }
    }
}