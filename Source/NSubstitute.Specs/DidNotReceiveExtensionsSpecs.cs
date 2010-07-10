using System;
using NSubstitute.Core;
using NSubstitute.Routes;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class DidNotReceiveExtensionsSpecs
    {
        public class When_did_not_receive_is_called_on_substitute : StaticConcern
        {
            private ICallRouter _routerForSubstitute;
            private object _substitute;

            [Test]
            public void Should_tell_substitute_to_assert_next_call_was_not_received_with_specified_arguments()
            {
                _routerForSubstitute.received(x => x.SetRoute<CheckCallNotReceivedRoute>(MatchArgs.AsSpecifiedInCall));
            }

            public override void Because()
            {
                _substitute.DidNotReceive();
            }

            public override void Context()
            {
                _substitute = new object();
                _routerForSubstitute = mock<ICallRouter>();
                var context = mock<ISubstitutionContext>();
                context.stub(x => x.GetCallRouterFor(_substitute)).Return(_routerForSubstitute);
                temporarilyChange(() => SubstitutionContext.Current).to(context);
            }
        }
    }
}