namespace NSubstitute.Core
{
    public class CallInfoFactory : ICallInfoFactory
    {
        public CallInfo Create(ICall call)
        {
            var arguments = GetArgumentsFromCall(call);
            return new CallInfo(arguments);
        }

        private static Argument[] GetArgumentsFromCall(ICall call)
        {
            var result = new Argument[call.GetOriginalArguments().Length];

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = new Argument(call, i);
            }

            return result;
        }
    }
}