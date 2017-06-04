using System;
using System.Collections.Generic;
using System.Linq;
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
            protected const int ValueToReturn = 7;
            protected ISubstitutionContext _context;
            protected ICall _call;
            protected IReceivedCalls _receivedCalls;
            protected ICallSpecification _callSpecFromCall;
            protected ICallSpecification _callSpecFromMethodSpec;
            protected ICallSpecificationFactory _callSpecificationFactory;
            protected FakeExceptionThrower _exceptionThrower;
            protected MatchArgs _argMatching = MatchArgs.AsSpecifiedInCall;
            protected Quantity _quantity;

            public override void Context()
            {
                _context = mock<ISubstitutionContext>();
                _receivedCalls = mock<IReceivedCalls>();
                _call = mock<ICall>();
                _callSpecFromCall = mock<ICallSpecification>();
                _callSpecFromMethodSpec = mock<ICallSpecification>();
                _callSpecificationFactory = mock<ICallSpecificationFactory>();

                _exceptionThrower = new FakeExceptionThrower();

                _callSpecFromCall.stub(x => x.IsSatisfiedBy(_call)).Return(true);
                _callSpecFromMethodSpec.stub(x=>x.IsSatisfiedBy(_call)).Return(false);
				
				_callSpecificationFactory.stub(x => x.CreateFrom(_call, _argMatching)).Return(_callSpecFromCall);
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
                _exceptionThrower.ShouldNotHaveBeenToldToThrow();
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
                _quantity = Quantity.AtLeastOne();
                _receivedCalls.stub(x => x.AllCalls()).Return(new[] { _call });
                _callSpecificationFactory.stub(x => x.CreateFrom(_call, MatchArgs.Any)).Return(_callSpecFromMethodSpec);
            }
        }

        public class When_handling_call_that_has_not_been_received_in_the_correct_quantity : Concern
        {
            ICallSpecification _callSpecWithAnyArguments;
            private ICall[] _matchingCalls;
            private ICall[] _relatedNonMatchingCalls;

            [Test]
            public void Should_throw_exception_with_actual_calls_and_related_calls()
            {
                _exceptionThrower.ShouldHaveBeenToldToThrowWith(_callSpecFromCall, _matchingCalls, _relatedNonMatchingCalls, _quantity);
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

                _quantity = Quantity.Exactly(2);
                _matchingCalls = new[] { _call };
                _relatedNonMatchingCalls = new[] { mock<ICall>() };
                var completelyNonMatchingCalls = new[] { mock<ICall>() };

                Array.ForEach(_relatedNonMatchingCalls, call => _callSpecWithAnyArguments.stub(x => x.IsSatisfiedBy(call)).Return(true));

                var allRelatedCalls = _matchingCalls.Concat(_relatedNonMatchingCalls).Concat(completelyNonMatchingCalls);
                _receivedCalls.stub(x => x.AllCalls()).Return(allRelatedCalls);
            }
        }

        public class FakeExceptionThrower : IReceivedCallsExceptionThrower
        {
            private ICallSpecification _callSpecification;
            private IEnumerable<ICall> _matchingCalls;
            private IEnumerable<ICall> _nonMatchingCalls;
            private Quantity _requiredQuantity;
            private bool _wasCalled;

            public void Throw(ICallSpecification callSpecification, IEnumerable<ICall> matchingCalls, IEnumerable<ICall> nonMatchingCalls, Quantity requiredQuantity)
            {
                _wasCalled = true;
                _callSpecification = callSpecification;
                _matchingCalls = matchingCalls;
                _nonMatchingCalls = nonMatchingCalls;
                _requiredQuantity = requiredQuantity;
            }

            public void ShouldHaveBeenToldToThrowWith(ICallSpecification callSpecification, IEnumerable<ICall> matchingCalls, IEnumerable<ICall> nonMatchingCalls, Quantity requiredQuantity)
            {
                Assert.That(_callSpecification, Is.EqualTo(callSpecification));
                Assert.That(_matchingCalls.ToArray(), Is.EquivalentTo(matchingCalls.ToArray()));
                Assert.That(_nonMatchingCalls.ToArray(), Is.EquivalentTo(nonMatchingCalls.ToArray()));
                Assert.That(_requiredQuantity, Is.EqualTo(requiredQuantity));
            }

            public void ShouldNotHaveBeenToldToThrow()
            {
                Assert.False(_wasCalled);
            }
        }
    }
}