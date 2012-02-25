using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public interface IMethodInfoFormatter
    {
        bool CanFormat(MethodInfo methodInfo);
        string Format(MethodInfo methodInfo, IEnumerable<IArgumentFormatInfo> argumentFormatInfos);
    }
}