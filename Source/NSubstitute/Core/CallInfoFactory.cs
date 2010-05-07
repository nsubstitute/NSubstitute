using System;

namespace NSubstitute.Core
{
    public class CallInfoFactory : ICallInfoFactory
    {
        public CallInfo Create(ICall call)
        {
            return new CallInfo(call.GetArguments());
        }
    }
}