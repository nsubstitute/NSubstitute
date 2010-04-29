using System;

namespace NSubstitute
{
    public class RaiseEventRoute : IRoute
    {
        private readonly ICallHandler _raiseEventHandler;

        public RaiseEventRoute(ICallHandler raiseEventHandler)
        {
            _raiseEventHandler = raiseEventHandler;
        }

        public object Handle(ICall call)
        {
            _raiseEventHandler.Handle(call);
            return null;
        }
    }
}