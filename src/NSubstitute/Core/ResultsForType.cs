using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core;

public class ResultsForType(ICallInfoFactory callInfoFactory) : IResultsForType
{
    private readonly CallResults _results = new CallResults(callInfoFactory);

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

    private class MatchingReturnTypeSpecification(Type expectedReturnType) : ICallSpecification
    {
        public bool IsSatisfiedBy(ICall call)
            => call.GetReturnType() == expectedReturnType;

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