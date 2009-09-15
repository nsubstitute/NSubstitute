using System;
using LinFu.AOP.Interfaces;

namespace NSubstituteSpike
{
    public class LinFuSubstituteInterceptor : IInterceptor
    {
        public object Intercept(IInvocationInfo info)
        {
            Console.WriteLine("Intercept: " + info.TargetMethod.Name);
            if (info.ReturnType == typeof(void)) return null;
            if (info.ReturnType.IsValueType) return Activator.CreateInstance(info.ReturnType);
            return null;
        }
    }

}