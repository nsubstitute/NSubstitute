using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routes.Handlers
{
    public class PropertySetterHandlerSpecs
    {
        public abstract class Concern : ConcernFor<PropertySetterHandler>
        {
            protected readonly object _setValue = new object();
            protected ICall _call;
            protected IPropertyHelper _propertyHelper;
            protected IResultSetter _resultSetter;
            protected ICall _propertyGetter;

            public override void Context()
            {
                _propertyHelper = mock<IPropertyHelper>();
                _resultSetter = mock<IResultSetter>();
                _call = mock<ICall>();
                _propertyGetter = mock<ICall>();
                _call.stub(x => x.GetArguments()).Return(new[] { _setValue });
                _propertyHelper.stub(x => x.CreateCallToPropertyGetterFromSetterCall(_call)).Return(_propertyGetter);
            }

            public override PropertySetterHandler CreateSubjectUnderTest()
            {
                return new PropertySetterHandler(_propertyHelper, _resultSetter);
            } 
        }

        public class When_call_is_a_property_setter : Concern
        {
            private ReturnValue _returnPassedToResultSetter;

            [Test]
            public void Should_add_set_value_to_configured_results()
            {
                Assert.That(_returnPassedToResultSetter.ReturnFor(null), Is.EqualTo(_setValue));
            }

            public override void Because()
            {
                sut.Handle(_call);
            }

            public override void Context()
            {
                base.Context();
                _propertyHelper.stub(x => x.IsCallToSetAReadWriteProperty(_call)).Return(true);
                _resultSetter
                    .stub(x => x.SetResultForCall(Arg.Is(_propertyGetter), Arg.Any<IReturn>(), Arg.Is(false)))
                    .IgnoreArguments()
                    .WhenCalled(x => _returnPassedToResultSetter = (ReturnValue) x.Arguments[1]);
            }
        }

        public class When_call_is_not_a_property_setter : Concern
        {
            [Test]
            public void Should_not_add_any_values_to_configured_results()
            {
                _resultSetter.did_not_receive(x => x.SetResultForCall(Arg.Is(_propertyGetter), Arg.Any<IReturn>(), Arg.Is(false)));
            }

            public override void Because()
            {
                sut.Handle(_call);
            }

            public override void Context()
            {
                base.Context();
                _propertyHelper.stub(x => x.IsCallToSetAReadWriteProperty(_call)).Return(false);
            }
        }
    }
}