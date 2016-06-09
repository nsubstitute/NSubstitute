using System.Linq;

namespace NSubstitute.Core
{
    public class CallResults : ICallResults
    {
        readonly ICallInfoFactory _callInfoFactory;
        readonly System.Collections.Concurrent.ConcurrentQueue<ResultForCallSpec> _results 
            = new System.Collections.Concurrent.ConcurrentQueue<ResultForCallSpec>();

        public CallResults(ICallInfoFactory callInfoFactory)
        {
            _callInfoFactory = callInfoFactory;
        }

        public void SetResult(ICallSpecification callSpecification, IReturn result)
        {
            _results.Enqueue(new ResultForCallSpec(callSpecification, result));
        }

        public bool HasResultFor(ICall call)
        {
            return _results.Any(x => x.IsResultFor(call));
        }

        public object GetResult(ICall call)
        {
            return _results
                    .Reverse()
                    .First(x => x.IsResultFor(call))
                    .GetResult(_callInfoFactory.Create(call));
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