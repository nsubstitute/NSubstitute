using System;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class MethodFormatterSpecs : ConcernFor<MethodFormatter>
    {
        private IArgumentsFormatter _argumentsFormatter;
        private ISample _sampleSub;

        public override void Context()
        {
            base.Context();
            _argumentsFormatter = Substitute.For<IArgumentsFormatter>();
            _sampleSub = Substitute.For<ISample>();
        }

        public override MethodFormatter CreateSubjectUnderTest()
        {
            return new MethodFormatter(_argumentsFormatter);
        }

        [Test]
        public void Should_format_method_name()
        {
            AssertCall(x => x.SampleMethod(1, "b"), "SampleMethod(args)");
        }

        [Test]
        public void Should_format_generic_type_string()
        {
            AssertCall(x => x.GenericMethod("a"), "GenericMethod<String>(args)");
        }

        [Test]
        public void Should_format_generic_method_with_multiple_types_string_int()
        {
            AssertCall(x => x.GenericMethodWithMultipleTypes("a", 2), "GenericMethodWithMultipleTypes<String, Int32>(args)");
        }

        public interface ISample
        {
            void SampleMethod(int a, string b);
            void GenericMethod<T>(T t);
            void GenericMethodWithMultipleTypes<T1, T2>(T1 t1, T2 t2);
        }

        private void AssertCall(Action<ISample> callOnSubstitute, string expectedFormat)
        {
            callOnSubstitute(_sampleSub);
            var call = _sampleSub.ReceivedCalls().First();

            _argumentsFormatter.Format(null).ReturnsForAnyArgs("args");

            string format = sut.Format(call.GetMethodInfo(), new IArgumentFormatInfo[0]);

            Assert.That(format, Is.EqualTo(expectedFormat));
        }
    }
}