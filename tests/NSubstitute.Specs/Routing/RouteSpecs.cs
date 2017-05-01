using NSubstitute.Core;
using NSubstitute.Routing;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing
{
    public class RouteSpecs
    {
        public class When_handling_a_call : ConcernFor<Route>
        {
            private object _result;
            private ICall _call;
            private ICallHandler _firstHandler;
            private ICallHandler _secondHandler;
            private readonly object _valueToReturn = new object();
            private ICallHandler[] _handlers;

            [Test]
            public void Should_return_result_from_when_a_handler_provides_a_result()
            {
                Assert.That(_result, Is.SameAs(_valueToReturn)); 
            }

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                _call = mock<ICall>();
                _firstHandler = mock<ICallHandler>();
                _firstHandler.stub(x => x.Handle(_call)).Return(RouteAction.Continue());
                _secondHandler = mock<ICallHandler>();
                _secondHandler.stub(x => x.Handle(_call)).Return(RouteAction.Return(_valueToReturn));

                _handlers = new[] { _firstHandler, _secondHandler };
            }

            public override Route CreateSubjectUnderTest()
            {
                return new Route(_handlers);
            }
        }
    }
}