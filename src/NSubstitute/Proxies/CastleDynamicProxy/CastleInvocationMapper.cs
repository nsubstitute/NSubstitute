using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy;

public class CastleInvocationMapper(ICallFactory callFactory, IArgumentSpecificationDequeue argSpecificationDequeue)
{
    public virtual ICall Map(IInvocation castleInvocation)
    {
        Func<object>? baseMethod = null;
        if (castleInvocation.InvocationTarget != null &&
            castleInvocation.MethodInvocationTarget.IsVirtual &&
            !castleInvocation.MethodInvocationTarget.IsAbstract)
        {
            baseMethod = CreateBaseResultInvocation(castleInvocation);
        }

        var queuedArgSpecifications = argSpecificationDequeue.DequeueAllArgumentSpecificationsForMethod(castleInvocation.Arguments.Length);
        return callFactory.Create(castleInvocation.Method, castleInvocation.Arguments, castleInvocation.Proxy, queuedArgSpecifications, baseMethod);
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
