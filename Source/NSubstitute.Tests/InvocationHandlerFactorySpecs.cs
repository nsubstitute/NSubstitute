using NSubstitute.Tests.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Tests
{
    public class InvocationHandlerFactorySpecs
    {
        public class When_creating_invocation_handler : ConcernFor<InvocationHandlerFactory>
        {
            IInvocationHandler result;
            ISubstitutionContext context;

            [Test]
            public void Should_create_an_instance_of_a_handler_using_the_given_context()
            {
                Assert.That(result, Is.TypeOf<InvocationHandler>());
            }

            public override void Because()
            {
                result = sut.CreateInvocationHandler(context);
            }

            public override void Context()
            {
                context = mock<ISubstitutionContext>();
            }

            public override InvocationHandlerFactory CreateSubjectUnderTest()
            {
                return new InvocationHandlerFactory();
            }
        }
    }
}