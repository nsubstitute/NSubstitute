using System.Collections.Generic;
using System.Reflection;

namespace NSubstitute
{
    public interface ICallSpecification
    {
        IList<IArgumentMatcher> ArgumentMatchers { get; }
        MethodInfo MethodInfo { get; }
    }
}