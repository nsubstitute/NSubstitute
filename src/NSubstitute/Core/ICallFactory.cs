using NSubstitute.Core.Arguments;
using System.Reflection;

namespace NSubstitute.Core;

public interface ICallFactory
{
    ICall Create(MethodInfo methodInfo, object?[] arguments, object target, IList<IArgumentSpecification> argumentSpecifications, Func<object?>? baseMethod);
    ICall Create(MethodInfo methodInfo, object?[] getterArgs, object target, IList<IArgumentSpecification> getArgumentSpecifications);
}