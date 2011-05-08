using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class CallInfoFactory : ICallInfoFactory
    {
        public CallInfo Create(ICall call)
        {
            var arguments = GetArgumentsFromCall(call).ToArray();
            return new CallInfo(arguments);
        }

        private static IEnumerable<Argument> GetArgumentsFromCall(ICall call)
        {
            var values = call.GetArguments();
            var types = call.GetParameterInfos().Select(x => x.ParameterType);

            return values.Zip(types, (value, type) => new Argument(type, value));
        }
    }
}