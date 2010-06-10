using System;

namespace NSubstitute.Core
{
    public interface ISubstituteFactory
    {
        T Create<T>(Type[] additionalInterfaces, object[] constructorArguments) where T : class;
        ICallRouter GetCallRouterCreatedFor(object substitute);
    }
}