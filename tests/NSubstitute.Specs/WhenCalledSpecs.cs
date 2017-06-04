using System;
using NSubstitute.Core;
using NSubstitute.Routing;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class WhenCalledSpecs
    {
        public class When_a_callback_is_given_to_run_for_a_call : ConcernFor<WhenCalled<IFoo>> {
            private Action<CallInfo> _callbackWithArguments;
            private ISubstitutionContext _context;
            private IFoo _substitute;
            private Action<IFoo> _call;
            private TestCallRouter _callRouter;
            private MatchArgs _matchArgs;
            private IRouteFactory _routeFactory;

            [Test]
            public void Should_set_route_to_do_action_when_called_and_invoke_call()
            {
                var state = mock<ISubstituteState>();
                _callRouter.FactoryMethodUsedToSetRoute(state);

                _routeFactory.received(x => x.DoWhenCalled(state, _callbackWithArguments, _matchArgs));
                _call.received(x => x(_substitute));
            }

            public override void Because()
            {
                sut.Do(_callbackWithArguments);
            }

            public override void Context()
            {
                _call = mock<Action<IFoo>>();
                _callbackWithArguments = args => { };
                _matchArgs = MatchArgs.AsSpecifiedInCall; 

                _callRouter = new TestCallRouter();
                _context = mock<ISubstitutionContext>();
                _routeFactory = mock<IRouteFactory>();
                _substitute = mock<IFoo>();

                _context.stub(x => x.GetCallRouterFor(_substitute)).Return(_callRouter);
                _context.stub(x => x.GetRouteFactory()).Return(_routeFactory);
            }

            public override WhenCalled<IFoo> CreateSubjectUnderTest()
            {
                return new WhenCalled<IFoo>(_context, _substitute, _call, _matchArgs);
            }
        }
    }
}