using System;
using NSubstitute.Core.Arguments;
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
        public void Should_not_match_if_arg_spec_throws()
        {
            var spec = new ArgumentMatchesSpecification<string>(x => Throw());
            Assert.False(spec.IsSatisfiedBy("hello world"));
        }

        [Test]
        public void Should_match_when_predicate_compares_null_to_null()
        {
            var spec = new ArgumentMatchesSpecification<string>(x => x == null);
            Assert.True(spec.IsSatisfiedBy(null));
        }

        private bool Throw() { throw new InvalidOperationException(); }
    }
}