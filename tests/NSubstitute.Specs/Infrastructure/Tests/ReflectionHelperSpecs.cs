using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace NSubstitute.Specs.Infrastructure.Tests
{
    [TestFixture]
    public class ReflectionHelperSpecs
    {
        [Test]
        public void Get_method_info_from_void_call()
        {
            var expectedMethodInfo = GetType().GetMethod("SampleMethod");

            var result = ReflectionHelper.GetMethod(() => SampleMethod());

            Assert.That(result, Is.EqualTo(expectedMethodInfo));
        }

        [Test]
        public void Get_method_info_from_non_void_call()
        {
            var expectedMethodInfo = GetType().GetMethod("NonVoidSampleMethod");

            var result = ReflectionHelper.GetMethod(() => NonVoidSampleMethod());

            Assert.That(result, Is.EqualTo(expectedMethodInfo));
        }

        [Test]
        public void Get_method_info_from_non_void_call_with_args()
        {
            var expectedMethodInfo = GetType().GetMethod("SampleMethodWithArgs");

            var result = ReflectionHelper.GetMethod(() => SampleMethodWithArgs(1, "a"));

            Assert.That(result, Is.EqualTo(expectedMethodInfo));
        }

        public void SampleMethod() { }
        public int NonVoidSampleMethod() { return 0; }
        public int SampleMethodWithArgs(int a, string b) { return 0; }
    }
}