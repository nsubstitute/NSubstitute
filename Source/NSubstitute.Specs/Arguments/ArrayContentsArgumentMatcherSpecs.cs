using System;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class ArrayContentsArgumentMatcherSpecs : ConcernFor<ArrayContentsArgumentMatcher>
    {
        private IArgumentSpecification[] _argumentSpecifications;
        private string[] _argument;
        private Type _forType;

        [Test]
        public void Should_match_when_all_argument_specs_match()
        {
            Assert.That(sut.IsSatisfiedBy(_argument), Is.True);
        }

        [Test]
        public void Should_not_match_when_not_all_argument_specs_match()
        {
            _argument[1] = "doh";
            Assert.That(sut.IsSatisfiedBy(_argument), Is.False);
        }

        [Test]
        public void Should_not_match_when_length_of_arrays_differ()
        {
            Assert.That(sut.IsSatisfiedBy(new[] { _argument[0] }), Is.False);
        }

        [Test]
        public void Should_not_match_when_argument_is_null()
        {
            Assert.That(sut.IsSatisfiedBy(null), Is.False);
        }

        [Test]
        public void Should_incorporate_toString_of_all_specifications_in_toString()
        {
            var expected = _argumentSpecifications[0].ToString() + ", " + _argumentSpecifications[1].ToString();

            Assert.That(sut.ToString(), Is.EqualTo(expected));
        }

        public override void Context()
        {
            _argument = new[] { "blah", "meh" };
            _forType = _argument.GetType();
            _argumentSpecifications = new[] { mock<IArgumentSpecification>(), mock<IArgumentSpecification>() };
            _argumentSpecifications[0].stub(x => x.IsSatisfiedBy(_argument[0])).Return(true);
           _argumentSpecifications[1].stub(x => x.IsSatisfiedBy(_argument[1])).Return(true);
        }

        public override ArrayContentsArgumentMatcher CreateSubjectUnderTest()
        {
            return new ArrayContentsArgumentMatcher(_argumentSpecifications);
        }
    }
}