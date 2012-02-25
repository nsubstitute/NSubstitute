using System.Collections.Generic;
using System.Reflection;

namespace NSubstitute.Core.Arguments
{
    public interface IArgumentFormatInfoFactory
    {
        IEnumerable<IArgumentFormatInfo> CreateArgumentFormatInfos(MethodInfo methodInfoOfCall, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight);
    }
}