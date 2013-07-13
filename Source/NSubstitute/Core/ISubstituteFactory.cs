using System;

namespace NSubstitute.Core
{
    public interface ISubstituteFactory
    {
        object Create(Type[] typesToProxy, object[] constructorArguments); 
        object Create(Type[] typesToProxy, object[] constructorArguments, bool callBaseByDefault); 
        ICallRouter GetCallRouterCreatedFor(object substitute);
    }
}