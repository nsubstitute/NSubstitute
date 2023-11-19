using System;
using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleInvocationMapper
    {
        private readonly ICallFactory _callFactory;
        private readonly IArgumentSpecificationDequeue _argSpecificationDequeue;

        public CastleInvocationMapper(ICallFactory callFactory, IArgumentSpecificationDequeue argSpecificationDequeue)
        {
            _callFactory = callFactory;
            _argSpecificationDequeue = argSpecificationDequeue;
        }

        public virtual ICall Map(IInvocation castleInvocation)
        {
            Func<object>? baseMethod = null;
            if (castleInvocation.InvocationTarget != null &&
                castleInvocation.MethodInvocationTarget.IsVirtual &&
                !castleInvocation.MethodInvocationTarget.IsAbstract &&
                !castleInvocation.MethodInvocationTarget.IsFinal)
            {
                baseMethod = CreateBaseResultInvocation(castleInvocation);
            }

            var queuedArgSpecifications = _argSpecificationDequeue.DequeueAllArgumentSpecificationsForMethod(castleInvocation.Arguments.Length);
            return _callFactory.Create(castleInvocation.Method, castleInvocation.Arguments, castleInvocation.Proxy, queuedArgSpecifications, baseMethod);
        }

        private static Func<object> CreateBaseResultInvocation(IInvocation invocation)
        {
            // Notice, it's important to keep this as a separate method, as methods with lambda closures
            // always allocate, even if delegate is not actually constructed.
            // This way we make allocation only if indeed required.
            Func<object> baseResult = () => { invocation.Proceed(); return invocation.ReturnValue; };
            var result = new Lazy<object>(baseResult);
            return () => result.Value;
        }
    }
}
