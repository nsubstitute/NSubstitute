using NSubstitute.Exceptions;

namespace NSubstitute.Core;

internal sealed class CallRouterResolver : ICallRouterResolver
{
    public ICallRouter ResolveFor(object substitute)
    {
        return substitute switch
        {
            null => throw new NullSubstituteReferenceException(),
            ICallRouterProvider provider => provider.GetCallRouter(),
            Delegate { Target: ICallRouterProvider provider } => provider.GetCallRouter(),
            _ => throw new NotASubstituteException()
        };
    }
}