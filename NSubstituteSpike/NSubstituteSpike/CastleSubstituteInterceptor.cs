using System;
using Castle.Core.Interceptor;

namespace NSubstituteSpike
{
    public class CastleSubstituteInterceptor : IInterceptor
    {        
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("Intercept: " + invocation.Method.Name);
            if (invocation.Method.ReturnType == typeof(void)) return;
            invocation.ReturnValue = 
                invocation.Method.ReturnType.IsValueType 
                ? Activator.CreateInstance(invocation.Method.ReturnType) 
                : null;
        }
    }
}