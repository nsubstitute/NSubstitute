using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class ArgumentSpecificationSpecs
    {
        public interface IFoo { }
        public class Foo : IFoo { }

        public class Concern : ConcernFor<ArgumentSpecification>
        {
            protected IArgumentMatcher _argumentMatcher;
            protected object _argument;

            public override void Context()
            {
                base.Context();
                _argumentMatcher = mock<IArgumentMatcher>();
            }

            public override ArgumentSpecification CreateSubjectUnderTest()
            {
                return new ArgumentSpecification(typeof(IFoo), _argumentMatcher);
            }

            public void Matches(object argument) { _argumentMatcher.stub(x => x.IsSatisfiedBy(_argument)).Return(true); }
            public void DoesNotMatch(object argument) { _argumentMatcher.stub(x => x.IsSatisfiedBy(_argument)).Return(false); }
        }

        public class When_argument_is_compatible_type : Concern
        {
            public override void Context()
            {
                base.Context();
                _argument = new Foo();
            }

            [Test]
            public void Spec_is_satisfied_when_matches()
            {
                Matches(_argument);

                Assert.That(sut.IsSatisfiedBy(_argument));
            }

            [Test]
            public void Spec_is_not_satisfied_when_not_matched()
            {
                DoesNotMatch(_argument);

                Assert.False(sut.IsSatisfiedBy(_argument));
            }
        }

        public class When_argument_is_incompatible_type : Concern
        {
            private bool _result;

            public override void Context()
            {
                base.Context();
                _argument = new object();
            }

            public override void Because()
            {
                base.Because();
                _result = sut.IsSatisfiedBy(_argument);
            }

            [Test]
            public void Spec_is_not_satisfied()
            {
                Assert.False(_result);
            }

            [Test]
            public void Spec_does_not_need_to_check_matcher()
            {
                _argumentMatcher.did_not_receive_with_any_args(x => x.IsSatisfiedBy(null));
            }
        }

        public class When_argument_is_reference_type : Concern
        {
            public ArgumentSpecification CreateSubjectForByRefType<T>(ref T value)
            {
                var parameterType = GetType().GetMethod("MethodWithARefArgument").GetParameters()[0].ParameterType;
                return new ArgumentSpecification(parameterType, _argumentMatcher);
            }

            public void MethodWithARefArgument<T>(ref T arg) { }
        }

        public class When_describing_why_an_argument_did_not_match
        {
            [Test]
            public void Returns_empty_when_matcher_does_not_support_describing_non_matches()
            {
                var matcher = new MatcherNotSupportingDescribe();
                var subject = new ArgumentSpecification(typeof(object), matcher);

                var result = subject.DescribeNonMatch(new object());

                Assert.That(result, Is.Empty);
            }

            [Test]
            public void Returns_the_desription_when_the_matcher_supports_describing_non_matches()
            {
                var matcher = new MatcherSupportingDescribe();
                var subject = new ArgumentSpecification(typeof(object), matcher);

                var result = subject.DescribeNonMatch(new object());

                Assert.That(result, Is.EqualTo(MatcherSupportingDescribe.Description));
            }

            [Test]
            public void Returns_arg_incompatible_without_calling_describable_matcher_when_specification_is_for_an_incompatible_type()
            {
                var matcher = new MatcherSupportingDescribe();
                var subject = new ArgumentSpecification(typeof(int), matcher);

                var result = subject.DescribeNonMatch("not an int");

                var expectedResult = string.Format("Expected an argument compatible with type {0}. Actual type was {1}.", typeof(int), typeof(string));
                Assert.That(result, Is.EqualTo(expectedResult));
            }

            [Test]
            public void Returns_empty_for_non_describably_matcher_when_specification_is_for_an_incompatible_type()
            {
                var matcher = new MatcherNotSupportingDescribe();
                var subject = new ArgumentSpecification(typeof(int), matcher);

                var result = subject.DescribeNonMatch("not an int");

                Assert.That(result, Is.Empty);
            }

            private class MatcherSupportingDescribe : IArgumentMatcher, IDescribeNonMatches
            {
                public const string Description = "description from matcher";
                public bool IsSatisfiedBy(object argument) { return false; }
                public string DescribeFor(object argument) { return Description; }
            }

            private class MatcherNotSupportingDescribe : IArgumentMatcher
            {
                public bool IsSatisfiedBy(object argument) { return false; }
            }

        }
    }
}