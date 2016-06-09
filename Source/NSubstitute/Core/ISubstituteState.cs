using System.Collections.Generic;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Routing.Handlers;

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
        SubstituteConfig SubstituteConfig { get; set; }
        SequenceNumberGenerator SequenceNumberGenerator { get; }
        IConfigureCall ConfigureCall { get; }
        IEventHandlerRegistry EventHandlerRegistry { get; }
        IList<IAutoValueProvider> AutoValueProviders { get; }
        ICallBaseExclusions CallBaseExclusions { get; }
        IResultsForType ResultsForType { get; }
        AutoValueBehaviour AutoValueBehaviour { get; set; }
        void ClearUnusedCallSpecs();
    }
}