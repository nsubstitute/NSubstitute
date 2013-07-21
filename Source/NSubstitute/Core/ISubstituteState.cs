using NSubstitute.Routing.AutoValues;

namespace NSubstitute.Core
{
    public interface ISubstituteState
    {
        ISubstitutionContext SubstitutionContext { get; }
        ICallStack CallStack { get; }
        IReceivedCalls ReceivedCalls { get; }
        IPendingSpecification PendingSpecification { get; }
        ICallResults CallResults { get; }
        ICallSpecificationFactory CallSpecificationFactory { get; }
        ICallActions CallActions { get; }
        bool CallBaseByDefault { get; set; }
        SequenceNumberGenerator SequenceNumberGenerator { get; }
        IConfigureCall ConfigureCall { get; }
        IEventHandlerRegistry EventHandlerRegistry { get; }
        IAutoValueProvider[] AutoValueProviders { get; }
        void ClearUnusedCallSpecs();
    }
}