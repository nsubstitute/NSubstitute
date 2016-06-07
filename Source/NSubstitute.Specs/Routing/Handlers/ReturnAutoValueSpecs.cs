using System;
using System.Reflection;
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

            public ReturnAutoValue CreateReturnAutoValue(AutoValueBehaviour autoValueBehaviour)
            {
                return new ReturnAutoValue(autoValueBehaviour, new[] {_firstAutoValueProvider, _secondAutoValueProvider}, ConfigureCall);
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
                _call.stub(x => x.GetArguments()).Return(new object[0]);
                _call.stub(x => x.GetParameterInfos()).Return(new IParameterInfo[0]);
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

            public override void Context()
            {
                base.Context();
                _call.stub(x => x.GetArguments()).Return(new object[0]);
                _call.stub(x => x.GetParameterInfos()).Return(new IParameterInfo[0]);
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
                ConfigureCall.did_not_receive_with_any_args(x => x.SetResultForCall(null, null, null));
            }

            public override void Context()
            {
                base.Context();
                _call.stub(x => x.GetArguments()).Return(new object[0]);
                _call.stub(x => x.GetParameterInfos()).Return(new IParameterInfo[0]);
                _secondAutoValueProvider.stub(x => x.CanProvideValueFor(_type)).Return(true);
                _secondAutoValueProvider.stub(x => x.GetValue(_type)).Return(_autoValue);
            }

            public override ReturnAutoValue CreateSubjectUnderTest()
            {
                return CreateReturnAutoValue(AutoValueBehaviour.ReturnAndForgetValue);
            }
        }

        public class When_has_an_auto_value_for_an_out_param_of_the_handled_call : Concern
        {
            readonly object _autoValue = 32;
            private object[] _arguments = new object[1];

            [Test]
            public void Should_return_auto_value_for_parameter()
            {
                Assert.That(_arguments[0], Is.SameAs(_autoValue));
            }

            [Test]
            public void Should_set_auto_value_for_out_param_to_return_for_subsequent_calls()
            {
                object[] arguments = new object[1];
                var callInfo =
                    new CallInfo(new[] {new Argument(typeof(int).MakeByRefType(), () => arguments[0], x => arguments[0] = x)});
                ConfigureCall.received(x => x.SetResultForCall(It.Is(_call), It.Matches<IReturn>(arg => arg.ReturnFor(callInfo) == _autoValue), It.Is(MatchArgs.AsSpecifiedInCall)));
                Assert.That(arguments[0], Is.SameAs(_autoValue));
            }

            public override void Context()
            {
                base.Context();
                var parameterInfo = mock<IParameterInfo>();
                var byRefType = _type.MakeByRefType();
                parameterInfo.stub(x => x.ParameterType).Return(byRefType);
                _call.stub(x => x.GetParameterInfos()).Return(new[] {parameterInfo});
                _call.stub(x => x.GetArguments()).Return(_arguments);
                _secondAutoValueProvider.stub(x => x.CanProvideValueFor(_type)).Return(true);
                _secondAutoValueProvider.stub(x => x.GetValue(_type)).Return(_autoValue);
                _secondAutoValueProvider.stub(x => x.CanProvideValueFor(byRefType)).Return(true);
                _secondAutoValueProvider.stub(x => x.GetValue(byRefType)).Return(_autoValue);
            }

            public override ReturnAutoValue CreateSubjectUnderTest()
            {
                return CreateReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls);
            }
        }

        public class When_method_is_void_but_has_an_auto_value_for_an_out_param_of_the_handled_call : Concern
        {
            readonly object _autoValue = 32;
            private object[] _arguments = new object[1];

            [Test]
            public void Should_return_auto_value_for_parameter()
            {
                Assert.That(_arguments[0], Is.SameAs(_autoValue));
            }

            [Test]
            public void Should_set_auto_value_for_out_param_to_return_for_subsequent_calls()
            {
                object[] arguments = new object[1];
                var callInfo =
                    new CallInfo(new[] { new Argument(typeof(int).MakeByRefType(), () => arguments[0], x => arguments[0] = x) });
                ConfigureCall.received(x => x.SetResultForCall(It.Is(_call), It.Matches<IReturn>(arg => arg.ReturnFor(callInfo) == null), It.Is(MatchArgs.AsSpecifiedInCall)));
                Assert.That(arguments[0], Is.SameAs(_autoValue));
            }

            public override void Context()
            {
                _type = typeof(void);
                base.Context();
                var parameterInfo = mock<IParameterInfo>();
                var byRefType = typeof(int).MakeByRefType();
                parameterInfo.stub(x => x.ParameterType).Return(byRefType);
                _call.stub(x => x.GetParameterInfos()).Return(new[] { parameterInfo });
                _call.stub(x => x.GetArguments()).Return(_arguments);
                _secondAutoValueProvider.stub(x => x.CanProvideValueFor(_type)).Return(false);
                _secondAutoValueProvider.stub(x => x.CanProvideValueFor(byRefType)).Return(true);
                _secondAutoValueProvider.stub(x => x.GetValue(byRefType)).Return(_autoValue);
            }

            public override ReturnAutoValue CreateSubjectUnderTest()
            {
                return CreateReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls);
            }
        }

        public class When_method_is_void_but_has_no_out_param : Concern
        {
            [Test]
            public void Should_not_set_to_return_for_subsequent_calls()
            {
                ConfigureCall.did_not_receive_with_any_args(x => x.SetResultForCall(It.IsAny<ICall>(), It.IsAny<IReturn>(), It.IsAny<MatchArgs>()));
            }

            public override void Context()
            {
                _type = typeof(void);
                base.Context();
                _call.stub(x => x.GetParameterInfos()).Return(new IParameterInfo[0]);
                _call.stub(x => x.GetArguments()).Return(new object[0]);
                _secondAutoValueProvider.stub(x => x.CanProvideValueFor(_type)).Return(false);
            }

            public override ReturnAutoValue CreateSubjectUnderTest()
            {
                return CreateReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls);
            }
        }
    }
}