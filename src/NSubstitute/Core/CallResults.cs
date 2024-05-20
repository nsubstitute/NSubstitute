using System.Collections.Concurrent;

namespace NSubstitute.Core;

public class CallResults(ICallInfoFactory callInfoFactory) : ICallResults
{
    // There was made a decision to use ConcurrentStack instead of ConcurrentQueue here.
    // The pros is that reverse enumeration is cheap. The cons is that stack allocates on each push.
    // We presume that read operations will dominate, so stack suits better.
    private readonly ConcurrentStack<ResultForCallSpec> _results = new ConcurrentStack<ResultForCallSpec>();

    public void SetResult(ICallSpecification callSpecification, IReturn result)
    {
        _results.Push(new ResultForCallSpec(callSpecification, result));
    }

    public bool TryGetResult(ICall call, out object? result)
    {
        result = default;
        if (ReturnsVoidFrom(call))
        {
            return false;
        }

        if (!TryFindResultForCall(call, out var configuredResult))
        {
            return false;
        }

        result = configuredResult.GetResult(call, callInfoFactory);
        return true;
    }

    private bool TryFindResultForCall(ICall call, out ResultForCallSpec configuredResult)
    {
        // Optimization for performance - enumerator makes allocation.
        if (_results.IsEmpty)
        {
            configuredResult = default;
            return false;
        }

        foreach (var result in _results)
        {
            if (result.IsResultFor(call))
            {
                configuredResult = result;
                return true;
            }
        }

        configuredResult = default;
        return false;
    }

    public void Clear()
    {
        _results.Clear();
    }

    private static bool ReturnsVoidFrom(ICall call)
    {
        return call.GetReturnType() == typeof(void);
    }

    private readonly struct ResultForCallSpec(ICallSpecification callSpecification, IReturn resultToReturn)
    {
        public bool IsResultFor(ICall call) => callSpecification.IsSatisfiedBy(call);
        public object? GetResult(ICall call, ICallInfoFactory callInfoFactory)
        {
            if (resultToReturn is ICallIndependentReturn callIndependentReturn)
            {
                return callIndependentReturn.GetReturnValue();
            }

            var callInfo = callInfoFactory.Create(call);
            return resultToReturn.ReturnFor(callInfo);
        }
    }
}