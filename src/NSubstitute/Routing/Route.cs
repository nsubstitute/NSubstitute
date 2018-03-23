using System.Collections.Generic;
using NSubstitute.Core;

namespace NSubstitute.Routing
{
    public enum RouteType { Other, RecordReplay }

    public class Route : IRoute
    {
        private readonly ICallHandler[] _handlers;

        public Route(ICallHandler[] handlers) : this(RouteType.Other, handlers) { }

        public Route(RouteType routeType, ICallHandler[] handlers)
        {
            IsRecordReplayRoute = routeType == RouteType.RecordReplay;
            _handlers = handlers;
        }

        public bool IsRecordReplayRoute { get; }

        public IEnumerable<ICallHandler> Handlers => _handlers;

        public object Handle(ICall call)
        {
            // This is a hot method which is invoked frequently and has major impact on performance.
            // Therefore, the LINQ cycle was unwinded to for loop.
            for (int i = 0; i < _handlers.Length; i++)
            {
                var result = _handlers[i].Handle(call);
                if (result.HasReturnValue)
                {
                    return result.ReturnValue;
                }
            }

            return null;
        }
    }
}