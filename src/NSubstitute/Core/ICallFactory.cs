using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public interface ICallFactory
    {
        ICall Create(MethodInfo methodInfo, object?[] arguments, object target, IList<IArgumentSpecification> argumentSpecifications, Func<object>? baseMethod);
        ICall Create(MethodInfo methodInfo, object?[] getterArgs, object target, IList<IArgumentSpecification> getArgumentSpecifications);
    }
}