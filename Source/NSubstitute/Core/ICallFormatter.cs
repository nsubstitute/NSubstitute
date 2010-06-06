using System.Reflection;

namespace NSubstitute.Core
{
    public interface ICallFormatter
    {
        string Format(MethodInfo methodInfoOfCall);
    }
}