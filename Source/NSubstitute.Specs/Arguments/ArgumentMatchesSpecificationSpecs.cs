using System;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class ArgumentMatchesSpecificationSpecs 
    {
        [Test]
        public void Should_use_expression_as_string_description()
        {
            var spec = new ArgumentMatchesSpecification<int>(x => x < 5);
            Assert.That(spec.ToString(), Is.StringContaining("x < 5"));
        }

        [Test]
        public void Should_match_when_predicate_is_satisfied()
        {
            var spec = new ArgumentMatchesSpecification<int>(x => x < 5);
            Assert.True(spec.IsSatisfiedBy(2));
        }

        [Test]
        public void Should_not_match_when_predicate_is_not_satisfied()
        {
            var spec = new ArgumentMatchesSpecification<int>(x => x < 5);
            Assert.False(spec.IsSatisfiedBy(7));
        }

        [Test]
        public void Should_throw_argument_matching_exception_if_arg_spec_throws()
        {
            var spec = new ArgumentMatchesSpecification<string>(x => Throw());
            Assert.That(() => spec.IsSatisfiedBy("hello world"), Throws.InstanceOf<ArgumentMatchingException>());
        }

        private bool Throw() { throw new InvalidOperationException(); }

        [Test]
        public void Should_throw_argument_matching_exception_that_mentions_null_arg_if_arg_spec_throws_due_to_null_reference()
        {
            var spec = new ArgumentMatchesSpecification<string>(x => x.Contains("something"));
            Assert.That(() => spec.IsSatisfiedBy(null), Throws.InstanceOf<ArgumentMatchingException>().With.Message.ContainsSubstring("<null>"));
        }
    }
}