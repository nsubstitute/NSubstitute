using System;
using System.Linq;
using System.Linq.Expressions;
using NSubstitute.Core;

namespace NSubstitute.Proxies.DelegateProxy
{
    public class DelegateProxyFactory : IProxyFactory
    {
        public T GenerateProxy<T>(ICallRouter callRouter) where T : class
        {
            var delegateCall = new DelegateCall(callRouter);
            return (T) DelegateProxy(typeof(T), delegateCall);
        }

        public object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[] additionalInterfaces, object[] constructorArguments)
        {
            var delegateCall = new DelegateCall(callRouter);
            return DelegateProxy(typeToProxy, delegateCall);
        }

        private object DelegateProxy(Type delegateType, DelegateCall delegateCall)
        {
            var delegateMethodToProxy = delegateType.GetMethod("Invoke");
            var invokeOnDelegateCall = DelegateCall.DelegateCallInvoke;

            ParameterExpression[] proxyParameters = delegateMethodToProxy.GetParameters().Select(x => Expression.Parameter(x.ParameterType, x.Name)).ToArray();
            Expression[] proxyParametersAsObjects = proxyParameters.Select(x => (Expression)Expression.Convert(x, typeof(object))).ToArray();

            Expression callInvokeOnDelegateCallInstance = 
                Expression.Call(
                    Expression.Constant(delegateCall),
                    invokeOnDelegateCall,
                    new Expression[]
                    {
                        Expression.NewArrayInit(typeof(object), proxyParametersAsObjects)
                    }
                );

            if (delegateMethodToProxy.ReturnType != typeof(void))
            {
                callInvokeOnDelegateCallInstance =
                    Expression.Convert(callInvokeOnDelegateCallInstance, delegateMethodToProxy.ReturnType);
            }
            
            var proxyExpression = Expression.Lambda(delegateType, callInvokeOnDelegateCallInstance, proxyParameters);
            return proxyExpression.Compile();
        }
    }
}