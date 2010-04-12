using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ResultSetterSpecs
    {
        public abstract class Concern : ConcernFor<ResultSetter>
        {
            protected ICall _call;
            protected ICallStack _callStack;
            protected ICallResults _configuredResults;
            protected ICallSpecification _callSpecification;
            protected ICallSpecificationFactory _callSpecificationFactory;

            public override void Context()
            {
                _callStack = mock<ICallStack>();
                _call = mock<ICall>();
                _configuredResults = mock<ICallResults>();
                _callSpecification = mock<ICallSpecification>();
                _callSpecificationFactory = mock<ICallSpecificationFactory>();
                _callSpecificationFactory.stub(x => x.CreateFrom(_call)).Return(_callSpecification);
            }

            public override ResultSetter CreateSubjectUnderTest()
            {
                return new ResultSetter(_callStack, _configuredResults, _callSpecificationFactory);
            }
        }
        
        public class When_the_return_value_for_the_last_call_is_set : Concern
        {
            const int ValueToReturn = 7;

            [Test]
            public void Should_remove_the_call_from_those_recorded_and_add_it_to_configured_results()
            {
                _configuredResults.received(x => x.SetResult(_callSpecification, ValueToReturn));
            }

            public override void Because()
            {
                sut.SetResultForLastCall(ValueToReturn);
            }

            public override void Context()
            {
                base.Context();
                _callStack.stub(x => x.Pop()).Return(_call);
            }
        }


    }
}