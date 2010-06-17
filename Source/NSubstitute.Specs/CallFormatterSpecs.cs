using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallFormatterSpecs : ConcernFor<CallFormatter>
    {
        [Test]
        public void Should_format_method_name_and_arguments()
        {
            var methodInfo = typeof(ISample).GetMethod("SampleMethod");
            var spec1 = mock<IArgumentSpecification>();
            var spec2 = mock<IArgumentSpecification>();
            var arguments = new [] { spec1, spec2 };
            var result = sut.Format(methodInfo, arguments);
            Assert.That(result, Is.EqualTo("SampleMethod(" + spec1 + ", " + spec2 + ")"));
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