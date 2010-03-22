using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallHandlerFactorySpecs
    {
        public class When_creating_call_handler : ConcernFor<CallHandlerFactory>
        {
            ICallHandler result;
            ISubstitutionContext context;

            [Test]
            public void Should_create_an_instance_of_a_handler_using_the_given_context()
            {
                Assert.That(result, Is.TypeOf<CallHandler>());
            }

            public override void Because()
            {
                result = sut.CreateCallHandler(context);
            }

            public override void Context()
            {
                context = mock<ISubstitutionContext>();
            }

            public override CallHandlerFactory CreateSubjectUnderTest()
            {
                return new CallHandlerFactory();
            }
        }
    }
}