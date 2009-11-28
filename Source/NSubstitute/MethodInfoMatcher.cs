namespace NSubstitute
{
    public class MethodInfoMatcher : IInvocationMatcher
    {
        public bool IsMatch(IInvocation first, IInvocation second)
        {
            return first.GetMethodInfo() == second.GetMethodInfo();
        }
    }
}