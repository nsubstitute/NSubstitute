using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallRouterFactorySpecs
    {
        public class When_creating : ConcernFor<CallRouterFactory>
        {
            ICallRouter _result;
            ISubstitutionContext _context;

            [Test]
            public void Should_create_an_instance_using_the_given_context()
            {
                Assert.That(_result, Is.TypeOf<CallRouter>());
            }

            public override void Because()
            {
                _result = sut.Create(_context);
            }

            public override void Context()
            {
                _context = mock<ISubstitutionContext>();
            }

            public override CallRouterFactory CreateSubjectUnderTest()
            {
                return new CallRouterFactory();
            }
        }
    }
}