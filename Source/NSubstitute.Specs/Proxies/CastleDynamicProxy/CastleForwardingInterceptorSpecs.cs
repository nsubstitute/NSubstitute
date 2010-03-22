extern alias CastleCore;
using NSubstitute.Specs.Infrastructure;
using CastleInvocation = CastleCore::Castle.Core.Interceptor.IInvocation;
using NSubstitute.Proxies.CastleDynamicProxy;
using NUnit.Framework;

namespace NSubstitute.Specs.Proxies.CastleDynamicProxy
{
    public class CastleForwardingInterceptorSpecs
    {
        public class When_intercepting_a_castle_invocation : ConcernFor<CastleForwardingInterceptor>
        {
            ICallHandler callHandler;
            ICall _mappedCall;
            CastleInvocation castleInvocation;
            CastleInvocationMapper invocationMapper;
            object returnValue;

            [Test]
            public void Should_forward_mapped_call_to_call_handler()
            {
                callHandler.received(x => x.Handle(_mappedCall));
            }

            [Test]
            public void Should_set_return_value_from_call_handler()
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
                callHandler = mock<ICallHandler>();
                castleInvocation = mock<CastleInvocation>();
                _mappedCall = mock<ICall>();
                invocationMapper = mock<CastleInvocationMapper>();
                invocationMapper.stub(x => x.Map(castleInvocation)).Return(_mappedCall);
                callHandler.stub(x => x.Handle(_mappedCall)).Return(returnValue);
            }

            public override CastleForwardingInterceptor CreateSubjectUnderTest()
            {
                return new CastleForwardingInterceptor(invocationMapper, callHandler);
            }
        }
    }
}