extern alias CastleCore;
using CastleInvocation = CastleCore::Castle.Core.Interceptor.IInvocation;
using NSubstitute.Proxies.CastleDynamicProxy;
using NSubstitute.Tests.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Tests.Proxies.CastleDynamicProxy
{
    public class CastleForwardingInterceptorSpecs
    {
        public class When_intercepting_a_castle_invocation : ConcernFor<CastleForwardingInterceptor>
        {
            IInvocationHandler invocationHandler;
            IInvocation mappedInvocation;
            CastleInvocation castleInvocation;
            CastleInvocationMapper invocationMapper;

            [Test]
            public void Should_forward_mapped_invocation_to_invocation_handler()
            {
                invocationHandler.received(x => x.HandleInvocation(mappedInvocation));
            }

            public override void Because()
            {
                sut.Intercept(castleInvocation);
            }

            public override void Context()
            {
                invocationHandler = mock<IInvocationHandler>();
                castleInvocation = mock<CastleInvocation>();
                mappedInvocation = mock<IInvocation>();
                invocationMapper = mock<CastleInvocationMapper>();
                invocationMapper.stub(x => x.Map(castleInvocation)).Return(mappedInvocation);
            }

            public override CastleForwardingInterceptor CreateSubjectUnderTest()
            {
                return new CastleForwardingInterceptor(invocationMapper, invocationHandler);
            }
        }
    }
}