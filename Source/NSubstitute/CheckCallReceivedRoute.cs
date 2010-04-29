using System;

namespace NSubstitute
{
    public class CheckCallReceivedRoute : IRoute
    {
        private readonly ICallHandler _checkReceivedCallHandler;

        public CheckCallReceivedRoute(ICallHandler checkReceivedCallHandler)
        {
            _checkReceivedCallHandler = checkReceivedCallHandler;
        }

        public object Handle(ICall call)
        {
            return _checkReceivedCallHandler.Handle(call);
        }
    }
}