using System;

namespace NSubstitute
{
    public class DoOnCallHandler : ICallHandler
    {
        private readonly Action<object[]> _actionToPerform;

        public DoOnCallHandler(Action<object[]> actionToPerform)
        {
            _actionToPerform = actionToPerform;
        }

        public object Handle(ICall call)
        {
            _actionToPerform(call.GetArguments());
            return null;
        }
    }
}