using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class AutoValuesForSubs
    {
        private ISample _sample;

        public class PureVirtualClass { public virtual void Foo() { } }
        public class NonVirtualClass { public void Bar() { } }
        delegate ISample SampleFactory();

        public interface ISample
        {
            int[] GetNumbers();
            IEnumerable<object> GetObjects();
            string Name { get; set; }
            List<string> ListOfStrings { get; set; }
            int? GetNullableNumber();
            PureVirtualClass VirtualClass { get; set; }
            NonVirtualClass NonVirtualClass { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            _sample = Substitute.For<ISample>();
        }

        [Test]
        public void Should_auto_return_empty_array()
        {
            Assert.That(_sample.GetNumbers().Length, Is.EqualTo(0));
        }

        [Test]
        public void Should_auto_return_empty_enumerable()
        {
            Assert.That(_sample.GetObjects(), Is.Not.Null);
            Assert.That(_sample.GetObjects().Count(), Is.EqualTo(0));
        }

        [Test]
        public void Should_auto_return_empty_string()
        {
            Assert.That(_sample.Name.Length, Is.EqualTo(0)); 
        }

        [Test]
        public void Should_return_null_for_nullables()
        {
            Assert.That(_sample.GetNullableNumber(), Is.Null); 
        }

        [Test]
        public void Should_return_same_empty_value_for_auto_values_for_reference_types()
        {
            var autoArrayValue = _sample.GetNumbers();
            Assert.That(_sample.GetNumbers(), Is.SameAs(autoArrayValue));
        }

        [Test]
        public void Should_return_substitute_for_pure_virtual_class()
        {
            Assert.That(_sample.VirtualClass, Is.Not.Null);
        }

        [Test]
        public void Should_return_default_value_for_non_virtual_class()
        {
            Assert.That(_sample.NonVirtualClass, Is.Null);
        }

        [Test]
        [Pending]
        public void Should_auto_return_empty_string_list()
        {
            Assert.That(_sample.ListOfStrings, Is.Not.Null);
            Assert.That(_sample.ListOfStrings.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Should_auto_return_a_substitute_from_a_function_that_returns_an_interface()
        {
            var x = Substitute.For<Func<ISample>>();
            var returnedFromFunc = x();
            Assert.That(returnedFromFunc, Is.Not.Null);
            AssertObjectIsASubstitute(returnedFromFunc);
        }

        [Test]
        public void Should_auto_return_an_empty_string_from_a_func_that_returns_a_string()
        {
            var x = Substitute.For<Func<ISample, string>>();

            Assert.That(x(_sample).Length, Is.EqualTo(0));
        }

        [Test]
        public void Should_auto_return_a_substitute_from_a_func_that_returns_a_pure_virtual_class()
        {
            var x = Substitute.For<Func<PureVirtualClass>>();
            var returnedFromFunc = x();

            Assert.That(returnedFromFunc, Is.Not.Null);
            AssertObjectIsASubstitute(returnedFromFunc);
        }

        [Test]
        public void Should_not_auto_return_a_substitute_from_a_func_that_returns_a_non_virtual_class()
        {
            var x = Substitute.For<Func<NonVirtualClass>>();
            var returnedFromFunc = x();

            Assert.That(returnedFromFunc, Is.Null);
        }

        [Test]
        public void Should_auto_return_a_substitute_from_a_delegate_that_returns_an_interface()
        {
            var x = Substitute.For<SampleFactory>();

            var returnedFromFunc = x();

            Assert.That(returnedFromFunc, Is.Not.Null);
            AssertObjectIsASubstitute(returnedFromFunc);
        }

        private static void AssertObjectIsASubstitute<T>(T obj) where T : class
        {
            Assert.That(obj.ReceivedCalls(), Is.Empty);
        }
    }
}