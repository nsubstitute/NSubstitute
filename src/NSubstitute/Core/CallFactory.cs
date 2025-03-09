using NSubstitute.Core.Arguments;
using System.Reflection;

namespace NSubstitute.Core;

internal sealed class CallFactory : ICallFactory
{
    public ICall Create(MethodInfo methodInfo, object?[] arguments, object target, IList<IArgumentSpecification> argumentSpecifications, Func<object>? baseMethod)
    {
        return new Call(methodInfo, arguments, target, argumentSpecifications, baseMethod);
    }

    public ICall Create(MethodInfo methodInfo, object?[] arguments, object target, IList<IArgumentSpecification> argumentSpecifications)
    {
        return new Call(methodInfo, arguments, target, argumentSpecifications, baseMethod: null);
    }
}