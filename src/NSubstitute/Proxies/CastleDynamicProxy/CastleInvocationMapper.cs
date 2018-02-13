using System;
using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleInvocationMapper
    {
        readonly ICallFactory _callFactory;
        private readonly IArgumentSpecificationDequeue _argSpecificationDequeue;

        public CastleInvocationMapper(ICallFactory callFactory, IArgumentSpecificationDequeue argSpecificationDequeue)
        {
            _callFactory = callFactory;
            _argSpecificationDequeue = argSpecificationDequeue;
        }

        public virtual ICall Map(IInvocation castleInvocation)
        {
            Func<object> baseMethod = null;
            if (castleInvocation.InvocationTarget != null &&
                castleInvocation.MethodInvocationTarget.IsVirtual &&
                !castleInvocation.MethodInvocationTarget.IsAbstract &&
                !castleInvocation.MethodInvocationTarget.IsFinal)
            {
                Func<object> baseResult = () => { castleInvocation.Proceed(); return castleInvocation.ReturnValue; };
                var result = new Lazy<object>(baseResult);
                baseMethod = () => result.Value;
            }

            var queuedArgSpecifications = _argSpecificationDequeue.DequeueAllArgumentSpecificationsForMethod(castleInvocation.Method);
            return _callFactory.Create(castleInvocation.Method, castleInvocation.Arguments, castleInvocation.Proxy, queuedArgSpecifications, baseMethod);
        }
    }
}