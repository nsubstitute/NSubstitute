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
            ISubstituteBuilder substituteBuilder;
            IInvocationHandlerFactory invocationHandlerFactory;
            ISubstitutionContext context;

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
                substituteBuilder = mock<ISubstituteBuilder>();
                var interceptor = mock<ISubstituteInterceptor>();
                var invocationHandler = mock<IInvocationHandler>();

                invocationHandlerFactory.stub(x => x.CreateInvocationHandler(context)).Return(invocationHandler);
                substituteBuilder.stub(x => x.CreateInterceptor(invocationHandler)).Return(interceptor);
                substituteBuilder.stub(x => x.GenerateProxy<Foo>(interceptor)).Return(proxy);
            }

            public override SubstituteFactory CreateSubjectUnderTest()
            {
                return new SubstituteFactory(context, invocationHandlerFactory, substituteBuilder);
            }
        }
    }
}