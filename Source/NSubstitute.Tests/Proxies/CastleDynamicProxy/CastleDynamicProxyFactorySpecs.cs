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
        public abstract class Concern : ConcernFor<CastleDynamicProxyFactory>
        {
            protected IInterceptor interceptor;
            protected IInvocationHandler invocationHandler;
            protected CastleProxyGeneratorWrapper proxyGeneratorWrapper;
            protected CastleInterceptorFactory interceptorFactory;

            public override void Context()
            {
                proxyGeneratorWrapper = mock<CastleProxyGeneratorWrapper>();
                invocationHandler = mock<IInvocationHandler>();
                interceptor = mock<IInterceptor>();
                interceptorFactory = mock<CastleInterceptorFactory>();
                interceptorFactory.stub(x => x.CreateForwardingInterceptor(invocationHandler)).Return(interceptor);
            }

            public override CastleDynamicProxyFactory CreateSubjectUnderTest()
            {
                return new CastleDynamicProxyFactory(proxyGeneratorWrapper, interceptorFactory);
            }
        }

        public class When_creating_a_proxy_for_an_interface : Concern
        {
            IFoo proxy;
            IFoo result;

            [Test]
            public void Should_generate_a_proxy_for_that_interface()
            {
                Assert.That(result, Is.SameAs(proxy));
            }

            public override void Because()
            {
                result = sut.GenerateProxy<IFoo>(invocationHandler);
            }

            public override void Context()
            {
                base.Context();
                proxy = mock<IFoo>();                                
                proxyGeneratorWrapper.stub(x => x.CreateProxyForInterface<IFoo>(interceptor)).Return(proxy);
            }            
        }

        public class When_creating_a_proxy_for_a_class : Concern
        {
            Foo proxy;
            Foo result;

            [Test]
            public void Should_generate_a_proxy_for_that_class()
            {
                Assert.That(result, Is.SameAs(proxy));
            }

            public override void Because()
            {
                result = sut.GenerateProxy<Foo>(invocationHandler);
            }

            public override void Context()
            {
                base.Context();
                proxy = mock<Foo>();
                proxyGeneratorWrapper.stub(x => x.CreateProxyForClass<Foo>(interceptor)).Return(proxy);
            }            

            
        }
    }
}