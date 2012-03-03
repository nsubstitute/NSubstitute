using System.Collections.Generic;
using System.Reflection;

namespace NSubstitute.Core
{
    public interface IArgumentParamsExtractorFactory
    {
        IArgumentParamsExtractor Create(MethodInfo methodInfo, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight);
    }
}
