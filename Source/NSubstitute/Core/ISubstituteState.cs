using System;
using NSubstitute.Routing.AutoValues;

namespace NSubstitute.Core
{
    public interface ISubstituteState
    {
        ISubstitutionContext SubstitutionContext { get; }
        ICallInfoFactory CallInfoFactory { get; }
        ICallStack CallStack { get; }
        IReceivedCalls ReceivedCalls { get; }
        IPendingSpecification PendingSpecification { get; }
        ICallResults CallResults { get; }
        ICallSpecificationFactory CallSpecificationFactory { get; }
        ISubstituteFactory SubstituteFactory { get; }
        ICallActions CallActions { get; }
        SequenceNumberGenerator SequenceNumberGenerator { get; }
        IPropertyHelper PropertyHelper { get; }
        IResultSetter ResultSetter { get; }
        IEventHandlerRegistry EventHandlerRegistry { get; }
        IReceivedCallsExceptionThrower ReceivedCallsExceptionThrower { get; }
        IDefaultForType DefaultForType { get; }
        IAutoValueProvider[] AutoValueProviders { get; }
    }
}