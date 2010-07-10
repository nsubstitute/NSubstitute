using System;
using NSubstitute.Core;
using NSubstitute.Routes.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routes.Handlers
{
    public class CheckReceivedCallHandlerSpecs
    {
        public abstract class Concern : ConcernFor<CheckReceivedCallHandler>
        {
            protected int _valueToReturn;
            protected ISubstitutionContext _context;
            protected ICall _call;
            protected IReceivedCalls _receivedCalls;
            protected ICallSpecification _callSpecification;
            protected ICallSpecificationFactory _callSpecificationFactory;
            protected ICallNotReceivedExceptionThrower _exceptionThrower;
            MatchArgs _argMatching = MatchArgs.AsSpecifiedInCall;

            public override void Context()
            {
                _valueToReturn = 7;
                _context = mock<ISubstitutionContext>();
                _receivedCalls = mock<IReceivedCalls>();
                _call = mock<ICall>();
                _callSpecification = mock<ICallSpecification>();
                _callSpecificationFactory = mock<ICallSpecificationFactory>();
                _exceptionThrower = mock<ICallNotReceivedExceptionThrower>();
                _callSpecificationFactory.stub(x => x.CreateFrom(_call, _argMatching)).Return(_callSpecification);
            }

            public override CheckReceivedCallHandler CreateSubjectUnderTest()
            {
                return new CheckReceivedCallHandler(_receivedCalls, _callSpecificationFactory, _exceptionThrower, _argMatching);
            } 
        }

        public class When_handling_call_that_has_been_received : Concern
        {
            private object _result;

            [Test]
            public void Should_return_without_exception()
            {
                _exceptionThrower.did_not_receive_with_any_args(x => x.Throw(null, null));
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
                _exceptionThrower.received(x => x.Throw(_callSpecification, _actualCalls));
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