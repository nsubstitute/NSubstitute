using System.Collections.Concurrent;
using System.Linq;

namespace NSubstitute.Core
{
    public class CallResults : ICallResults
    {
        readonly ICallInfoFactory _callInfoFactory;
        ConcurrentQueue<ResultForCallSpec> _results;

        public CallResults(ICallInfoFactory callInfoFactory)
        {
            _results = new ConcurrentQueue<ResultForCallSpec>();
            _callInfoFactory = callInfoFactory;
        }

        public void SetResult(ICallSpecification callSpecification, IReturn result)
        {
            _results.Enqueue(new ResultForCallSpec(callSpecification, result));
        }

        public bool TryGetResult(ICall call, out object result)
        {
            result = null;
            if (ReturnsVoidFrom(call)) return false;

            var resultWrapper = _results.Reverse().FirstOrDefault(x => x.IsResultFor(call));
            if(resultWrapper == null) return false;

            result = resultWrapper.GetResult(_callInfoFactory.Create(call));
            return true;
        }

        public void Clear()
        {
            _results = new ConcurrentQueue<ResultForCallSpec>();
        }

        bool ReturnsVoidFrom(ICall call)
        {
            return call.GetReturnType() == typeof(void);
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

            public bool IsResultFor(ICall call) => _callSpecification.IsSatisfiedBy(call);
            public object GetResult(CallInfo callInfo) => _resultToReturn.ReturnFor(callInfo);
        }
    }
}