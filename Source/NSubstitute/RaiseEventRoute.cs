using System;

namespace NSubstitute
{
    public class RaiseEventRoute : IRoute
    {
        private readonly IEventRaiser _eventRaiser;
        private readonly Func<ICall, object[]> _argumentsToRaiseEventWith;

        public RaiseEventRoute(IEventRaiser eventRaiser, Func<ICall, object[]> argumentsToRaiseEventWith)
        {
            _eventRaiser = eventRaiser;
            _argumentsToRaiseEventWith = argumentsToRaiseEventWith;
        }

        public object Handle(ICall call)
        {
            _eventRaiser.Raise(call, _argumentsToRaiseEventWith(call));
            return null;
        }
    }
}