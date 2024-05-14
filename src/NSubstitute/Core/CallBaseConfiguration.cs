using System.Collections.Concurrent;

namespace NSubstitute.Core;

public class CallBaseConfiguration : ICallBaseConfiguration
{
    // Even though ConcurrentStack allocates on each push, we expect that configuration happens rarely.
    // Stack allows us to perform reverse enumeration (which is the most common operation) cheaply.
    private ConcurrentStack<CallBaseRule> Rules { get; } = new();

    /// <inheritdoc />
    public bool CallBaseByDefault { get; set; }

    /// <inheritdoc />
    public void Exclude(ICallSpecification callSpecification)
    {
        Rules.Push(new CallBaseRule(callSpecification, callBase: false));
    }

    /// <inheritdoc />
    public void Include(ICallSpecification callSpecification)
    {
        Rules.Push(new CallBaseRule(callSpecification, callBase: true));
    }

    /// <inheritdoc />
    public bool ShouldCallBase(ICall call)
    {
        return TryGetExplicitConfiguration(call, out bool callBase)
            ? callBase
            : CallBaseByDefault;
    }

    private bool TryGetExplicitConfiguration(ICall call, out bool callBase)
    {
        callBase = default;

        // Performance optimization, as enumerator retrieval allocates.
        if (Rules.IsEmpty)
        {
            return false;
        }

        // Use explicit foreach instead of LINQ to improve performance (avoid delegate construction).
        foreach (var rule in Rules)
        {
            if (rule.IsSatisfiedBy(call))
            {
                callBase = rule.CallBase;
                return true;
            }
        }

        return false;
    }

    private readonly struct CallBaseRule(ICallSpecification callSpecification, bool callBase)
    {
        public bool CallBase { get; } = callBase;

        public bool IsSatisfiedBy(ICall call) => callSpecification.IsSatisfiedBy(call);
    }
}