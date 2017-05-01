using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class PropertySetterHandlerSpecs
    {
        public abstract class Concern : ConcernFor<PropertySetterHandler>
        {
            protected readonly object _setValue = new object();
            protected ICall _call;
            protected IPropertyHelper _propertyHelper;
            protected IConfigureCall ConfigureCall;
            protected ICall _propertyGetter;

            public override void Context()
            {
                _propertyHelper = mock<IPropertyHelper>();
                ConfigureCall = mock<IConfigureCall>();
                _call = mock<ICall>();
                _propertyGetter = mock<ICall>();

                var settersWithMultipleArgsLikeIndexersHaveSetValueAsLastInArray = new[] { new object(), new object(), _setValue };
                _call.stub(x => x.GetArguments()).Return(settersWithMultipleArgsLikeIndexersHaveSetValueAsLastInArray);
                _propertyHelper.stub(x => x.CreateCallToPropertyGetterFromSetterCall(_call)).Return(_propertyGetter);
            }

            public override PropertySetterHandler CreateSubjectUnderTest()
            {
                return new PropertySetterHandler(_propertyHelper, ConfigureCall);
            }
        }

        public class When_call_is_a_property_setter : Concern
        {
            private ReturnValue _returnPassedToResultSetter;
            private RouteAction _result;

            [Test]
            public void Should_add_set_value_to_configured_results()
            {
                Assert.That(_returnPassedToResultSetter.ReturnFor(null), Is.EqualTo(_setValue));
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
                _propertyHelper.stub(x => x.IsCallToSetAReadWriteProperty(_call)).Return(true);
                ConfigureCall
                    .stub(x => x.SetResultForCall(It.Is(_propertyGetter), It.IsAny<IReturn>(), It.Is(MatchArgs.AsSpecifiedInCall)))
                    .WhenCalled(x => _returnPassedToResultSetter = (ReturnValue)x.Arguments[1]);
            }
        }

        public class When_call_is_not_a_property_setter : Concern
        {
            private RouteAction _result;

            [Test]
            public void Should_not_add_any_values_to_configured_results()
            {
                ConfigureCall.did_not_receive(x => x.SetResultForCall(It.Is(_propertyGetter), It.IsAny<IReturn>(), It.Is(MatchArgs.AsSpecifiedInCall)));
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
                _propertyHelper.stub(x => x.IsCallToSetAReadWriteProperty(_call)).Return(false);
            }
        }
    }
}