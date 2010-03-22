namespace NSubstitute
{
    public class MethodInfoMatcher : ICallMatcher
    {
        public bool IsMatch(ICall first, ICall second)
        {
            return first.GetMethodInfo() == second.GetMethodInfo();
        }

        public bool IsMatch(ICall call, ICallSpecification callSpecification)
        {
            return call.GetMethodInfo() == callSpecification.MethodInfo;
        }
    }
}