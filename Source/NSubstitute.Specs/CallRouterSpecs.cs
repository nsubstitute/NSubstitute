using System;
using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Routing;
using NSubstitute.Routing.Definitions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallRouterSpecs
    {
        public abstract class Concern : ConcernFor<CallRouter>
        {
            protected ISubstitutionContext _context;
            protected ICall _call;
            protected IResultSetter _resultSetter;
            protected IRouteFactory _routeFactory;
            protected IReceivedCalls _receivedCalls;

            public override void Context()
            {
                _context = mock<ISubstitutionContext>();
                _call = mock<ICall>();
                _receivedCalls = mock<IReceivedCalls>();
                _resultSetter = mock<IResultSetter>();
                _routeFactory = mock<IRouteFactory>();
            }

            public override CallRouter CreateSubjectUnderTest()
            {
                return new CallRouter(_context, _receivedCalls, _resultSetter, _routeFactory);
            }

            protected IRoute CreateRouteThatReturns(object returnValue)
            {
                var route = mock<IRoute>();
                route.stub(x => x.Handle(_call)).Return(returnValue);
                return route;
            }
        }

        public class When_a_route_is_set_and_a_member_is_called : Concern
        {
            readonly object _returnValueFromRoute = new object();
            readonly object _returnValueFromRecordReplayRoute = new object();
            object _result;
            IRoute _route;
            IRoute _recordReplayRoute;

            [Test]
            public void Should_update_last_call_router_on_substitution_context()
            {
                _context.received(x => x.LastCallRouter(sut));
            }

            [Test]
            public void Should_send_call_to_route_and_return_response()
            {
                Assert.That(_result, Is.SameAs(_returnValueFromRoute));
            }

            [Test]
            public void Should_send_next_call_to_record_replay_route_by_default()
            {
                var nextResult = sut.Route(_call);
                Assert.That(nextResult, Is.EqualTo(_returnValueFromRecordReplayRoute));
            }

            public override void Because()
            {
                sut.SetRoute<SampleRoute>();
                _result = sut.Route(_call);
            }

            public override void Context()
            {
                base.Context();
                _recordReplayRoute = CreateRouteThatReturns(_returnValueFromRecordReplayRoute);
                _route = CreateRouteThatReturns(_returnValueFromRoute);
                
                _routeFactory.stub(x => x.Create<SampleRoute>()).Return(_route);
                _routeFactory.stub(x => x.Create<RecordReplayRoute>()).Return(_recordReplayRoute);
            }

            abstract class SampleRoute : IRouteDefinition { public abstract IEnumerable<Type> HandlerTypes { get; } }
        }

        public class When_setting_result_of_last_call : Concern
        {
            IReturn _returnValue;
            MatchArgs _argMatching = MatchArgs.AsSpecifiedInCall;

            [Test]
            public void Should_set_result()
            {
                _resultSetter.received(x => x.SetResultForLastCall(_returnValue, _argMatching));
            }

            public override void Because()
            {
                sut.LastCallShouldReturn(_returnValue, _argMatching);
            }

            public override void Context()
            {
                base.Context();
                _returnValue = mock<IReturn>();
            }
        }

        public class When_clearing_received_calls : Concern
        {
            [Test]
            public void Should_clear_calls()
            {
                _receivedCalls.received(x => x.Clear());
            }
            public override void Because()
            {
                sut.ClearReceivedCalls();
            }
        }

        public class When_getting_received_calls : Concern
        {
            private IEnumerable<ICall> _result;
            private IEnumerable<ICall> _allCalls;

            [Test]
            public void Should_return_all_calls()
            {
                Assert.That(_result, Is.SameAs(_allCalls)); 
            }

            public override void Because()
            {
                _result = sut.ReceivedCalls();
            }

            public override void Context()
            {
                base.Context();
                _allCalls = new ICall[0];
                _receivedCalls.stub(x => x.AllCalls()).Return(_allCalls);
            }
        }
    }
}