using System;
using LinFu.AOP.Interfaces;

namespace NSubstituteSpike
{
    public class SubstituteInterceptor : IInterceptor
    {
        public object Intercept(IInvocationInfo info)
        {
            Console.WriteLine("Intercept: " + info.TargetMethod.Name);
            return Activator.CreateInstance(info.ReturnType);
        }
    }
}