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

        public bool HasResultFor(ICall call)
        {
            if (ReturnsVoidFrom(call)) return false;
            return _results.Any(x => x.IsResultFor(call));
        }

        public object GetResult(ICall call)
        {
            return _results
                    .Reverse()
                    .First(x => x.IsResultFor(call))
                    .GetResult(_callInfoFactory.Create(call));
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
            readonly ICallSpecification _callSpecification;
            readonly IReturn _resultToReturn;

            public ResultForCallSpec(ICallSpecification callSpecification, IReturn resultToReturn)
            {
                _callSpecification = callSpecification;
                _resultToReturn = resultToReturn;
            }

            public bool IsResultFor(ICall call) { return _callSpecification.IsSatisfiedBy(call); }
            public object GetResult(CallInfo callInfo) { return _resultToReturn.ReturnFor(callInfo); }
        }
    }
}