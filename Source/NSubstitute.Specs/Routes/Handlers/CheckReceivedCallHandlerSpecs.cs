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

            public override void Context()
            {
                _valueToReturn = 7;
                _context = mock<ISubstitutionContext>();
                _receivedCalls = mock<IReceivedCalls>();
                _call = mock<ICall>();
                _callSpecification = mock<ICallSpecification>();
                _callSpecificationFactory = mock<ICallSpecificationFactory>();
                _callSpecificationFactory.stub(x => x.CreateFrom(_call)).Return(_callSpecification);
            }

            public override CheckReceivedCallHandler CreateSubjectUnderTest()
            {
                return new CheckReceivedCallHandler(_receivedCalls, _callSpecificationFactory);
            } 
        }

        public class When_handling_call : Concern
        {
            [Test]
            public void Should_throw_exception_if_call_has_not_been_received()
            {
                _receivedCalls.received(x => x.ThrowIfCallNotFound(_callSpecification));
            }

            public override void Because()
            {
                sut.Handle(_call);
            }
        }
    }
}