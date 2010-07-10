using NSubstitute.Core;
using NSubstitute.Routes;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class DidNotReceiveWithAnyArgsExtensionSpec : StaticConcern
    {
            private ICallRouter _routerForSubstitute;
            private IFoo _substitute;
            private ISubstitutionContext _context;

            [Test]
            public void Should_set_tell_the_substitute_to_assert_that_the_next_call_has_not_been_received_with_any_arguments()
            {
                _routerForSubstitute.received(x => x.SetRoute<CheckCallNotReceivedRoute>(MatchArgs.Any));
            }

            public override void Because()
            {
                _substitute.DidNotReceiveWithAnyArgs();
            }

            public override void Context()
            {
                _routerForSubstitute = mock<ICallRouter>();
                _substitute = mock<IFoo>();
                _context = mock<ISubstitutionContext>();

                _context.stub(x => x.GetCallRouterFor(_substitute)).Return(_routerForSubstitute);

                temporarilyChange(() => SubstitutionContext.Current).to(_context);        
            }
    }
}