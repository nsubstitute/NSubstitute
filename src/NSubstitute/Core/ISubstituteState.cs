using NSubstitute.Routing.AutoValues;

namespace NSubstitute.Core
{
    public interface ISubstituteState
    {
        ICallCollection ReceivedCalls { get; }
        ICallResults CallResults { get; }
        ICallActions CallActions { get; }
        SubstituteConfig SubstituteConfig { get; set; }
        SequenceNumberGenerator SequenceNumberGenerator { get; }
        IConfigureCall ConfigureCall { get; }
        IEventHandlerRegistry EventHandlerRegistry { get; }
        IAutoValueProvider[] AutoValueProviders { get; }
        ICallResults AutoValuesCallResults { get; }
        ICallBaseExclusions CallBaseExclusions { get; }
        IResultsForType ResultsForType { get; }
        ICustomHandlers CustomHandlers { get; }
    }
}