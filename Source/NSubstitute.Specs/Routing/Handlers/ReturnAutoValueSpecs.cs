using System;
using NSubstitute.Core;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public abstract class ReturnAutoValueSpecs
    {
        public abstract class Concern : ConcernFor<ReturnAutoValue>
        {
            protected Type _type = typeof(int);
            protected IAutoValueProvider _firstAutoValueProvider;
            protected IAutoValueProvider _secondAutoValueProvider;
            protected RouteAction _result;
            protected ICall _call;
            protected ICallResults _callResults;
            protected ICallSpecification _callSpecification;
            protected ICallSpecificationFactory _callSpecificationFactory;

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                base.Context();
                _call = mock<ICall>();
                _call.stub(x => x.GetReturnType()).Return(_type);
                _callResults = mock<ICallResults>();
                _callSpecification = mock<ICallSpecification>();
                _callSpecificationFactory = mock<ICallSpecificationFactory>();
                _callSpecificationFactory.stub(x => x.CreateFrom(_call, MatchArgs.AsSpecifiedInCall)).Return(_callSpecification);
                _firstAutoValueProvider = mock<IAutoValueProvider>();
                _secondAutoValueProvider = mock<IAutoValueProvider>();
            }

            public ReturnAutoValue CreateReturnAutoValue(AutoValueBehaviour autoValueBehaviour)
            {
                return new ReturnAutoValue(autoValueBehaviour, new[] {_firstAutoValueProvider, _secondAutoValueProvider}, _callResults, _callSpecificationFactory);
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
                _callResults.received(x => x.SetResult(It.Is(_callSpecification), It.Matches<IReturn>(arg => arg.ReturnFor(null) == _autoValue)));
            }

            public override void Context()
            {
                base.Context();
                _secondAutoValueProvider.stub(x => x.CanProvideValueFor(_type)).Return(true);
                _secondAutoValueProvider.stub(x => x.GetValue(_type)).Return(_autoValue);
            }

            public override ReturnAutoValue CreateSubjectUnderTest()
            {
                return CreateReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls);
            }
        }

        public class When_no_auto_value_is_available_for_return_type_of_handled_call : Concern
        {
            [Test]
            public void Should_continue_route()
            {
                Assert.That(_result, Is.EqualTo(RouteAction.Continue()));
            }

            public override ReturnAutoValue CreateSubjectUnderTest()
            {
                return CreateReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls);
            }
        }

        public class When_has_an_auto_value_for_the_return_type_and_set_to_forget_values : Concern
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
                _callResults.did_not_receive_with_any_args(x => x.SetResult(null, null));
            }

            public override void Context()
            {
                base.Context();
                _secondAutoValueProvider.stub(x => x.CanProvideValueFor(_type)).Return(true);
                _secondAutoValueProvider.stub(x => x.GetValue(_type)).Return(_autoValue);
            }

            public override ReturnAutoValue CreateSubjectUnderTest()
            {
                return CreateReturnAutoValue(AutoValueBehaviour.ReturnAndForgetValue);
            }
        }
    }
}