using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public abstract class ObjectParamsArrayArgumentFormatInfoSpecs : ConcernFor<ObjectParamsArrayArgumentFormatInfo>
    {
        public class When_a_string_array_is_formatted : ObjectParamsArrayArgumentFormatInfoSpecs
        {
            private ArgumentFormatter _argumentFormatter;

            public override ObjectParamsArrayArgumentFormatInfo CreateSubjectUnderTest()
            {
                return new ObjectParamsArrayArgumentFormatInfo(new[] { "foo", "bar" }, false);
            }


            public override void Context()
            {
                _argumentFormatter = new ArgumentFormatter();
            }

            [Test]
            public void Should_format_a_string_array()
            {
                Assert.That(sut.Format(_argumentFormatter), Is.EqualTo("\"foo\", \"bar\""));
            }
        }

        public class When_a_string_array_is_formatted_and_highlighted : ObjectParamsArrayArgumentFormatInfoSpecs
        {
            private ArgumentFormatter _argumentFormatter;

            public override ObjectParamsArrayArgumentFormatInfo CreateSubjectUnderTest()
            {
                return new ObjectParamsArrayArgumentFormatInfo(new[] {"foo", "bar"}, true);
            }


            public override void Context()
            {
                _argumentFormatter = new ArgumentFormatter();
            }
            
            [Test]
            public void Should_format_a_string_array()
            {
                Assert.That(sut.Format(_argumentFormatter), Is.EqualTo("*\"foo\", \"bar\"*"));
            }
        }
    }
}