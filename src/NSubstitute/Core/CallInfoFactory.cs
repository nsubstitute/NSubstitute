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
            var parameterInfos = call.GetParameterInfos();

            for (var index = 0; index < values.Length; index++)
            {
                var i = index;
                yield return new Argument(parameterInfos[i].ParameterType, () => values[i], x => values[i] = x);
            }
        }
    }
}