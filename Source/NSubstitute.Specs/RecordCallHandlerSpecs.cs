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
            protected IResultSetter _resultSetter;

            public override void Context()
            {
                _valueToReturn = 7;
                _context = mock<ISubstitutionContext>();
                _callStack = mock<ICallStack>();
                _configuredResults = mock<ICallResults>();
                _reflectionHelper = mock<IReflectionHelper>();
                _resultSetter = mock<IResultSetter>();
                _call = mock<ICall>();
                _callSpecification = mock<ICallSpecification>();
            }

            public override RecordCallHandler CreateSubjectUnderTest()
            {
                return new RecordCallHandler(_callStack, _configuredResults);
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
    }
}