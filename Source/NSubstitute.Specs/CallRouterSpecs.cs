using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Routing;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallRouterSpecs
    {
        public abstract class Concern : ConcernFor<CallRouter>
        {
            protected readonly object _returnValueFromRecordReplayRoute = "value from record replay route";
            protected ISubstitutionContext _context;
            protected ICall _call;
            protected IConfigureCall ConfigureCall;
            protected IRouteFactory _routeFactory;
            protected IReceivedCalls _receivedCalls;
            protected ISubstituteState _state;
            protected IResultsForType _resultsForType;

            public override void Context()
            {
                _context = mock<ISubstitutionContext>();
                _call = mock<ICall>();
                _state = mock<ISubstituteState>();
                _receivedCalls = mock<IReceivedCalls>();
                ConfigureCall = mock<IConfigureCall>();
                _routeFactory = mock<IRouteFactory>();
                _resultsForType = mock<IResultsForType>();
                _state.stub(x => x.ReceivedCalls).Return(_receivedCalls);
                _state.stub(x => x.ConfigureCall).Return(ConfigureCall);
                _state.stub(x => x.ResultsForType)
                      .Return(_resultsForType);
                var recordReplayRoute = CreateRouteThatReturns(_returnValueFromRecordReplayRoute);
                recordReplayRoute.stub(x => x.IsRecordReplayRoute).Return(true);
                _routeFactory.stub(x => x.RecordReplay(_state)).Return(recordReplayRoute);
            }

            public override CallRouter CreateSubjectUnderTest()
            {
                return new CallRouter(_state, _context, _routeFactory);
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
            object _result;
            IRoute _route;

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
                sut.SetRoute(x => _route);
                _result = sut.Route(_call);
            }

            public override void Context()
            {
                base.Context();
                _route = CreateRouteThatReturns(_returnValueFromRoute);
            }
        }

        public class When_using_default_route : Concern
        {
            readonly object _returnValueFromRecordCallSpecRoute = "value from call spec route";
            IRoute _recordCallSpecRoute;

            [Test]
            public void Should_record_call_spec_if_argument_matchers_have_been_specified_and_the_call_takes_arguments()
            {
                _call.stub(x => x.GetArguments()).Return(new object[4]);
                _call.stub(x => x.GetArgumentSpecifications()).Return(CreateArgSpecs(3));

                var result = sut.Route(_call);

                Assert.That(result, Is.SameAs(_returnValueFromRecordCallSpecRoute));
            }

            [Test]
            public void Should_record_call_and_replay_configured_result_if_this_looks_like_a_regular_call()
            {
                _call.stub(x => x.GetArguments()).Return(new object[4]);
                _call.stub(x => x.GetArgumentSpecifications()).Return(CreateArgSpecs(0));

                var result = sut.Route(_call);

                Assert.That(result, Is.SameAs(_returnValueFromRecordReplayRoute));
            }

            [Test]
            public void Should_record_call_and_replay_configured_result_if_there_are_arg_matchers_but_the_call_does_not_take_args()
            {
                _call.stub(x => x.GetArguments()).Return(new object[0]);
                _call.stub(x => x.GetArgumentSpecifications()).Return(CreateArgSpecs(2));

                var result = sut.Route(_call);

                Assert.That(result, Is.SameAs(_returnValueFromRecordReplayRoute));
            }

            public override void Context()
            {
                base.Context();
                _recordCallSpecRoute = CreateRouteThatReturns(_returnValueFromRecordCallSpecRoute);
                
                _routeFactory.stub(x => x.RecordCallSpecification(_state)).Return(_recordCallSpecRoute);
            }

            IList<IArgumentSpecification> CreateArgSpecs(int count)
            {
                return Enumerable.Range(1, count).Select(x => mock<IArgumentSpecification>()).ToList();
            }
        }

        public class When_setting_result_of_last_call : Concern
        {
            readonly MatchArgs _argMatching = MatchArgs.AsSpecifiedInCall;
            IReturn _returnValue;

            [Test]
            public void Should_set_result()
            {
                ConfigureCall.received(x => x.SetResultForLastCall(_returnValue, _argMatching));
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

        public class When_setting_result_for_type : Concern
        {
            private readonly Type _type = typeof(IFoo);
            IReturn _returnValue;

            [Test]
            public void Should_set_result()
            {
                _resultsForType.received(x => x.SetResult(_type, _returnValue));
            }

            public override void Because()
            {
                sut.SetReturnForType(_type, _returnValue);
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

        public class When_handling_a_call_while_querying : Concern
        {
            private readonly object _resultFromQueryRoute = new object();
            private object _result;

            public override void Context()
            {
                base.Context();
                _context.stub(x => x.IsQuerying).Return(true);
                _routeFactory.stub(x => x.CallQuery(_state)).Return(CreateRouteThatReturns(_resultFromQueryRoute));
            }

            public override void Because()
            {
                _result = sut.Route(_call);
            }

            [Test]
            public void Should_handle_via_query_route()
            {
                Assert.That(_result, Is.EqualTo(_resultFromQueryRoute));
            }
        }
    }
}