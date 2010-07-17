using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routing
{
    public class Route : IRoute
    {
        private readonly IEnumerable<ICallHandler> _handlers;

        public Route(IEnumerable<ICallHandler> handlers)
        {
            _handlers = handlers;
        }

        public object Handle(ICall call)
        {
            var result = _handlers.Select(x => x.Handle(call)).FirstOrDefault(x => x.HasReturnValue);
            return result == null ? null : result.ReturnValue;
        }
    }
}