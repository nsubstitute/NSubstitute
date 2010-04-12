using System.Collections.Generic;

namespace NSubstitute
{
    public interface ICallSpecificationFactory
    {
        ICallSpecification CreateFrom(ICall call);
    }
}