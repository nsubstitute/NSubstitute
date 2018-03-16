using System;

namespace NSubstitute.Core
{
    public interface ISubstituteFactory
    {
        object Create(Type[] typesToProxy, object[] constructorArguments); 
        object CreatePartial(Type[] typesToProxy, object[] constructorArguments); 
    }
}