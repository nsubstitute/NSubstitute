using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class PassthroughArgumentParamsExtractorSpecs : ConcernFor<PassthroughArgumentParamsExtractor>
    {
        private IEnumerable<object> _argumentsWithAnArray;
        private IEnumerable<object> _emptyArguments;
        private IEnumerable<object> _argumentsWithoutAnArray;
        private IEnumerable<object> _argumentsWithObjectAndArray;
        private IEnumerable<int> _firstArgumentHighlighted;
        private IEnumerable<int> _noArgumentsHighlighted;
        private IEnumerable<int> _secondArgumentHighlighted;

        public override PassthroughArgumentParamsExtractor CreateSubjectUnderTest()
        {
            return new PassthroughArgumentParamsExtractor();
        }

        public override void Context()
        {
            _argumentsWithAnArray = new object[] { new[] { "foo", "bar" } };
            _argumentsWithObjectAndArray = new object[] { "blah", new[] { "foo", "bar" } };
            _argumentsWithoutAnArray = new object[] { "foobar" };
            _emptyArguments = new object[0];

            _noArgumentsHighlighted = new int[0];
            _firstArgumentHighlighted = new[] { 0 };
            _secondArgumentHighlighted = new[] { 1 };
        }

        [Test]
        public void Should_return_an_empty_list_for_empty_arguments()
        {
            IEnumerable<object> arguments = sut.GetWithExtractedArguments(_emptyArguments);

            Assert.That(arguments, Is.EquivalentTo(_emptyArguments));
        }

        [Test]
        public void Should_return_original_arguments_with_an_array()
        {
            IEnumerable<object> arguments = sut.GetWithExtractedArguments(_argumentsWithAnArray);

            Assert.That(arguments, Is.EquivalentTo(_argumentsWithAnArray));
        }

        [Test]
        public void Should_return_original_arguments_with_an_object_and_array()
        {
            IEnumerable<object> arguments = sut.GetWithExtractedArguments(_argumentsWithObjectAndArray);

            Assert.That(arguments, Is.EquivalentTo(_argumentsWithObjectAndArray));
        }

        [Test]
        public void Should_return_original_arguments_without_an_array()
        {
            IEnumerable<object> arguments = sut.GetWithExtractedArguments(_argumentsWithoutAnArray);

            Assert.That(arguments, Is.EquivalentTo(_argumentsWithoutAnArray));
        }

        [Test]
        public void Should_return_original_highlights_when_there_are_no_highlights()
        {
            IEnumerable<int> highlights = sut.GetWithExtractedArgumentsToHighlight(_argumentsWithAnArray, _noArgumentsHighlighted);

            Assert.That(highlights, Is.EquivalentTo(_noArgumentsHighlighted));
        }

        [Test]
        public void Should_return_original_highlights_from_array_when_array_is_highlighted()
        {
            IEnumerable<int> highlights = sut.GetWithExtractedArgumentsToHighlight(_argumentsWithAnArray, _firstArgumentHighlighted);

            Assert.That(highlights.Count(), Is.EqualTo(1));
            Assert.That(highlights.ToList()[0], Is.EqualTo(0));
        }


        [Test]
        public void Should_return_original_highlights_from_object_and_array_when_object_is_highlighted()
        {
            IEnumerable<int> highlights = sut.GetWithExtractedArgumentsToHighlight(_argumentsWithObjectAndArray, _firstArgumentHighlighted);

            Assert.That(highlights.Count(), Is.EqualTo(1));
            Assert.That(highlights.ToList()[0], Is.EqualTo(0));
        }

        [Test]
        public void Should_return_original_highlights_from_object_and_array_when_array_is_highlighted()
        {
            IEnumerable<int> highlights = sut.GetWithExtractedArgumentsToHighlight(_argumentsWithObjectAndArray, _secondArgumentHighlighted);

            Assert.That(highlights.Count(), Is.EqualTo(1));
            Assert.That(highlights.ToList()[0], Is.EqualTo(1));
        }
    }
}
