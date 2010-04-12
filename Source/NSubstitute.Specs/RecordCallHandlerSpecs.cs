using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class RecordCallHandlerSpecs
    {
        public abstract class Concern : ConcernFor<RecordCallHandler>
        {
            protected int _valueToReturn;
            protected ISubstitutionContext _context;
            protected ICall _call;
            protected ICallStack _callStack;
            protected ICallResults _configuredResults;
            protected IReflectionHelper _reflectionHelper;
            protected ICallSpecification _callSpecification;
            protected ICallSpecificationFactory _callSpecificationFactory;

            public override void Context()
            {
                _valueToReturn = 7;
                _context = mock<ISubstitutionContext>();
                _callStack = mock<ICallStack>();
                _configuredResults = mock<ICallResults>();
                _reflectionHelper = mock<IReflectionHelper>();
                _call = mock<ICall>();
                _callSpecification = mock<ICallSpecification>();
                _callSpecificationFactory = mock<ICallSpecificationFactory>();
                _callSpecificationFactory.stub(x => x.CreateFrom(_call)).Return(_callSpecification);
            }

            public override RecordCallHandler CreateSubjectUnderTest()
            {
                return new RecordCallHandler(_callStack, _configuredResults, _reflectionHelper, _callSpecificationFactory);
            } 
        }

        public class When_handling_call_to_a_member : Concern
        {
            object _result;

            [Test]
            public void Should_record_call()
            {
                _callStack.received(x => x.Push(_call));
            }

            [Test]
            public void Should_return_value_from_configured_results()
            {
                Assert.That(_result, Is.EqualTo(_valueToReturn));
            }

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                base.Context();
                _configuredResults.stub(x => x.GetResult(_call)).Return(_valueToReturn);
            }
        }

        public class When_call_is_a_property_setter : Concern
        {
            private object _setValue;
            private ICall _propertyGetter;
            private ICallSpecification _propertyGetterSpecification;

            [Test]
            public void Should_add_set_value_to_configured_results()
            {
                _configuredResults.received(x => x.SetResult(_propertyGetterSpecification, _setValue));
            }

            public override void Because()
            {
                sut.Handle(_call);
            }

            public override void Context()
            {
                base.Context();
                _setValue = new object();
                _propertyGetter = mock<ICall>();
                _propertyGetterSpecification = mock<ICallSpecification>();
                _call.stub(x => x.GetArguments()).Return(new[] { _setValue });
                _reflectionHelper.stub(x => x.IsCallToSetAReadWriteProperty(_call)).Return(true);
                _reflectionHelper.stub(x => x.CreateCallToPropertyGetterFromSetterCall(_call)).Return(_propertyGetter);
                _callSpecificationFactory.stub(x => x.CreateFrom(_propertyGetter)).Return(_propertyGetterSpecification);
            }
        }

    }
}