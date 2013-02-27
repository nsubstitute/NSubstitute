using System;
using NSubstitute.Core;
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
            protected IReturn _returnValue;

            public override void Context()
            {
                _configuredResults = mock<ICallResults>();
                _callActions = mock<ICallActions>();
                _getCallSpec = mock<IGetCallSpec>();

                _returnValue = mock<IReturn>();
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
                _getCallSpec.stub(x => x.FromLastCall(MatchArgs.AsSpecifiedInCall)).Return(lastCallSpec);

                sut.SetResultForLastCall(_returnValue, MatchArgs.AsSpecifiedInCall);

                _configuredResults.received(x => x.SetResult(lastCallSpec, _returnValue));
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

                sut.SetResultForCall(call, _returnValue, MatchArgs.AsSpecifiedInCall);

                _configuredResults.received(x => x.SetResult(callSpec, _returnValue));
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
                _getCallSpec.stub(x => x.FromLastCall(MatchArgs.AsSpecifiedInCall)).Return(lastCallSpec);

                var config = sut.SetResultForLastCall(_returnValue, MatchArgs.AsSpecifiedInCall);
                config
                    .AndDoes(firstAction)
                    .AndDoes(secondAction);

                _callActions.received(x => x.Add(lastCallSpec, firstAction));
                _callActions.received(x => x.Add(lastCallSpec, secondAction));
            }
        }
    }
}
