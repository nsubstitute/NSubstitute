namespace NSubstitute.Core;

public interface ICallRouter
{
    /// <summary>
    /// Specifies whether base method should be called by default.
    /// </summary>
    /// <remarks>
    /// This configuration is considered only when base method exists (e.g. you created a substitute for
    /// the AbstractType with method implementation).
    /// </remarks>
    bool CallBaseByDefault { get; set; }
    ConfiguredCall LastCallShouldReturn(IReturn returnValue, MatchArgs matchArgs, PendingSpecificationInfo pendingSpecInfo);
    object? Route(ICall call);
    IEnumerable<ICall> ReceivedCalls();
    void SetReturnForType(Type type, IReturn returnValue);
    void RegisterCustomCallHandlerFactory(CallHandlerFactory factory);
    void Clear(ClearOptions clear);
}