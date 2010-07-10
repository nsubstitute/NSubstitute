using System;
using System.Collections.Generic;
using System.Reflection;

namespace NSubstitute.Core
{
    public interface IArgumentSpecificationFactory
    {
        IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object[] arguments, Type[] parameterTypes, MatchArgs matchArgs);
    }
}