using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ResultSetterSpecs
    {
        public abstract class Concern : ConcernFor<ResultSetter>
        {
            protected ICallStack _callStack;
            protected IPendingSpecification _pendingSpecification;
            protected ICallResults _configuredResults;
            protected ICallSpecificationFactory _callSpecificationFactory;
            protected IReturn _returnValue;

            public override void Context()
            {
                _callStack = mock<ICallStack>();
                _pendingSpecification = mock<IPendingSpecification>();
                _configuredResults = mock<ICallResults>();
                _callSpecificationFactory = mock<ICallSpecificationFactory>();

                _returnValue = mock<IReturn>();
            }

            public override ResultSetter CreateSubjectUnderTest()
            {
                return new ResultSetter(_callStack, _pendingSpecification, _configuredResults, _callSpecificationFactory);
            }
        }

        public class When_a_call_specification_already_exists_for_the_last_call : Concern
        {
            ICallSpecification _lastCallSpecification;

            [Test]
            public void Should_configure_result_for_the_specification()
            {
                _configuredResults.received(x => x.SetResult(_lastCallSpecification, _returnValue));
            }

            [Test]
            public void Should_not_touch_call_stack()
            {
                _callStack.did_not_receive(x => x.Pop());
            }

            public override void Because()
            {
                sut.SetResultForLastCall(_returnValue, MatchArgs.AsSpecifiedInCall);
            }

            public override void Context()
            {
                base.Context();
                _lastCallSpecification = mock<ICallSpecification>();
                _pendingSpecification.stub(x => x.HasPendingCallSpec()).Return(true);
                _pendingSpecification.stub(x => x.UseCallSpec()).Return(_lastCallSpecification);
            }
        }

        public class When_a_call_specification_already_exists_for_the_last_call_and_we_are_setting_for_any_args : Concern
        {
            ICallSpecification _callSpecForAnyArgs;
            ICallSpecification _lastCallSpecification;

            [Test]
            public void Should_configure_result_for_the_specification()
            {
                _configuredResults.received(x => x.SetResult(_callSpecForAnyArgs, _returnValue));
            }

            [Test]
            public void Should_not_touch_call_stack()
            {
                _callStack.did_not_receive(x => x.Pop());
            }

            public override void Because()
            {
                sut.SetResultForLastCall(_returnValue, MatchArgs.Any);
            }

            public override void Context()
            {
                base.Context();
                _callSpecForAnyArgs = mock<ICallSpecification>();
                _lastCallSpecification = mock<ICallSpecification>();
                _pendingSpecification.stub(x => x.HasPendingCallSpec()).Return(true);
                _pendingSpecification.stub(x => x.UseCallSpec()).Return(_lastCallSpecification);
                _lastCallSpecification.stub(x => x.CreateCopyThatMatchesAnyArguments()).Return(_callSpecForAnyArgs);
            }
        }

        public class When_setting_return_value_for_last_call_and_there_is_no_existing_call_spec : Concern
        {
            private readonly MatchArgs _argMatchStrategy = MatchArgs.AsSpecifiedInCall;
            ICall _call;
            ICallSpecification _callSpecification;

            [Test]
            public void Should_remove_the_call_from_those_recorded_and_add_it_to_configured_results()
            {
                _configuredResults.received(x => x.SetResult(_callSpecification, _returnValue));
            }

            public override void Because()
            {
                sut.SetResultForLastCall(_returnValue, _argMatchStrategy);
            }

            public override void Context()
            {
                base.Context();
                _call = mock<ICall>();
                _callStack.stub(x => x.Pop()).Return(_call);
                _pendingSpecification.stub(x => x.HasPendingCallSpec()).Return(false);

                _callSpecification = mock<ICallSpecification>();
                _callSpecificationFactory.stub(x => x.CreateFrom(_call, _argMatchStrategy)).Return(_callSpecification);
            }
        }
    }
}