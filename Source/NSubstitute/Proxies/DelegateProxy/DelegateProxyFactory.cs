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
            return DelegateProxy<T>(delegateCall);
        }

        private TDelegate DelegateProxy<TDelegate>(DelegateCall delegateCall)
        {
            var delegateMethodToProxy = typeof (TDelegate).GetMethod("Invoke");
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
            
            var proxyExpression = Expression.Lambda<TDelegate>(callInvokeOnDelegateCallInstance, proxyParameters);
            return proxyExpression.Compile();
        }
    }
}