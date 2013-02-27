using System;
using NSubstitute.Core;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public abstract class ReturnAutoValueForThisAndSubsequentCallsHandlerSpecs
    {
        public class Concern : ConcernFor<ReturnAutoValueForThisAndSubsequentCallsHandler>
        {
            protected Type _type = typeof(int);
            private IAutoValueProvider _firstAutoValueProvider;
            protected IAutoValueProvider _secondAutoValueProvider;
            protected RouteAction _result;
            protected ICall _call;
            protected IConfigureCall ConfigureCall;

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                base.Context();
                ConfigureCall = mock<IConfigureCall>();
                _call = mock<ICall>();
                _call.stub(x => x.GetReturnType()).Return(_type);
                _firstAutoValueProvider = mock<IAutoValueProvider>();
                _secondAutoValueProvider = mock<IAutoValueProvider>();
            }

            public override ReturnAutoValueForThisAndSubsequentCallsHandler CreateSubjectUnderTest()
            {
                return new ReturnAutoValueForThisAndSubsequentCallsHandler(new[] {_firstAutoValueProvider, _secondAutoValueProvider}, ConfigureCall);
            }
        }

        public class When_has_an_auto_value_for_the_return_type_of_the_handled_call : Concern
        {
            readonly object _autoValue = new object();

            [Test]
            public void Should_return_auto_value()
            {
                Assert.That(_result.ReturnValue, Is.SameAs(_autoValue)); 
            }

            [Test]
            public void Should_set_auto_value_as_value_to_return_for_subsequent_calls()
            {
                ConfigureCall.received(x => x.SetResultForCall(It.Is(_call), It.Matches<IReturn>(arg => arg.ReturnFor(null) == _autoValue), It.Is(MatchArgs.AsSpecifiedInCall)));
            }

            public override void Context()
            {
                base.Context();
                _secondAutoValueProvider.stub(x => x.CanProvideValueFor(_type)).Return(true);
                _secondAutoValueProvider.stub(x => x.GetValue(_type)).Return(_autoValue);
            }
        }

        public class When_no_auto_value_is_available_for_return_type_of_handled_call : Concern
        {
            [Test]
            public void Should_continue_route()
            {
                Assert.That(_result, Is.EqualTo(RouteAction.Continue()));
            }
        }
    }
}