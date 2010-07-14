using System;
using System.Collections.Generic;

namespace NSubstitute.Core
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
            object result = null;
            foreach (var handler in _handlers)
            {
                result = handler.Handle(call); 
            }
            return result;
        }
    }
}