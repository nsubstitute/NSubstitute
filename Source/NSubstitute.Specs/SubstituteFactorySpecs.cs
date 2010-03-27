using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class SubstituteFactorySpecs
    {
        public class When_creating_a_substitute_for_a_type : ConcernFor<SubstituteFactory>
        {
            Foo _proxy;
            Foo _result;
            ICallHandlerFactory _callHandlerFactory;
            ISubstitutionContext _context;
            IProxyFactory _proxyFactory;

            [Test]
            public void Should_return_a_proxy_for_that_type()
            {
                Assert.That(_result, Is.SameAs(_proxy));
            }

            public override void Because()
            {
                _result = sut.Create<Foo>();
            }

            public override void Context()
            {
                _proxy = new Foo();
                _context = mock<ISubstitutionContext>();
                _callHandlerFactory = mock<ICallHandlerFactory>();
                _proxyFactory = mock<IProxyFactory>();
                var callHandler = mock<ICallHandler>();

                _callHandlerFactory.stub(x => x.CreateCallHandler(_context)).Return(callHandler);
                _proxyFactory.stub(x => x.GenerateProxy<Foo>(callHandler)).Return(_proxy);
            }

            public override SubstituteFactory CreateSubjectUnderTest()
            {
                return new SubstituteFactory(_context, _callHandlerFactory, _proxyFactory);
            }
        }
    }
}