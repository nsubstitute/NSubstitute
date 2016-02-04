using System.Linq;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class ArrayContentsArgumentMatcherSpecs : ConcernFor<ArrayContentsArgumentMatcher>
    {
        private IArgumentSpecification[] _argumentSpecifications;
        private string[] _argument;

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

        [Test]
        public void Should_format_each_spec_and_argument_when_they_are_the_same_length()
        {
            _argumentSpecifications[0].stub(x => x.FormatArgument(_argument[0])).Return("first");
            _argumentSpecifications[1].stub(x => x.FormatArgument(_argument[1])).Return("second");
            var expected = "first, second";
            var result = sut.Format(_argument, true);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Should_handle_formatting_when_there_are_more_arguments_than_specs()
        {
            _argumentSpecifications[0].stub(x => x.FormatArgument(_argument[0])).Return("first");
            _argumentSpecifications[1].stub(x => x.FormatArgument(_argument[1])).Return("second");
            var argsWithExtra = _argument.Concat(new[] { "doh" }).ToArray();
            var expected = "first, second, " + DefaultFormat("doh", true);

            var result = sut.Format(argsWithExtra, true);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Should_handle_formatting_when_there_are_less_arguments_than_specs()
        {
            _argumentSpecifications[0].stub(x => x.FormatArgument(_argument[0])).Return("first");
            _argumentSpecifications[1].stub(x => x.FormatArgument(_argument[1])).Return("second");
            var lessArgsThanSpecs = new[] { _argument[0] };
            var expected = "first";

            var result = sut.Format(lessArgsThanSpecs, true);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Should_handle_formatting_valuetype_array_args()
        {
            var valueTypeArgs = new[] {1, 2};
            _argumentSpecifications[0].stub(x => x.FormatArgument(valueTypeArgs[0])).Return("1");
            _argumentSpecifications[1].stub(x => x.FormatArgument(valueTypeArgs[1])).Return("2");

            var result = sut.Format(valueTypeArgs, true);
            Assert.That(result, Is.EqualTo("1, 2"));
        }

        [Test]
        public void Should_highlight_empty_args_when_args_where_expected()
        {
            var emptyArgs = new int[0];
            var result = sut.Format(emptyArgs, true);
            Assert.That(result, Is.EqualTo("**"));
        }

        public override void Context()
        {
            _argument = new[] { "blah", "meh" };
            _argumentSpecifications = new[] { mock<IArgumentSpecification>(), mock<IArgumentSpecification>() };
            _argumentSpecifications[0].stub(x => x.IsSatisfiedBy(_argument[0])).Return(true);
            _argumentSpecifications[1].stub(x => x.IsSatisfiedBy(_argument[1])).Return(true);
        }

        private string DefaultFormat(string text, bool highlight)
        {
            return new ArgumentFormatter().Format(text, highlight);
        }

        public override ArrayContentsArgumentMatcher CreateSubjectUnderTest()
        {
            return new ArrayContentsArgumentMatcher(_argumentSpecifications);
        }
    }
}