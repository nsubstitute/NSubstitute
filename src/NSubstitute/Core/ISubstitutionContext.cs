using NSubstitute.Routing;

namespace NSubstitute.Core;

public interface ISubstitutionContext
{
    ISubstituteFactory SubstituteFactory { get; }
    IRouteFactory RouteFactory { get; }
    ICallSpecificationFactory CallSpecificationFactory { get; }

    /// <summary>
    /// A thread bound state of the NSubstitute context. Usually this API is used to provide the fluent
    /// features of the NSubstitute.
    /// </summary>
    IThreadLocalContext ThreadContext { get; }

    ICallRouter GetCallRouterFor(object substitute);
}
