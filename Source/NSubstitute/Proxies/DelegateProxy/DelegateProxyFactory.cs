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
            var delegateMethod = typeof (TDelegate).GetMethod("Invoke");
            var delegateTo = DelegateCall.DelegateCallInvoke;

            ParameterExpression[] parameters = delegateMethod.GetParameters().Select(x => Expression.Parameter(x.ParameterType, x.Name)).ToArray();
            Expression[] parameterInitialisers = parameters.Select(x => (Expression)Expression.Convert(x, typeof(object))).ToArray();

            var delegateExpression =
                Expression.Lambda<TDelegate>(
                    Expression.Convert(
                        Expression.Call(
                            Expression.Constant(delegateCall),
                            delegateTo,
                            new Expression[]
                            {
                                Expression.NewArrayInit(typeof(object), parameterInitialisers)
                            }
                        ),
                        delegateMethod.ReturnType
                    ),
                    parameters
                );
            return delegateExpression.Compile();
        }
    }
}