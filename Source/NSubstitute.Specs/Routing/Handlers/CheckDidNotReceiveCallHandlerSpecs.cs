using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class CheckDidNotReceiveCallHandlerSpecs
    {
        public abstract class Concern : ConcernFor<CheckDidNotReceiveCallHandler>
        {
            protected int _valueToReturn;
            protected ISubstitutionContext _context;
            protected ICall _call;
            protected IReceivedCalls _receivedCalls;
            protected ICallSpecification _callSpecification;
            protected ICallSpecificationFactory _callSpecificationFactory;
            protected ICallReceivedExceptionThrower _exceptionThrower;
            MatchArgs _argMatching = MatchArgs.AsSpecifiedInCall;

            public override void Context()
            {
                _valueToReturn = 7;
                _context = mock<ISubstitutionContext>();
                _receivedCalls = mock<IReceivedCalls>();
                _call = mock<ICall>();
                _callSpecification = mock<ICallSpecification>();
                _callSpecificationFactory = mock<ICallSpecificationFactory>();
                _exceptionThrower = mock<ICallReceivedExceptionThrower>();
                _callSpecificationFactory.stub(x => x.CreateFrom(_call, _argMatching)).Return(_callSpecification);
            }

            public override CheckDidNotReceiveCallHandler CreateSubjectUnderTest()
            {
                return new CheckDidNotReceiveCallHandler(_receivedCalls, _callSpecificationFactory, _exceptionThrower, _argMatching);
            }
        }

        public class When_handling_call_that_has_been_received : Concern
        {
            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrower.received(x => x.Throw(_callSpecification));
            }

            public override void Because()
            {
                sut.Handle(_call);
            }

            public override void Context()
            {
                base.Context();
                _receivedCalls.stub(x => x.FindMatchingCalls(_callSpecification)).Return(new[] { _call });
            }
        }

        public class When_handling_call_that_has_not_been_received : Concern
        {
            private RouteAction _result;

            [Test]
            public void Should_return_without_exception()
            {
                _exceptionThrower.did_not_receive(x => x.Throw(_callSpecification));
            }

            [Test]
            public void Should_continue_route()
            {
                Assert.That(_result, Is.SameAs(RouteAction.Continue()));
            }

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                base.Context();
                _receivedCalls.stub(x => x.FindMatchingCalls(_callSpecification)).Return(new ICall[0]);
            }
        }
    }
}