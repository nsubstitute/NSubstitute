using System.Collections.Generic;
using System.Linq;

namespace NSubstitute
{
    public class AllInvocationMatcher : IInvocationMatcher
    {
        readonly IEnumerable<IInvocationMatcher> _invocationMatchers;

        public AllInvocationMatcher(IEnumerable<IInvocationMatcher> invocationMatchers)
        {
            _invocationMatchers = invocationMatchers;
        }

        public bool IsMatch(IInvocation first, IInvocation second)
        {
            return _invocationMatchers.All(x => x.IsMatch(first, second));
        }
    }
}