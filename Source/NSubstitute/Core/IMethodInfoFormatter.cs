using System.Collections.Generic;
using System.Reflection;

namespace NSubstitute.Core
{
    public interface IMethodInfoFormatter
    {
        bool CanFormat(MethodInfo methodInfo);
        string Format(MethodInfo methodInfo, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight);
    }
}