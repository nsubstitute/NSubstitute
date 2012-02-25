using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class When_a_string_is_formatted : ConcernFor<ObjectArgumentFormatInfo>
    {
        private IArgumentFormatter _argumentFormatter;

        public override ObjectArgumentFormatInfo CreateSubjectUnderTest()
        {
            return new ObjectArgumentFormatInfo("foo", false);
        }

        public override void Context()
        {
            _argumentFormatter = new ArgumentFormatter();
        }

        [Test]
        public void Should_format_a_string_object()
        {
            Assert.That(sut.Format(_argumentFormatter), Is.EqualTo("\"foo\""));
        }
    }

    public class When_a_string_is_formatted_and_highlighted : ConcernFor<ObjectArgumentFormatInfo>
    {
        private ArgumentFormatter _argumentFormatter;

        public override ObjectArgumentFormatInfo CreateSubjectUnderTest()
        {
            return new ObjectArgumentFormatInfo("foo", true);
        }


        public override void Context()
        {
            _argumentFormatter = new ArgumentFormatter();
        }

        [Test]
        public void Should_format_a_string_object()
        {
            Assert.That(sut.Format(_argumentFormatter), Is.EqualTo("*\"foo\"*"));
        }
    }

    public class When_a_string_array_is_formatted : ConcernFor<ObjectArgumentFormatInfo>
    {
        private ArgumentFormatter _argumentFormatter;

        public override ObjectArgumentFormatInfo CreateSubjectUnderTest()
        {
            return new ObjectArgumentFormatInfo(new[] { "foo", "bar" }, false);
        }


        public override void Context()
        {
            _argumentFormatter = new ArgumentFormatter();
        }

        [Test]
        public void Should_format_a_string_array()
        {
            Assert.That(sut.Format(_argumentFormatter), Is.EqualTo("String[]"));
        }
    }

    public class When_a_string_array_is_formatted_and_highlighted : ConcernFor<ObjectArgumentFormatInfo>
    {
        private ArgumentFormatter _argumentFormatter;

        public override ObjectArgumentFormatInfo CreateSubjectUnderTest()
        {
            return new ObjectArgumentFormatInfo(new[] {"foo", "bar"}, true);
        }


        public override void Context()
        {
            _argumentFormatter = new ArgumentFormatter();
        }

        [Test]
        public void Should_format_a_string_array()
        {
            Assert.That(sut.Format(_argumentFormatter), Is.EqualTo("*String[]*"));
        }
    }
}
