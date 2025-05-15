using NSubstitute.Core;

namespace NSubstitute.Internal.Routing;

public interface IRoute
{
    object? Handle(ICall call);
}