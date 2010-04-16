using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class PropertySetterHandlerSpecs
    {
        public abstract class Concern : ConcernFor<PropertySetterHandler>
        {
            protected ICall _call;
            protected IReflectionHelper _reflectionHelper;
            protected IResultSetter _resultSetter;
            protected object _setValue;
            protected ICall _propertyGetter;

            public override void Context()
            {
                _reflectionHelper = mock<IReflectionHelper>();
                _resultSetter = mock<IResultSetter>();
                _call = mock<ICall>();
                _setValue = new object();
                _propertyGetter = mock<ICall>();
                _call.stub(x => x.GetArguments()).Return(new[] { _setValue });
                _reflectionHelper.stub(x => x.CreateCallToPropertyGetterFromSetterCall(_call)).Return(_propertyGetter);
            }

            public override PropertySetterHandler CreateSubjectUnderTest()
            {
                return new PropertySetterHandler(_reflectionHelper, _resultSetter);
            } 
        }

        public class When_call_is_a_property_setter : Concern
        {
            [Test]
            public void Should_add_set_value_to_configured_results()
            {
                _resultSetter.received(x => x.SetResultForCall(_propertyGetter, _setValue));
            }

            public override void Because()
            {
                sut.Handle(_call);
            }

            public override void Context()
            {
                base.Context();
                _reflectionHelper.stub(x => x.IsCallToSetAReadWriteProperty(_call)).Return(true);
            }
        }

        public class When_call_is_not_a_property_setter : Concern
        {
            [Test]
            public void Should_not_add_any_values_to_configured_results()
            {
                _resultSetter.did_not_receive(x => x.SetResultForCall(_propertyGetter, _setValue));
            }

            public override void Because()
            {
                sut.Handle(_call);
            }

            public override void Context()
            {
                base.Context();
                _reflectionHelper.stub(x => x.IsCallToSetAReadWriteProperty(_call)).Return(false);
            }
        }
    }
}