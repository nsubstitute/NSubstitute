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
            IProxyGenerator proxyGenerator;

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
                proxyGenerator = mock<IProxyGenerator>();
                var invocationHandler = mock<IInvocationHandler>();

                invocationHandlerFactory.stub(x => x.CreateInvocationHandler(context)).Return(invocationHandler);
                proxyGenerator.stub(x => x.GenerateProxy<Foo>(invocationHandler)).Return(proxy);
            }

            public override SubstituteFactory CreateSubjectUnderTest()
            {
                return new SubstituteFactory(context, invocationHandlerFactory, proxyGenerator);
            }
        }
    }
}