using NSubstitute.Tests.TestInfrastructure;
using NSubstitute.Tests.TestStructures;
using NUnit.Framework;

namespace NSubstitute.Tests
{
    public class SubstituteFactorySpecs
    {
        public class When_creating_a_substitute_for_a_type : ConcernFor<SubstituteFactory>
        {
            Foo proxy;
            Foo result;
            IInvocationHandlerFactory invocationHandlerFactory;
            ISubstitutionContext context;
            IProxyFactory _proxyFactory;

            [Test]
            public void Should_return_a_proxy_for_that_type()
            {
                Assert.That(result, Is.SameAs(proxy));
            }

            public override void Because()
            {
                result = sut.Create<Foo>();
            }

            public override void Context()
            {
                proxy = new Foo();
                context = mock<ISubstitutionContext>();
                invocationHandlerFactory = mock<IInvocationHandlerFactory>();
                _proxyFactory = mock<IProxyFactory>();
                var invocationHandler = mock<IInvocationHandler>();

                invocationHandlerFactory.stub(x => x.CreateInvocationHandler(context)).Return(invocationHandler);
                _proxyFactory.stub(x => x.GenerateProxy<Foo>(invocationHandler)).Return(proxy);
            }

            public override SubstituteFactory CreateSubjectUnderTest()
            {
                return new SubstituteFactory(context, invocationHandlerFactory, _proxyFactory);
            }
        }
    }
}