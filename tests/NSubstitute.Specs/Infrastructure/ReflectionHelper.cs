using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NSubstitute.Specs.Infrastructure
{
    public class ReflectionHelper
    {
        public static MethodInfo GetMethod(Expression<Action> methodCall)
        {
            var methodCallExpression = (MethodCallExpression) methodCall.Body;
            return methodCallExpression.Method;
        }
    }
}