using System.Collections.Concurrent;

namespace NSubstitute.Core
{
    public class CallResults : ICallResults
    {
        private readonly ICallInfoFactory _callInfoFactory;
        // There was made a decision to use ConcurrentStack instead of ConcurrentQueue here.
        // The pros is that reverse enumeration is cheap. The cons is that stack allocates on each push.
        // We presume that read operations will dominate, so stack suits better.
        private readonly ConcurrentStack<ResultForCallSpec> _results;

        public CallResults(ICallInfoFactory callInfoFactory)
        {
            _results = new ConcurrentStack<ResultForCallSpec>();
            _callInfoFactory = callInfoFactory;
        }

        public void SetResult(ICallSpecification callSpecification, IReturn result)
        {
            _results.Push(new ResultForCallSpec(callSpecification, result));
        }

        public bool TryGetResult(ICall call, out object result)
        {
            result = default;
            if (ReturnsVoidFrom(call))
            {
                return false;
            }

            var resultWrapper = FindResultForCall(call);
            if (resultWrapper == null)
            {
                return false;
            }

            result = resultWrapper.GetResult(call, _callInfoFactory);
            return true;
        }

        private ResultForCallSpec FindResultForCall(ICall call)
        {
            // Optimization for performance - enumerator makes allocation.
            if (_results.IsEmpty)
            {
                return null;
            }

            foreach (var result in _results)
            {
                if (result.IsResultFor(call))
                {
                    return result;
                }
            }

            return null;
        }

        public void Clear()
        {
            _results.Clear();
        }

        private static bool ReturnsVoidFrom(ICall call)
        {
            return call.GetReturnType() == typeof(void);
        }

        private class ResultForCallSpec
        {
            private readonly ICallSpecification _callSpecification;
            private readonly IReturn _resultToReturn;

            public ResultForCallSpec(ICallSpecification callSpecification, IReturn resultToReturn)
            {
                _callSpecification = callSpecification;
                _resultToReturn = resultToReturn;
            }

            public bool IsResultFor(ICall call) => _callSpecification.IsSatisfiedBy(call);
            public object GetResult(ICall call, ICallInfoFactory callInfoFactory)
            {
                if (_resultToReturn is ICallIndependentReturn callIndependentReturn)
                {
                    return callIndependentReturn.GetReturnValue();
                }

                var callInfo = callInfoFactory.Create(call);
                return _resultToReturn.ReturnFor(callInfo);
            }
        }
    }
}