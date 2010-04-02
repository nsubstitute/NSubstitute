using System.Collections.Generic;
using System.Reflection;

namespace NSubstitute
{
    public interface ICallSpecification
    {
        IList<IArgumentSpecification> ArgumentSpecifications { get; }
        MethodInfo MethodInfo { get; }
    }
}