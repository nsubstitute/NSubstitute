extern alias CastleCore;
using NSubstitute.Specs.TestInfrastructure;
using CastleInvocation = CastleCore::Castle.Core.Interceptor.IInvocation;
using NSubstitute.Proxies.CastleDynamicProxy;
using NUnit.Framework;

namespace NSubstitute.Specs.Proxies.CastleDynamicProxy
{
    public class CastleForwardingInterceptorSpecs
    {
        public class When_intercepting_a_castle_invocation : ConcernFor<CastleForwardingInterceptor>
        {
            IInvocationHandler invocationHandler;
            IInvocation mappedInvocation;
            CastleInvocation castleInvocation;
            CastleInvocationMapper invocationMapper;
            object returnValue;

            [Test]
            public void Should_forward_mapped_invocation_to_invocation_handler()
            {
                invocationHandler.received(x => x.HandleInvocation(mappedInvocation));
            }

            [Test]
            public void Should_set_return_value_from_invocation_handler()
            {
                Assert.That(castleInvocation.ReturnValue, Is.SameAs(returnValue));
            }

            public override void Because()
            {
                sut.Intercept(castleInvocation);
            }

            public override void Context()
            {
                returnValue = new object();
                invocationHandler = mock<IInvocationHandler>();
                castleInvocation = mock<CastleInvocation>();
                mappedInvocation = mock<IInvocation>();
                invocationMapper = mock<CastleInvocationMapper>();
                invocationMapper.stub(x => x.Map(castleInvocation)).Return(mappedInvocation);
                invocationHandler.stub(x => x.HandleInvocation(mappedInvocation)).Return(returnValue);
            }

            public override CastleForwardingInterceptor CreateSubjectUnderTest()
            {
                return new CastleForwardingInterceptor(invocationMapper, invocationHandler);
            }
        }
    }
}