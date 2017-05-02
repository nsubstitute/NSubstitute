using System.Collections.Generic;
using NSubstitute.Core.Arguments;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class PropertyEqualsArgumentMatcherSpecs
    {
        #region Setup

        public class TestClass
        {
            public TestClass(int intProperty, string stringProperty, IList<string> refProperty, int privateProperty = 0)
            {
                IntProperty = intProperty;
                StringProperty = stringProperty;
                RefProperty = refProperty;
                PrivateProperty = privateProperty;
            }

            public int IntProperty { get; private set; }
            public string StringProperty { get; private set; }
            public IList<string> RefProperty { get; private set; }
            private int PrivateProperty { get; set; }
        }

        public class TestCase
        {
            public TestClass TestObj1 { get; set; }
            public TestClass TestObj2 { get; set; }
        }

        private static IEnumerable<TestCase> GetNonMatchingArguments()
        {
            var list = new List<string>();

            yield return new TestCase
            {
                TestObj1 = new TestClass(1, "Test", list),
                TestObj2 = new TestClass(2, "Test", list)
            };

            yield return new TestCase
            {
                TestObj1 = new TestClass(1, "Some string", list),
                TestObj2 = new TestClass(1, "another string", list)
            };

            // Note that this a shallow comparison so any object properties must be Reference equal (list in this case)
            yield return new TestCase
            {
                TestObj1 = new TestClass(1, "Test", new List<string>()),
                TestObj2 = new TestClass(2, "Test", new List<string>())
            };
        }

        #endregion

        [Test]
        public void Should_match_when_arguments_have_same_public_property_values()
        {
            var list = new List<string>();
            var testObj1 = new TestClass(1, "Test", list);
            var testObj2 = new TestClass(1, "Test", list);

            Assert.That(new PropertyEqualsArgumentMatcher(testObj1).IsSatisfiedBy(testObj2));
        }

        [Test]
        public void Should_match_when_only_private_properties_differ()
        {
            var list = new List<string>();
            var testObj1 = new TestClass(1, "Test", list, 1);
            var testObj2 = new TestClass(1, "Test", list, 10);

            Assert.That(new PropertyEqualsArgumentMatcher(testObj1).IsSatisfiedBy(testObj2));
        }

        [Test]
        [TestCaseSource("GetNonMatchingArguments")]
        public void Should_not_match_when_public_properties_differ(TestCase testCase)
        {
            Assert.That(new PropertyEqualsArgumentMatcher(testCase.TestObj1).IsSatisfiedBy(testCase.TestObj2), Is.False);
        }
    }
}
