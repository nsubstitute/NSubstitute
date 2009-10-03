extern alias CastleCore;
using IInterceptor = CastleCore::Castle.Core.Interceptor.IInterceptor;
using NSubstitute.Proxies.CastleDynamicProxy;
using NSubstitute.Tests.TestInfrastructure;
using NSubstitute.Tests.TestStructures;
using NUnit.Framework;

namespace NSubstitute.Tests.Proxies.CastleDynamicProxy
{
    public class CastleDynamicProxyFactorySpecs
    {
        public class When_creating_a_proxy : ConcernFor<CastleDynamicProxyFactory>
        {
            IInterceptor interceptor;
            IInvocationHandler invocationHandler;
            CastleInterceptorFactory interceptorFactory;
            CastleProxyGeneratorWrapper proxyGeneratorWrapper;
            IFoo proxy;
            IFoo result;

            [Test]
            public void Should_generate_a_proxy_using_an_interceptor_that_forwards_to_the_given_invocation_handler()
            {
                Assert.That(result, Is.SameAs(proxy));
            }

            public override void Because()
            {
                result = sut.GenerateProxy<IFoo>(invocationHandler);
            }

            public override void Context()
            {
                invocationHandler = mock<IInvocationHandler>();
                interceptor = mock<IInterceptor>();
                proxy = mock<IFoo>();                
                
                interceptorFactory = mock<CastleInterceptorFactory>();
                interceptorFactory.stub(x => x.CreateForwardingInterceptor(invocationHandler)).Return(interceptor);
                
                proxyGeneratorWrapper = mock<CastleProxyGeneratorWrapper>();
                proxyGeneratorWrapper.stub(x => x.CreateProxyForInterface<IFoo>(interceptor)).Return(proxy);
            }

            public override CastleDynamicProxyFactory CreateSubjectUnderTest()
            {
                return new CastleDynamicProxyFactory(proxyGeneratorWrapper, interceptorFactory);
            }
        }
    }
}