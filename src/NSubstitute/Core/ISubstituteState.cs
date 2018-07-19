using System.Collections.Generic;
using NSubstitute.Routing.AutoValues;

namespace NSubstitute.Core
{
    public interface ISubstituteState
    {
        ICallBaseConfiguration CallBaseConfiguration { get; }
        ICallCollection ReceivedCalls { get; }
        ICallResults CallResults { get; }
        ICallActions CallActions { get; }
        IConfigureCall ConfigureCall { get; }
        IEventHandlerRegistry EventHandlerRegistry { get; }
        IReadOnlyCollection<IAutoValueProvider> AutoValueProviders { get; }
        ICallResults AutoValuesCallResults { get; }
        IResultsForType ResultsForType { get; }
        ICustomHandlers CustomHandlers { get; }
    }
}
