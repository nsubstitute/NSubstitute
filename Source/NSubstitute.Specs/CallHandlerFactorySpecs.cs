using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallHandlerFactorySpecs
    {
        public class When_creating_call_handler : ConcernFor<CallHandlerFactory>
        {
            ICallHandler _result;
            ISubstitutionContext _context;

            [Test]
            public void Should_create_an_instance_of_a_handler_using_the_given_context()
            {
                Assert.That(_result, Is.TypeOf<CallHandler>());
            }

            public override void Because()
            {
                _result = sut.CreateCallHandler(_context);
            }

            public override void Context()
            {
                _context = mock<ISubstitutionContext>();
            }

            public override CallHandlerFactory CreateSubjectUnderTest()
            {
                return new CallHandlerFactory();
            }
        }
    }
}