using System.Collections.Generic;
using System.Reflection;

namespace NSubstitute.Core
{
    public interface ICallFormatter
    {
        string Format(MethodInfo methodInfoOfCall, IEnumerable<object> arguments);
        string Format(ICall call);
    }
}