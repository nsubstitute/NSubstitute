using System;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallFormatterSpecs : ConcernFor<CallFormatter>
    {
        private ISample _sampleSub;
        protected int _ignored;

        [Test]
        public void Should_format_method_name_and_arguments()
        {
            AssertCallFormat(x => x.SampleMethod(1, "b"), "SampleMethod(1, b)");
        }

        [Test]
        public void Should_format_generic_method_and_arguments()
        {
            AssertCallFormat(x => x.GenericMethod(1), "GenericMethod<Int32>(1)");
        }

        [Test]
        public void Should_format_method_with_multiple_generic_args()
        {
            AssertCallFormat(x => x.GenericMethodWithMultipleTypes(1, "b"), "GenericMethodWithMultipleTypes<Int32, String>(1, b)"); 
        }

        [Test]
        public void Should_format_property_set()
        {
            AssertCallFormat(x => x.Property = 2, "Property = 2");
        }

        [Test]
        public void Should_format_property_get()
        {
            AssertCallFormat(x => { _ignored = x.Property; }, "Property");
        }

        [Test]
        public void Should_format_indexer_setter()
        {
            AssertCallFormat(x => x["a", "b"] = 4, "this[a, b] = 4"); 
        }

        [Test]
        public void Should_formated_indexer_getter()
        {
            AssertCallFormat(x => { _ignored = x["a", "b"]; }, "this[a, b]");
        }

        [Test]
        public void Should_format_event_subscription()
        {
            AssertCallFormat(x => x.AnEvent += null, "AnEvent += null");  
        }

        [Test]
        public void Should_format_event_unsubscription()
        {
            AssertCallFormat(x => x.AnEvent -= null, "AnEvent -= null");  
        }

        public override void Context()
        {
            base.Context();
            _sampleSub = Substitute.For<ISample>();
        }

        public override CallFormatter CreateSubjectUnderTest()
        {
            return new CallFormatter();
        }

        private void AssertCallFormat(Action<ISample> callOnSubstitute, string expectedFormat)
        {
            callOnSubstitute(_sampleSub);
            var call =  _sampleSub.ReceivedCalls().First();
            var format = sut.Format(call.GetMethodInfo(), call.GetArguments().Select(x => string.Format("{0}", x ?? "null")));
            Assert.That(format, Is.EqualTo(expectedFormat));
        }

        public interface ISample
        {
            void SampleMethod(int a, string b);
            void GenericMethod<T>(T t);
            void GenericMethodWithMultipleTypes<T1, T2>(T1 t1, T2 t2);
            int Property { get; set; }
            int this[string a, string b] { get; set; }
            event Action AnEvent;
        }
    }
}