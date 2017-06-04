using System;
using NSubstitute.Core;
using NSubstitute.Routing;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ReceivedExtensionSpec
    {
        public class When_received_is_called_on_a_substitute : StaticConcern
        {
            private TestCallRouter _routerForSubstitute;
            private IFoo _substitute;
            private ISubstitutionContext _context;
            private IRouteFactory _routeFactory;

            [Test]
            public void Should_set_route_to_assert_that_the_next_call_has_been_received_with_specified_arguments()
            {
                var state = mock<ISubstituteState>();
                _routerForSubstitute.FactoryMethodUsedToSetRoute(state);
                _routeFactory.received(x => x.CheckReceivedCalls(state, MatchArgs.AsSpecifiedInCall, Quantity.AtLeastOne()));
            }

            public override void Because()
            {
                _substitute.Received();
            }

            public override void Context()
            {
                _routerForSubstitute = new TestCallRouter();
                _substitute = mock<IFoo>();
                _context = mock<ISubstitutionContext>();
                _routeFactory = mock<IRouteFactory>();

                _context.stub(x => x.GetCallRouterFor(_substitute)).Return(_routerForSubstitute);
                _context.stub(x => x.GetRouteFactory()).Return(_routeFactory);


                temporarilyChange(() => SubstitutionContext.Current).to(_context);
            }
        }
    }
}