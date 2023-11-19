using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class ResultsForType : IResultsForType
    {
        private readonly CallResults _results;

        public ResultsForType(ICallInfoFactory callInfoFactory)
        {
            _results = new CallResults(callInfoFactory);
        }

        public void SetResult(Type type, IReturn resultToReturn)
        {
            _results.SetResult(new MatchingReturnTypeSpecification(type), resultToReturn);
        }

        public bool TryGetResult(ICall call, out object? result)
        {
            return _results.TryGetResult(call, out result);
        }

        public void Clear()
        {
            _results.Clear();
        }

        private class MatchingReturnTypeSpecification : ICallSpecification
        {
            private readonly Type _expectedReturnType;

            public MatchingReturnTypeSpecification(Type expectedReturnType)
            {
                _expectedReturnType = expectedReturnType;
            }

            public bool IsSatisfiedBy(ICall call)
                => call.GetReturnType() == _expectedReturnType;
            
            // ******* Rest methods are not required *******

            public string Format(ICall call)
                => throw new NotSupportedException();

            public ICallSpecification CreateCopyThatMatchesAnyArguments()
                => throw new NotSupportedException();

            public void InvokePerArgumentActions(CallInfo callInfo)
                => throw new NotSupportedException();

            public IEnumerable<ArgumentMatchInfo> NonMatchingArguments(ICall call)
                => throw new NotSupportedException();

            public MethodInfo GetMethodInfo()
                => throw new NotSupportedException();

            public Type ReturnType()
                => throw new NotSupportedException();
        }
    }
}