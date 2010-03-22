using System.Collections.Generic;
using System.Reflection;

namespace NSubstitute
{
    public class CallSpecification: ICallSpecification
    {
        readonly List<IArgumentMatcher> _argumentMatchers;
        public MethodInfo MethodInfo { get; private set; }

        public CallSpecification(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            _argumentMatchers = new List<IArgumentMatcher>();
        }

        public IList<IArgumentMatcher> ArgumentMatchers
        {
            get { return _argumentMatchers; }
        }
    }
}