using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public interface ICallSpecification
    {
        bool IsSatisfiedBy(ICall call);
        string Format(ICall call);
        ICallSpecification CreateCopyThatMatchesAnyArguments();
        void InvokePerArgumentActions(CallInfo callInfo);
        IEnumerable<ArgumentMatchInfo> NonMatchingArguments(ICall call);
        MethodInfo GetMethodInfo();
        Type ReturnType();
        /// <summary>
        /// Determines whether the specified call specification is similar.
        /// </summary>
        /// <param name="spec">The call specification.</param>
        /// <returns></returns>
        bool IsSimilar(ICallSpecification spec);
    }
}