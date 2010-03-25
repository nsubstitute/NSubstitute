using System;
using System.Collections.Generic;

namespace NSubstitute
{
    public interface ISubstitutionContext
    {
        void LastCallShouldReturn<T>(T value, IList<IArgumentMatcher> argumentMatchers);
        void LastCallHandler(ICallHandler callHandler);
        ISubstituteFactory GetSubstituteFactory();
        ICallHandler GetCallHandlerFor(object substitute);
        void AddArgument<T>(Predicate<T> predicate);
        IList<IArgumentMatcher> RetrieveArgumentMatchers();
    }
}