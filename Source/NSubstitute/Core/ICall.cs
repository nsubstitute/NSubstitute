using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public interface ICall
    {
        Type GetReturnType();
        MethodInfo GetMethodInfo();
        object[] GetArguments();
        object[] GetOriginalArguments();
        object Target();
        IParameterInfo[] GetParameterInfos();
        IList<IArgumentSpecification> GetArgumentSpecifications();
        void AssignSequenceNumber(long number);
        long GetSequenceNumber();
        Maybe<object> TryCallBase();
    }
}
