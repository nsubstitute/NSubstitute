using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.Proxies.DelegateProxy
{
    public class DelegateProxyFactory : IProxyFactory
    {
        public object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[] additionalInterfaces, object[] constructorArguments)
        {
            if (HasItems(additionalInterfaces))
            {
                throw new SubstituteException(
                    "Can not specify additional interfaces when substituting for a delegate. You must specify only a single delegate type if you need to substitute for a delegate.");
            }
            if (HasItems(constructorArguments))
            {
                throw new SubstituteException("Can not provide constructor arguments when substituting for a delegate.");
            }

            return DelegateProxy(typeToProxy, callRouter);
        }

        private bool HasItems<T>(T[] array)
        {
            return array != null && array.Length > 0;
        }

        private object DelegateProxy(Type delegateType, ICallRouter callRouter)
        {
            var delegateMethodToProxy = delegateType.GetMethod("Invoke");

            var proxyParameterTypes = delegateMethodToProxy.GetParameters().Select(x => new ParameterInfoWrapper(x)).ToArray();
            
            var delegateCall = new DelegateCall(callRouter, delegateType, delegateMethodToProxy.ReturnType, proxyParameterTypes);
            var invokeOnDelegateCall = delegateCall.MethodToInvoke;

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