using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Routing.Handlers;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class RecordCallSpecificationHandlerSpecs
    {
        public class When_handling_call : ConcernFor<RecordCallSpecificationHandler>
        {
            IPendingSpecification _pendingSpec;
            ICallActions _callActions;
            ICallSpecificationFactory _callSpecFactory;
            ICall _call;
            ICallSpecification _callSpec;
            RouteAction _result;

            [Test]
            public void Should_set_pending_call_spec()
            {
                _pendingSpec.received(x => x.Set(_callSpec));
            }

            [Test]
            public void Should_add_any_specified_actions()
            {
                _callActions.received(x => x.Add(_callSpec));
            }

            [Test]
            public void Should_continue_route()
            {
                Assert.That(_result, Is.EqualTo(RouteAction.Continue()));
            }

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                _call = mock<ICall>();
                _callSpec = mock<ICallSpecification>();
                _callActions = mock<ICallActions>();
                _pendingSpec = mock<IPendingSpecification>();
                _callSpecFactory = mock<ICallSpecificationFactory>();
                _callSpecFactory.stub(x => x.CreateFrom(_call, MatchArgs.AsSpecifiedInCall)).Return(_callSpec);
            }

            public override RecordCallSpecificationHandler CreateSubjectUnderTest()
            {
                return new RecordCallSpecificationHandler(_pendingSpec, _callSpecFactory, _callActions);
            }
        }
    }
}