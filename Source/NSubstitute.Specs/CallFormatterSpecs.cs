using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallFormatterSpecs : ConcernFor<CallFormatter>
    {
        private ISample _sampleSub;
        private IArgumentFormatter _argumentFormatter;

        [Test]
        public void Should_format_method_name_and_arguments()
        {
            AssertCallFormat(x => x.SampleMethod(1, "b"), "SampleMethod(arg, arg)");
        }

        [Test]
        public void Should_format_generic_method_and_arguments()
        {
            AssertCallFormat(x => x.GenericMethod(1), "GenericMethod<Int32>(arg)");
        }

        [Test]
        public void Should_format_method_with_multiple_generic_args()
        {
            AssertCallFormat(x => x.GenericMethodWithMultipleTypes(1, "b"), "GenericMethodWithMultipleTypes<Int32, String>(arg, arg)"); 
        }

        [Test]
        public void Should_format_property_set()
        {
            AssertCallFormat(x => x.Property = 2, "Property = arg");
        }

        [Test]
        public void Should_highlight_specified_arguments()
        {
            AssertCallFormat(x => x.SampleMethod(1, "b"), new[] {1}, "SampleMethod(arg, *arg*)");
        }

        public override void Context()
        {
            base.Context();
            _argumentFormatter = mock<IArgumentFormatter>();
            _argumentFormatter.stub(x => x.Format("any")).IgnoreArguments().Return("arg");
            _sampleSub = Substitute.For<ISample>();
        }

        public override CallFormatter CreateSubjectUnderTest()
        {
            return new CallFormatter(_argumentFormatter);
        }

        private void AssertCallFormat(Action<ISample> callOnSubstitute, string expectedFormat)
        {
            AssertCallFormat(callOnSubstitute, new int[0], expectedFormat);
        }

        private void AssertCallFormat(Action<ISample> callOnSubstitute, IEnumerable<int> argumentsToHighlight, string expectedFormat)
        {
            callOnSubstitute(_sampleSub);
            var call =  _sampleSub.ReceivedCalls().First();
            var format = sut.Format(call.GetMethodInfo(), call.GetArguments(), argumentsToHighlight);
            Assert.That(format, Is.EqualTo(expectedFormat));
        }

        public interface ISample
        {
            void SampleMethod(int a, string b);
            void GenericMethod<T>(T t);
            void GenericMethodWithMultipleTypes<T1, T2>(T1 t1, T2 t2);
            int Property { get; set; }
        }
    }
}