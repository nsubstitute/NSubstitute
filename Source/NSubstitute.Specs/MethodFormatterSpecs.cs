using System;
using System.Collections.Generic;
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
        public void Should_call_formatter_with_int_and_string()
        {
            AssertCall(x => x.SampleMethod(1, "b"), "Int32, String");
        }

        [Test]
        public void Should_call_formatter_with_generic_type_string()
        {
            AssertCall(x => x.GenericMethod("a"), "String");
        }

        [Test]
        public void Should_call_formatter_with_multiple_types_string_int()
        {
            AssertCall(x => x.GenericMethodWithMultipleTypes("a", 2), "String, Int32");
        }

        [Test]
        public void Should_call_formatter_with_params_empty()
        {
            AssertCall(x => x.MethodWithParams(), "");
        }

        [Test]
        public void Should_call_formatter_with_params_string()
        {
            AssertCall(x => x.MethodWithParams("a"), "String");
        }

        [Test]
        public void Should_call_formatter_params_string_string()
        {
            AssertCall(x => x.MethodWithParams("a", "b"), "String, String");
        }

        [Test]
        public void Should_call_formatter_with_params_string_string_string()
        {
            AssertCall(x => x.MethodWithParams("a", "b", "c"), "String, String, String");
        }

        [Test]
        public void Should_call_formatter_with_params_string_array()
        {
            AssertCall(x => x.MethodWithParams(new []{"a", "b", "c" }), "String, String, String");
        }

        [Test]
        public void Should_call_formatter_with_string_array()
        {
            AssertCall(x => x.MethodWithArray(new[] { "a", "b", "c" }), "String[]");
        }

        [Test]
        public void Should_call_formatter_with_array_and_params()
        {
            AssertCall(x => x.MethodWithArrayAndParams(new [] { "a", "b"}, "c", "d"), "String[], String, String");
        }

        [Test]
        public void Should_call_formatter_with_string_and_params()
        {
            AssertCall(x => x.MethodWithStringAndParams("a", "b", "c", "d"), "String, String, String, String");
        }

        [Test]
        public void Should_call_formatter_with_string_and_empty_params()
        {
            AssertCall(x => x.MethodWithStringAndParams("a"), "String");
        }

        [Test]
        public void Should_call_formatter_with_string_and_params_string_array()
        {
            AssertCall(x => x.MethodWithStringAndParams("a", new [] { "b", "c", "d" }), "String, String, String, String");
        }

        [Test]
        public void Should_call_formatter_with_array_and_params_highlighted()
        {
            AssertCall(x => x.MethodWithArrayAndParams(new[] { "a", "b" }, "c", "d"), "String[], String, String", 1, 1, 2);
        }

        [Test]
        public void Should_call_formatter_with_array_and_params_highlighted2()
        {
            AssertCall(x => x.MethodWithArrayAndParams(new[] { "a", "b" }, "c", "d"), "String[], String, String", 0, 0);
        }

        [Test]
        public void Should_call_formatter_with_string_and_params_highlighted()
        {
            AssertCall(x => x.MethodWithStringAndParams("a", "b", "c", "d"), "String, String, String, String", 1, 1, 2, 3);
        }


        public interface ISample
        {
            void SampleMethod(int a, string b);
            void GenericMethod<T>(T t);
            void GenericMethodWithMultipleTypes<T1, T2>(T1 t1, T2 t2);
            void MethodWithParams(params string[] arguments);
            void MethodWithArray(string[] arguments);
            void MethodWithArrayAndParams(string[] arguments1, params string[] arguments2);
            void MethodWithStringAndParams(string argument1, params string[] arguments2);
        }

        private void AssertCall(Action<ISample> callOnSubstitute, string expectedArguments, int? highlightedIndex = null, params int[] expectedHighlights)
        {
            callOnSubstitute(_sampleSub);
            var call = _sampleSub.ReceivedCalls().First();

            sut.Format(call.GetMethodInfo(), call.GetArguments(), highlightedIndex == null ? new int[0] : new [] { highlightedIndex.Value });
            var formatCall = _argumentsFormatter.ReceivedCalls();
            
            Assert.That(formatCall.Count(), Is.EqualTo(1));
            AssertCallArguments(formatCall.Single().GetArguments(), expectedArguments, expectedHighlights);
        }

        private void AssertCallArguments(object[] callArguments, string expectedArguments, int[] expectedHighlights)
        {
            Assert.That(callArguments.Count(), Is.EqualTo(2));

            AssertFormatArguments(callArguments, expectedArguments);
            AssertHighlights(callArguments, expectedHighlights);
        }

        private void AssertHighlights(object[] callArguments, int[] expectedHighlights)
        {
            var highlightArguments = callArguments[1] as IEnumerable<int>;
            Assert.That(highlightArguments, Is.Not.Null);
            CollectionAssert.AreEqual(expectedHighlights, highlightArguments.ToArray());
        }

        private void AssertFormatArguments(object[] callArguments, string expectedArguments)
        {
            var formatArguments = callArguments[0] as IEnumerable<object>;
            Assert.That(formatArguments, Is.Not.Null);

            if(expectedArguments.Equals(String.Empty))
            {
                CollectionAssert.AreEqual(new string[0], formatArguments.Select(a => a.GetType().Name).ToArray());                                
            }
            else
            {
                CollectionAssert.AreEqual(expectedArguments.Split(',').Select(s => s.Trim()).ToArray(),
                                          formatArguments.Select(a => a.GetType().Name).ToArray());
            }
        }
    }
}