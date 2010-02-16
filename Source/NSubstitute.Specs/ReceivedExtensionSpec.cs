using NSubstitute.Specs.TestInfrastructure;
using NSubstitute.Specs.TestStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ReceivedExtensionSpec
    {
        public class When_received_is_called_on_a_substitute : StaticConcern
        {
            private IInvocationHandler _handlerForSubstitute;
            private IFoo _substitute;
            private ISubstitutionContext _context;

            [Test]
            public void Should_set_tell_the_substitute_to_assert_that_the_next_invocation_has_been_received()
            {
                _handlerForSubstitute.received(x => x.AssertNextCallHasBeenReceived());
            }

            public override void Because()
            {
                _substitute.Received();
            }

            public override void Context()
            {
                _handlerForSubstitute = mock<IInvocationHandler>();
                _substitute = mock<IFoo>();
                _context = mock<ISubstitutionContext>();

                _context.stub(x => x.GetInvocationHandlerFor(_substitute)).Return(_handlerForSubstitute);

                temporarilyChange(SubstitutionContext.Current)
                    .to(_context)
                    .via(x => SubstitutionContext.Current = x);                    
            }
        }        
    }
}