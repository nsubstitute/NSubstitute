using System;
using System.Linq.Expressions;

namespace NSubstitute.Core
{
    public interface ICallSpecificationFactory
    {
        ICallSpecification CreateFrom(ICall call, MatchArgs matchArgs);
        ICallSpecification CreateFrom<T>(Expression<Action<T>> call);
    }
}