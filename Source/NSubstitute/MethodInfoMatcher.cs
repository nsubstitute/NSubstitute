namespace NSubstitute
{
    public class MethodInfoMatcher : ICallMatcher
    {
        public bool IsMatch(ICall first, ICall second)
        {
            return first.GetMethodInfo() == second.GetMethodInfo();
        }
    }
}