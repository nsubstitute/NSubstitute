using System;
using System.Linq;

namespace NSubstitute.Core
{
    public class ResultsForType : IResultsForType
    {
        readonly ICallInfoFactory _callInfoFactory;
        System.Collections.Concurrent.ConcurrentQueue<ResultForTypeSpec> _results 
            = new System.Collections.Concurrent.ConcurrentQueue<ResultForTypeSpec>();

        public ResultsForType(ICallInfoFactory callInfoFactory)
        {
            _callInfoFactory = callInfoFactory;
        }

        public bool HasResultFor(ICall call)
        {
            return !ReturnsVoid(call) && _results.Any(x => x.IsResultFor(call));
        }

        private bool ReturnsVoid(ICall call)
        {
            return call.GetReturnType() == typeof(void);
        }

        public void SetResult(Type type, IReturn resultToReturn)
        {
            _results.Enqueue(new ResultForTypeSpec(type, resultToReturn));
        }

        public void Clear()
        {
            _results = new System.Collections.Concurrent.ConcurrentQueue<ResultForTypeSpec>();
        }

        public object GetResult(ICall call)
        {
            return _results
                    .Reverse()
                    .First(x => x.IsResultFor(call))
                    .GetResult(_callInfoFactory.Create(call));
        }

        class ResultForTypeSpec
        {
            private readonly Type _type;
            private readonly IReturn _resultToReturn;

            public ResultForTypeSpec(Type type, IReturn resultToReturn)
            {
                _type = type;
                _resultToReturn = resultToReturn;
            }

            public bool IsResultFor(ICall call)
            {
                return call.GetReturnType() == _type;
            }

            public object GetResult(CallInfo callInfo)
            {
                return _resultToReturn.ReturnFor(callInfo);
            }
        }
    }
}