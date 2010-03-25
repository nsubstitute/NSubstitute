using System.Collections.Generic;

namespace NSubstitute
{
    public interface ICallSpecificationFactory
    {
        ICallSpecification Create(ICall call, IList<IArgumentMatcher> argumentMatchers);
    }
}