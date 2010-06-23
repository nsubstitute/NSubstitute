using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallSpecificationSpecs
    {
        public class Concern : ConcernFor<CallSpecification>
        {
            protected MethodInfo _methodInfo;
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
                _methodInfo = typeof(IAmForTesting).GetMethod("TestMethod");
                firstArg = "something";
                secondArg = 123;
                _call = new Call(_methodInfo, new object[] { firstArg, secondArg }, null);
            }

            public override void Because()
            {
                _result = sut.IsSatisfiedBy(_call);
            }

            public override CallSpecification CreateSubjectUnderTest()
            {
                return new CallSpecification(_methodInfo, new[] { _firstArgSpec, _secondArgSpec });
            }
        }

        public interface IAmForTesting
        {
            int TestMethod(string aString, int aNumber);
            int AnotherTestMethod(string aString, int aNumber);
        }

        public class When_checking_a_call_with_matching_method_and_all_argument_specifications_met : Concern
        {
            public override void Context()
            {
                base.Context();
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
            public override void Context()
            {
                base.Context();
                _methodInfo = typeof(IAmForTesting).GetMethod("AnotherTestMethod");
                _firstArgSpec.stub(x => x.IsSatisfiedBy(firstArg)).Return(true);
                _secondArgSpec.stub(x => x.IsSatisfiedBy(secondArg)).Return(true);
            }

            [Test]
            public void Should_not_be_satisfied()
            {
                Assert.That(_result, Is.False);
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
        }

        public class When_call_has_different_number_of_arguments_than_specified : Concern
        {
            public override void Context()
            {
                base.Context();
                _firstArgSpec.stub(x => x.IsSatisfiedBy(firstArg)).Return(true);
                _secondArgSpec.stub(x => x.IsSatisfiedBy(secondArg)).Return(true);
                _call = new Call(_methodInfo, new object[] { firstArg }, null);
            }

            [Test]
            public void Should_not_be_satisfied()
            {
                Assert.That(_result, Is.False);
            }
        }
        
        public class When_formatting_call_as_string : Concern
        {
            private ICallFormatter _callFormatter;
            private string _specAsString;
            private const string FormattedCall = "Call(first, second)";

            public override void Context()
            {
                base.Context();
                _callFormatter = mock<ICallFormatter>();
                _callFormatter.stub(x => x.Format(_methodInfo, new [] { _firstArgSpec, _secondArgSpec })).Return(FormattedCall);
            }

            public override void Because()
            {
                _specAsString = sut.ToString(_callFormatter);
            }

            [Test]
            public void Should_return_formatted_call()
            {
                Assert.That(_specAsString, Is.EqualTo(FormattedCall));
            }
        }
    }
}