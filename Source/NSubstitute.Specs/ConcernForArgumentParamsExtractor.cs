using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public abstract class ConcernForArgumentParamsExtractor : ConcernFor<ArgumentParamsExtractor>
    {
        protected IEnumerable<object> _argumentsWithAnArray;
        protected IEnumerable<object> _emptyArguments;
        protected IEnumerable<object> _argumentsWithoutAnArray;
        protected IEnumerable<object> _argumentsWithObjectAndArray;
        protected IEnumerable<int> _firstArgumentHighlighted;
        protected IEnumerable<int> _noArgumentsHighlighted;
        protected IEnumerable<int> _secondArgumentHighlighted;

        [Test]
        public void Should_throw_an_exception_on_empty_arguments()
        {
            Assert.Throws<ArgumentException>(() => sut.GetWithExtractedArguments(_emptyArguments));
        }

        [Test]
        public void Should_throw_an_exception_on_missing_argument_array()
        {
            Assert.Throws<ArgumentException>(() => sut.GetWithExtractedArguments(_argumentsWithoutAnArray));
        }

        [Test]
        public void Should_extract_arguments_from_array_contents()
        {
            IEnumerable<object> arguments = sut.GetWithExtractedArguments(_argumentsWithAnArray);

            Assert.That(arguments.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Should_extract_arguments_from_object_and_array_contents()
        {
            IEnumerable<object> arguments = sut.GetWithExtractedArguments(_argumentsWithObjectAndArray);

            Assert.That(arguments.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Should_not_extract_highlights_when_there_are_no_highlights()
        {
            IEnumerable<int> highlights = sut.GetWithExtractedArgumentsToHighlight(_argumentsWithAnArray, _noArgumentsHighlighted);

            Assert.That(highlights, Is.EquivalentTo(_noArgumentsHighlighted));
        }

        [Test]
        public void Should_extract_highlights_from_array_when_array_is_highlighted()
        {
            IEnumerable<int> highlights = sut.GetWithExtractedArgumentsToHighlight(_argumentsWithAnArray, _firstArgumentHighlighted);

            Assert.That(highlights.Count(), Is.EqualTo(2));
            Assert.That(highlights.ToList()[0], Is.EqualTo(0));
            Assert.That(highlights.ToList()[1], Is.EqualTo(1));
        }


        [Test]
        public void Should_not_extract_highlights_from_object_and_array_when_object_is_highlighted()
        {
            IEnumerable<int> highlights = sut.GetWithExtractedArgumentsToHighlight(_argumentsWithObjectAndArray, _firstArgumentHighlighted);

            Assert.That(highlights.Count(), Is.EqualTo(1));
            Assert.That(highlights.ToList()[0], Is.EqualTo(0));
        }

        [Test]
        public void Should_extract_highlights_from_object_and_array_when_array_is_highlighted()
        {
            IEnumerable<int> highlights = sut.GetWithExtractedArgumentsToHighlight(_argumentsWithObjectAndArray, _secondArgumentHighlighted);

            Assert.That(highlights.Count(), Is.EqualTo(2));
            Assert.That(highlights.ToList()[0], Is.EqualTo(1));
            Assert.That(highlights.ToList()[1], Is.EqualTo(2));
        }
    }
}