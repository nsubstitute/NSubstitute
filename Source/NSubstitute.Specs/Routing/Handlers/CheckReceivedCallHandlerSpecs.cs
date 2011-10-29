using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class CheckReceivedCallHandlerSpecs
    {
        public abstract class Concern : ConcernFor<CheckReceivedCallsHandler>
        {
            protected int _valueToReturn;
            protected ISubstitutionContext _context;
            protected ICall _call;
            protected IReceivedCalls _receivedCalls;
            protected ICallSpecification _callSpecification;
            protected ICallSpecificationFactory _callSpecificationFactory;
            protected IReceivedCallsExceptionThrower _exceptionThrower;
            protected MatchArgs _argMatching = MatchArgs.AsSpecifiedInCall;
            protected Quantity _quantity = Quantity.AtLeastOne();

            public override void Context()
            {
                _valueToReturn = 7;
                _context = mock<ISubstitutionContext>();
                _receivedCalls = mock<IReceivedCalls>();
                _call = mock<ICall>();
                _callSpecification = mock<ICallSpecification>();
                _callSpecificationFactory = mock<ICallSpecificationFactory>();
                _exceptionThrower = mock<IReceivedCallsExceptionThrower>();
                _callSpecificationFactory.stub(x => x.CreateFrom(_call, _argMatching)).Return(_callSpecification);
            }

            public override CheckReceivedCallsHandler CreateSubjectUnderTest()
            {
                return new CheckReceivedCallsHandler(_receivedCalls, _callSpecificationFactory, _exceptionThrower, _argMatching, _quantity);
            } 
        }

        public class When_handling_call_that_has_been_received : Concern
        {
            private RouteAction _result;

            [Test]
            public void Should_return_without_exception()
            {
                _exceptionThrower.did_not_receive_with_any_args(x => x.Throw(null, null, null));
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
                _receivedCalls.stub(x => x.FindMatchingCalls(_callSpecification)).Return(new [] {_call});
            }
        }

        public class When_handling_call_that_has_not_been_received : Concern
        {
            ICallSpecification _callSpecWithAnyArguments;
            private ICall[] _actualCalls;

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrower.received(x => x.Throw(_callSpecification, _actualCalls, _quantity));
            }

            public override void Because()
            {
                sut.Handle(_call);
            }

            public override void Context()
            {
                base.Context();
                _callSpecWithAnyArguments = mock<ICallSpecification>();
                _callSpecificationFactory.stub(x => x.CreateFrom(_call, MatchArgs.Any)).Return(_callSpecWithAnyArguments);
                _actualCalls = new ICall[0];
                _receivedCalls.stub(x => x.FindMatchingCalls(_callSpecification)).Return(new ICall[0]);
                _receivedCalls.stub(x => x.FindMatchingCalls(_callSpecWithAnyArguments)).Return(_actualCalls);
            }
        }
    }
}