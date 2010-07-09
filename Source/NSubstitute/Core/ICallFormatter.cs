using System.Collections.Generic;
using System.Reflection;

namespace NSubstitute.Core
{
    public interface ICallFormatter
    {
        string Format(ICall call, ICallSpecification withRespectToCallSpec);
        string Format(MethodInfo methodInfoOfCall, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight);
    }
}