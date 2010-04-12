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
            ICallRouterFactory _callRouterFactory;
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
                _callRouterFactory = mock<ICallRouterFactory>();
                _proxyFactory = mock<IProxyFactory>();
                var callRouter = mock<ICallRouter>();

                _callRouterFactory.stub(x => x.Create(_context)).Return(callRouter);
                _proxyFactory.stub(x => x.GenerateProxy<Foo>(callRouter)).Return(_proxy);
            }

            public override SubstituteFactory CreateSubjectUnderTest()
            {
                return new SubstituteFactory(_context, _callRouterFactory, _proxyFactory);
            }
        }
    }
}