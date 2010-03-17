using NSubstitute.Specs.TestInfrastructure;
using NSubstitute.Specs.TestStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class SubstituteFactorySpecs
    {
        public class When_creating_a_substitute_for_a_type : ConcernFor<SubstituteFactory>
        {
            Foo proxy;
            Foo result;
            ICallHandlerFactory _callHandlerFactory;
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
                _callHandlerFactory = mock<ICallHandlerFactory>();
                _proxyFactory = mock<IProxyFactory>();
                var callHandler = mock<ICallHandler>();

                _callHandlerFactory.stub(x => x.CreateCallHandler(context)).Return(callHandler);
                _proxyFactory.stub(x => x.GenerateProxy<Foo>(callHandler)).Return(proxy);
            }

            public override SubstituteFactory CreateSubjectUnderTest()
            {
                return new SubstituteFactory(context, _callHandlerFactory, _proxyFactory);
            }
        }
    }
}