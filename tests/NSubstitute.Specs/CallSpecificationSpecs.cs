using System.Linq;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallSpecificationSpecs
    {
        public class Concern : ConcernFor<CallSpecification>
        {
            protected MethodInfo _methodInfoSpecified;
            protected MethodInfo _methodInfoOfCall;
            protected IArgumentSpecification _firstArgSpec;
            protected IArgumentSpecification _secondArgSpec;
            protected ICall _call;
            protected bool _result;
            protected string firstArg;
            protected int secondArg;

            public override void Context()
            {
                _firstArgSpec = mock<IArgumentSpecification>();
                _secondArgSpec = mock<IArgumentSpecification>();
                _methodInfoOfCall = ReflectionHelper.GetMethod(() => SampleMethod());
                _methodInfoSpecified = _methodInfoOfCall;
                firstArg = "something";
                secondArg = 123;
                _call = new Call(_methodInfoOfCall, new object[] { firstArg, secondArg }, null, new IArgumentSpecification[0].ToList());
            }

            public override void Because()
            {
                _result = sut.IsSatisfiedBy(_call);
            }

            public override CallSpecification CreateSubjectUnderTest()
            {
                return new CallSpecification(_methodInfoSpecified, new[] { _firstArgSpec, _secondArgSpec });
            }

            private void SampleMethod() { }
        }

        public class When_checking_a_call_with_matching_method_and_all_argument_specifications_met : Concern
        {
            public override void Context()
            {
                base.Context();
                _methodInfoSpecified = _methodInfoOfCall;
                _firstArgSpec.stub(x => x.IsSatisfiedBy(firstArg)).Return(true);
                _secondArgSpec.stub(x => x.IsSatisfiedBy(secondArg)).Return(true);
            }

            [Test]
            public void Should_be_satisfied()
            {
                Assert.That(_result);
            }
        }

        public class When_method_info_is_different : Concern
        {
            private void DifferentSampleMethod() { }

            public override void Context()
            {
                base.Context();
                _methodInfoSpecified = ReflectionHelper.GetMethod(() => DifferentSampleMethod());
                _firstArgSpec.stub(x => x.IsSatisfiedBy(firstArg)).Return(true);
                _secondArgSpec.stub(x => x.IsSatisfiedBy(secondArg)).Return(true);
            }

            [Test]
            public void Should_not_be_satisfied()
            {
                Assert.That(_result, Is.False);
            }

            [Test]
            public void Non_matching_args_should_be_empty()
            {
                Assert.That(sut.NonMatchingArguments(_call).ToArray(), Is.Empty);
            }
        }

        public class When_an_argument_spec_is_not_satisfied : Concern
        {
            public override void Context()
            {
                base.Context();
                _firstArgSpec.stub(x => x.IsSatisfiedBy(firstArg)).Return(true);
                _secondArgSpec.stub(x => x.IsSatisfiedBy(secondArg)).Return(false);
            }

            [Test]
            public void Should_not_be_satisfied()
            {
                Assert.That(_result, Is.False);
            }

            [Test]
            public void Should_return_non_matching_arguments()
            {
                Assert.That(sut.NonMatchingArguments(_call).ToArray(), Is.EqualTo(new[] { new ArgumentMatchInfo(1, secondArg, _secondArgSpec) }));
            }
        }

        public class When_call_has_less_arguments_than_specified : Concern
        {
            public override void Context()
            {
                base.Context();
                _firstArgSpec.stub(x => x.IsSatisfiedBy(firstArg)).Return(true);
                _secondArgSpec.stub(x => x.IsSatisfiedBy(secondArg)).Return(true);
                _call = new Call(_methodInfoSpecified, new object[] { firstArg }, null, new IArgumentSpecification[0].ToList());
            }

            [Test]
            public void Should_not_be_satisfied()
            {
                Assert.That(_result, Is.False);
            }

            [Test]
            public void Should_return_empty_for_non_matching_arg_because_args_that_are_specified_match()
            {
                Assert.That(sut.NonMatchingArguments(_call).ToArray(), Is.Empty);
            }
        }

        public class When_formatting_call_as_string : Concern
        {
            private IMethodInfoFormatter _callFormatter;
            private string _specAsString;
            private const string FormattedCall = "Call(first, second)";

            public override void Context()
            {
                base.Context();
                _firstArgSpec.stub(x => x.FormatArgument(firstArg)).Return("first");
                _secondArgSpec.stub(x => x.FormatArgument(secondArg)).Return("second");
                _callFormatter = mock<IMethodInfoFormatter>();
                _callFormatter
                    .stub(x => x.Format(_methodInfoSpecified, new[] { "first", "second" }))
                    .Return(FormattedCall);

            }

            public override void AfterContextEstablished()
            {
                base.AfterContextEstablished();
                sut.CallFormatter = _callFormatter;
            }

            public override void Because()
            {
                _specAsString = sut.Format(_call);
            }

            [Test]
            public void Should_return_formatted_call()
            {
                Assert.That(_specAsString, Is.EqualTo(FormattedCall));
            }
        }

        public class When_converting_call_spec_to_string : Concern
        {
            private IMethodInfoFormatter _callFormatter;
            private string _specAsString;
            private const string FormattedCallSpec = "CallSpec(first, second)";

            public override void Context()
            {
                base.Context();
                _callFormatter = mock<IMethodInfoFormatter>();
                _callFormatter
                    .stub(x => x.Format(_methodInfoSpecified, new[] { _firstArgSpec.ToString(), _secondArgSpec.ToString() }))
                    .Return(FormattedCallSpec);
            }

            public override void AfterContextEstablished()
            {
                base.AfterContextEstablished();
                sut.CallFormatter = _callFormatter;
            }

            public override void Because()
            {
                _specAsString = sut.ToString();
            }

            [Test]
            public void Should_return_formatted_method_and_argument_specifications()
            {
                Assert.That(_specAsString, Is.EqualTo(FormattedCallSpec));
            }
        }
    }
}
