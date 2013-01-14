using NSubstitute.Core;

namespace NSubstitute.Routing
{
    public interface IRoute
    {
        object Handle(ICall call);
        bool IsRecordReplayRoute { get; }
    }
}