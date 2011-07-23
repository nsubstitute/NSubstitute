using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
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

        public class When_using_default_route : Concern
        {
            readonly object _returnValueFromRecordCallSpecRoute = "value from call spec route";
            readonly object _returnValueFromRecordReplayRoute = "value from record replay route";
            IRoute _recordReplayRoute;
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
                _recordReplayRoute = CreateRouteThatReturns(_returnValueFromRecordReplayRoute);
                _recordCallSpecRoute = CreateRouteThatReturns(_returnValueFromRecordCallSpecRoute);
                
                _routeFactory.stub(x => x.Create<RecordReplayRoute>()).Return(_recordReplayRoute);
                _routeFactory.stub(x => x.Create<RecordCallSpecificationRoute>()).Return(_recordCallSpecRoute);
            }

            IList<IArgumentSpecification> CreateArgSpecs(int count)
            {
                return Enumerable.Range(1, count).Select(x => mock<IArgumentSpecification>()).ToList();
            }
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