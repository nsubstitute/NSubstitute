using System;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ConfigureCallSpecs
    {
        public abstract class Concern : ConcernFor<ConfigureCall>
        {
            protected ICallActions _callActions;
            protected ICallResults _configuredResults;
            protected IGetCallSpec _getCallSpec;
            protected IReturn _compatibleReturnValue;

            public override void Context()
            {
                _configuredResults = mock<ICallResults>();
                _callActions = mock<ICallActions>();
                _getCallSpec = mock<IGetCallSpec>();

                _compatibleReturnValue = mock<IReturn>();
                _compatibleReturnValue.stub(x => x.CanBeAssignedTo(It.IsAny<Type>())).Return(true);
            }

            public override ConfigureCall CreateSubjectUnderTest()
            {
                return new ConfigureCall(_configuredResults, _callActions, _getCallSpec);
            }
        }

        public class When_setting_result_for_last_call : Concern
        {
            [Test]
            public void Configure_result_for_last_specified_call()
            {
                var lastCallSpec = mock<ICallSpecification>();
                _getCallSpec.stub(x => x.FromPendingSpecification(MatchArgs.AsSpecifiedInCall)).Return(lastCallSpec);

                sut.SetResultForLastCall(_compatibleReturnValue, MatchArgs.AsSpecifiedInCall);

                _configuredResults.received(x => x.SetResult(lastCallSpec, _compatibleReturnValue));
            }
        }

        public class When_setting_result_for_a_call : Concern
        {
            [Test]
            public void Configure_result_with_new_spec_for_that_call()
            {
                var call = mock<ICall>();
                var callSpec = mock<ICallSpecification>();
                _getCallSpec.stub(x => x.FromCall(call, MatchArgs.AsSpecifiedInCall)).Return(callSpec);

                sut.SetResultForCall(call, _compatibleReturnValue, MatchArgs.AsSpecifiedInCall);

                _configuredResults.received(x => x.SetResult(callSpec, _compatibleReturnValue));
            }
        }

        public class When_configuring_action_for_last_call : Concern
        {
            [Test]
            public void Configure_actions_for_last_specified_call()
            {
                Action<CallInfo> firstAction = x => { };
                Action<CallInfo> secondAction = x => { };
                var lastCallSpec = mock<ICallSpecification>();
                _getCallSpec.stub(x => x.FromPendingSpecification(MatchArgs.AsSpecifiedInCall)).Return(lastCallSpec);

                var config = sut.SetResultForLastCall(_compatibleReturnValue, MatchArgs.AsSpecifiedInCall);
                config
                    .AndDoes(firstAction)
                    .AndDoes(secondAction);

                _callActions.received(x => x.Add(lastCallSpec, firstAction));
                _callActions.received(x => x.Add(lastCallSpec, secondAction));
            }
        }

        public class When_setting_incompatible_result_for_call : Concern
        {
            [Test]
            public void Configure_result_for_last_specified_call()
            {
                var lastCallSpec = mock<ICallSpecification>();
                _getCallSpec.stub(x => x.FromPendingSpecification(MatchArgs.AsSpecifiedInCall)).Return(lastCallSpec);
                lastCallSpec.stub(x => x.GetMethodInfo()).Return(ReflectionHelper.GetMethod(() => SomeType.SampleMethod()));

                var incompatibleReturn = mock<IReturn>();
                incompatibleReturn.stub(x => x.CanBeAssignedTo(It.IsAny<Type>())).Return(false);
                incompatibleReturn.stub(x => x.TypeOrNull()).Return(typeof (SomeType));

                Assert.Throws<CouldNotSetReturnDueToTypeMismatchException>(
                        () => sut.SetResultForLastCall(incompatibleReturn, MatchArgs.AsSpecifiedInCall)
                    );
                _configuredResults.did_not_receive(x => x.SetResult(lastCallSpec, incompatibleReturn));
            }
        }

        private class SomeType { public static SomeType SampleMethod() { return null; } }
    }
}
