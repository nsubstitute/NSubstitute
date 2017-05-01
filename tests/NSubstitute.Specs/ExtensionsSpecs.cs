using System;
using System.Collections;
using NSubstitute.Core;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ExtensionsSpecs
    {
        public class ZipExtension
        {
            [Test]
            public void Zip_two_equal_collections()
            {
                var ints = new[] { 1, 2, 3 };
                var strings = new[] { "a", "b", "c" };

                var zipped = ints.Zip(strings, (i, s) => i + s);
                Assert.That(zipped, Is.EqualTo(new[] { "1a", "2b", "3c" }));
            }
        }

        public class IsCompatibleWithExtension
        {
            [Test]
            public void Compatible_types()
            {
                var s = "a string";
                Assert.That(s.IsCompatibleWith(typeof(IEnumerable)));
            }

            [Test]
            public void Incompatible_types()
            {
                Assert.False(3.IsCompatibleWith(typeof(string)));
            }

            [Test]
            public void Null_with_reference_type()
            {
                string s = null;
                Assert.That(s.IsCompatibleWith(typeof(object)));
            }

            [Test]
            public void Null_with_value_type()
            {
                string s = null;
                Assert.False(s.IsCompatibleWith(typeof(int)));
            }

            [Test]
            public void Compatible_type_passed_by_reference()
            {
                var intByRefType = GetIntByRefType();
                Assert.That(3.IsCompatibleWith(intByRefType));
            }

            [Test]
            public void Incompatible_type_passed_by_reference()
            {
                var intByRefType = GetIntByRefType();
                Assert.False("hello".IsCompatibleWith(intByRefType));
            }

            private Type GetIntByRefType()
            {
                return GetType().GetMethod("MethodWithARefArg").GetParameters()[0].ParameterType;
            }

            public void MethodWithARefArg(ref int i) { }
        }

        public class JoinExtension
        {
            [Test]
            public void Join_strings()
            {
                var strings = new[] { "hello", "world" };
                Assert.That(strings.Join("; "), Is.EqualTo("hello; world"));
            }
        }
    }
}