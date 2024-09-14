using NSubstitute.Core.DependencyInjection;
using NSubstitute.Routing;

namespace NSubstitute.Core;

public class SubstitutionContext(
    ISubstituteFactory substituteFactory,
    IRouteFactory routeFactory,
    ICallSpecificationFactory callSpecificationFactory,
    IThreadLocalContext threadLocalContext,
    ICallRouterResolver callRouterResolver) : ISubstitutionContext
{
    public static ISubstitutionContext Current { get; set; }

    public ISubstituteFactory SubstituteFactory { get; } = substituteFactory;
    public IRouteFactory RouteFactory { get; } = routeFactory;
    public IThreadLocalContext ThreadContext { get; } = threadLocalContext;
    public ICallSpecificationFactory CallSpecificationFactory { get; } = callSpecificationFactory;

    static SubstitutionContext()
    {
        Current = NSubstituteDefaultFactory.CreateSubstitutionContext();
    }

    public ICallRouter GetCallRouterFor(object substitute) =>
        callRouterResolver.ResolveFor(substitute);
}
