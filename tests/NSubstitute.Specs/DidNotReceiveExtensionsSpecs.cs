using System;
using NSubstitute.Core;
using NSubstitute.Routing;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class DidNotReceiveExtensionsSpecs
    {
        public class When_did_not_receive_is_called_on_substitute : StaticConcern
        {
            private TestCallRouter _routerForSubstitute;
            private object _substitute;
            private IRouteFactory _routeFactory;

            [Test]
            public void Should_set_route_to_assert_next_call_was_not_received_with_specified_arguments()
            {
                var state = mock<ISubstituteState>();
                _routerForSubstitute.FactoryMethodUsedToSetRoute(state);
                _routeFactory.received(x => x.CheckReceivedCalls(state, MatchArgs.AsSpecifiedInCall, Quantity.None()));
            }

            public override void Because()
            {
                _substitute.DidNotReceive();
            }

            public override void Context()
            {
                _substitute = new object();
                _routerForSubstitute = new TestCallRouter();
                _routeFactory = mock<IRouteFactory>();
                var context = mock<ISubstitutionContext>();
                context.stub(x => x.GetCallRouterFor(_substitute)).Return(_routerForSubstitute);
                context.stub(x => x.GetRouteFactory()).Return(_routeFactory);
                temporarilyChange(() => SubstitutionContext.Current).to(context);
            }
        }
    }
}