using System.Collections.Generic;
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
            var arguments = new object[] { 1, "s" };
            var result = sut.Format(methodInfo, arguments);
            Assert.That(result, Is.EqualTo("SampleMethod(1, \"s\")"));
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