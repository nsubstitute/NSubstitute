using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routing
{
    public enum RouteType { Other, RecordReplay };

    public class Route : IRoute
    {
        private readonly bool _isRecordReplayRoute;
        private readonly IEnumerable<ICallHandler> _handlers;

        public Route(IEnumerable<ICallHandler> handlers) : this(RouteType.Other, handlers) { }

        public Route(RouteType routeType, IEnumerable<ICallHandler> handlers)
        {
            _isRecordReplayRoute = routeType == RouteType.RecordReplay;
            _handlers = handlers;
        }

        public bool IsRecordReplayRoute { get { return _isRecordReplayRoute; } }

        public IEnumerable<ICallHandler> Handlers
        {
            get { return _handlers; }
        }

        public object Handle(ICall call)
        {
            var result = Handlers.Select(x => x.Handle(call)).FirstOrDefault(x => x.HasReturnValue);
            return result == null ? null : result.ReturnValue;
        }
    }
}