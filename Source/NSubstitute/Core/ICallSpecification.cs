using System.Collections.Generic;
using System.Reflection;

namespace NSubstitute.Core
{
    public interface ICallSpecification
    {
        IEnumerable<IArgumentSpecification> ArgumentSpecifications { get; }
        MethodInfo MethodInfo { get; }
        bool IsSatisfiedBy(ICall call);
    }
}