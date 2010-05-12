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

        public void SetResult<T>(ICallSpecification callSpecification, T valueToReturn)
        {
            _results.Add(new ResultForCallSpec(callSpecification, (Func<CallInfo,T>) (x => valueToReturn)));
        }

        public void SetResult<T>(ICallSpecification callSpecification, Func<CallInfo,T> funcToGetReturnValue)
        {
            _results.Add(new ResultForCallSpec(callSpecification, funcToGetReturnValue));
        }

        public object GetResult(ICall call)
        {
            if (ReturnsVoidFrom(call)) return null;
            foreach (var callResult in _results.Reverse())
            {
                var callSpecification = callResult.CallSpecification;
                if (callSpecification.IsSatisfiedBy(call))
                {
                    var funcToGetReturnValue = callResult.FuncToGetResult;
                    var callInfo = _callInfoFactory.Create(call);
                    return funcToGetReturnValue.DynamicInvoke(callInfo);
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
            public ResultForCallSpec(ICallSpecification callSpecification, Delegate funcToGetResult)
            {
                CallSpecification = callSpecification;
                FuncToGetResult = funcToGetResult;
            }

            public ICallSpecification CallSpecification { get; private set; }
            public Delegate FuncToGetResult { get; private set; }
        }
    }
}