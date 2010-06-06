using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallFormatterSpecs : ConcernFor<CallFormatter>
    {
        [Test]
        public void Should_format_method_name()
        {
            var methodInfo = typeof(ISample).GetMethod("SampleMethod");
            var result = sut.Format(methodInfo);
            Assert.That(result, Is.EqualTo("SampleMethod"));
        }

        public override CallFormatter CreateSubjectUnderTest()
        {
            return new CallFormatter();
        }

        public interface ISample
        {
            void SampleMethod(int a, string b);
        }
    }
}