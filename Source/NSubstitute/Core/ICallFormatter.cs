using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public interface ICallFormatter
    {
        string Format(ICall call, ICallSpecification withRespectToCallSpec);
        string Format(MethodInfo methodInfoOfCall, IEnumerable<object> arguments, IEnumerable<ArgumentMatchInfo> nonMatchingArguments);
    }
}