using System;
using System.Collections.Generic;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class FormattingCallsWhenThrowingReceivedCallsExceptions
    {
        public interface ISample
        {
            void SampleMethod();
            void SampleMethod(int i);
            void SampleMethod(string s, int i, IList<string> strings);
            void AnotherMethod(IList<string> strings);
            int AProperty { get; set; }
            int this[string a, string b] { get; set; }
            void ParamsMethod(int a, params string[] strings);
            void IntParamsMethod(params int[] ints);
        }

        public class When_no_calls_are_made_to_the_expected_member : Context
        {
            protected override void ConfigureContext()
            {
                Sample.AnotherMethod(new List<string>());
                Sample.AnotherMethod(new List<string>());
            }

            protected override void ExpectedCall()
            {
                Sample.Received().SampleMethod(5);
            }

            [Test]
            public void Should_report_the_method_we_were_expecting()
            {
                ExceptionMessageContains(ExpectedCallMessagePrefix + "SampleMethod(5)");
            }
        }

        public class When_calls_have_been_made_to_expected_member_but_with_different_arg : Context
        {
            protected override void ConfigureContext()
            {
                Sample.SampleMethod(1);
                Sample.SampleMethod(2);
            }

            protected override void ExpectedCall()
            {
                Sample.Received().SampleMethod(5);
            }

            [Test]
            public void Should_report_the_method_we_were_expecting()
            {
                ExceptionMessageContains(ExpectedCallMessagePrefix + "SampleMethod(5)");
            }

            [Test]
            public void Should_report_non_matching_calls()
            {
                ExceptionMessageContains("Received 2 non-matching calls (non-matching arguments indicated with '*' characters):");
                ExceptionMessageContains("SampleMethod(*1*)");
                ExceptionMessageContains("SampleMethod(*2*)");
            }
        }

        public class When_calls_have_been_made_to_expected_member_but_with_some_different_args : Context
        {
            readonly IList<string> _strings = new List<string> { "a", "b" };

            protected override void ConfigureContext()
            {
                Sample.SampleMethod("different", 1, new List<string>());
                Sample.SampleMethod("string", 7, new List<string>());
            }

            protected override void ExpectedCall()
            {
                Sample.Received().SampleMethod("string", 1, _strings);
            }

            [Test]
            public void Should_report_the_method_we_were_expecting()
            {
                ExceptionMessageContains(ExpectedCallMessagePrefix + "SampleMethod(\"string\", 1, List<String>)");
            }

            [Test]
            public void Should_indicate_which_args_are_different()
            {
                ExceptionMessageContains("SampleMethod(*\"different\"*, 1, *List<String>*)");
                ExceptionMessageContains("SampleMethod(\"string\", *7*, *List<String>*)");
            }
        }

        public class When_not_enough_matching_calls_were_made_to_a_method : Context
        {
            protected override void ConfigureContext()
            {
                Sample.SampleMethod(1);
                Sample.SampleMethod(2);
            }

            protected override void ExpectedCall()
            {
                Sample.Received(2).SampleMethod(2);
            }

            [Test]
            public void Should_show_expected_call()
            {
                ExceptionMessageContains("Expected to receive exactly 2 calls matching:\n\t" + "SampleMethod(2)");
            }

            [Test]
            public void Should_list_matching_calls()
            {
                ExceptionMessageContains("Actually received 1 matching call:" + Environment.NewLine + "\t" + "SampleMethod(2)");
            }

            [Test]
            public void Should_list_actual_related_calls()
            {
                ExceptionMessageContains("Received 1 non-matching call (non-matching arguments indicated with '*' characters):" + Environment.NewLine + "\t" + "SampleMethod(*1*)");
            }
        }

        public class When_too_many_matching_calls_were_made_to_a_method : Context
        {
            protected override void ConfigureContext()
            {
                Sample.SampleMethod(1);
                Sample.SampleMethod(2);
                Sample.SampleMethod(2);
                Sample.SampleMethod(2);
            }

            protected override void ExpectedCall()
            {
                Sample.Received(2).SampleMethod(2);
            }

            [Test]
            public void Should_show_expected_call()
            {
                ExceptionMessageContains("Expected to receive exactly 2 calls matching:\n\t" + "SampleMethod(2)");
            }

            [Test]
            public void Should_list_matching_calls()
            {
                ExceptionMessageContains("Actually received 3 matching calls:" + Environment.NewLine + "\t" + "SampleMethod(2)");
            }

            [Test]
            public void Should_not_list_non_matching_calls()
            {
                ExceptionMessageDoesNotContain("Received 1 related call");
                ExceptionMessageDoesNotContain("SampleMethod(*1*)");
            }
        }

        public class When_checking_call_to_a_property_setter : Context
        {
            protected override void ConfigureContext()
            {
                Sample.AProperty = 1;
            }

            protected override void ExpectedCall()
            {
                Sample.Received().AProperty = 5;
            }

            [Test]
            public void Should_list_expected_property_set()
            {
                ExceptionMessageContains(ExpectedCallMessagePrefix + "AProperty = 5");
            }

            [Test]
            public void Should_list_actual_calls()
            {
                ExceptionMessageContains("Property = *1*");
            }
        }

        public class When_checking_call_to_a_property_getter : Context
        {
            protected int _ignored;

            protected override void ExpectedCall()
            {
                _ignored = Sample.Received().AProperty;
            }

            [Test]
            public void Should_list_expected_property_get()
            {
                ExceptionMessageContains(ExpectedCallMessagePrefix + "AProperty");
            }
        }

        public class When_checking_call_to_an_indexer_setter : Context
        {
            protected override void ConfigureContext()
            {
                Sample["c", "d"] = 5;
                Sample["a", "z"] = 2;
                Sample["a", "b"] = 1;
            }

            protected override void ExpectedCall()
            {
                Sample.Received()["a", "b"] = 5;
            }

            [Test]
            public void Should_list_expected_indexer_set()
            {
                ExceptionMessageContains(ExpectedCallMessagePrefix + "this[\"a\", \"b\"] = 5");
            }

            [Test]
            public void Should_list_non_matching_calls()
            {
                ExceptionMessageContains("this[*\"c\"*, *\"d\"*] = 5");
                ExceptionMessageContains("this[\"a\", *\"z\"*] = *2*");
                ExceptionMessageContains("this[\"a\", \"b\"] = *1*");
            }
        }

        public class When_checking_call_to_an_indexer_getter : Context
        {
            protected int _ignored;

            protected override void ConfigureContext()
            {
                _ignored = Sample["c", "d"];
                _ignored = Sample["a", "d"];
            }

            protected override void ExpectedCall()
            {
                _ignored = Sample.Received()["a", "b"];
            }

            [Test]
            public void Should_list_expected_indexer_get()
            {
                ExceptionMessageContains(ExpectedCallMessagePrefix + "this[\"a\", \"b\"]");
            }

            [Test]
            public void Should_list_non_matching_calls()
            {
                ExceptionMessageContains("this[*\"c\"*, *\"d\"*]");
                ExceptionMessageContains("this[\"a\", *\"d\"*]");
            }
        }

        public class When_checking_call_to_method_with_params : Context
        {
            protected override void ConfigureContext()
            {
                Sample.ParamsMethod(2, "hello", "everybody");
                Sample.ParamsMethod(1, new[] {"hello", "everybody"});
                Sample.ParamsMethod(1, "hello");
                Sample.ParamsMethod(3, "1", "2", "3");
                Sample.ParamsMethod(1);
            }

            protected override void ExpectedCall()
            {
                Sample.Received().ParamsMethod(1, "hello", "world");
            }

            [Test]
            public void Should_show_expected_call()
            {
                ExceptionMessageContains("ParamsMethod(1, \"hello\", \"world\")");
            }

            [Test]
            public void Should_show_non_matching_calls_with_params_expanded()
            {
                ExceptionMessageContains("ParamsMethod(*2*, \"hello\", *\"everybody\"*)");
                ExceptionMessageContains("ParamsMethod(1, \"hello\", *\"everybody\"*)");
                ExceptionMessageContains("ParamsMethod(1, \"hello\")");
                ExceptionMessageContains("ParamsMethod(*3*, *\"1\"*, *\"2\"*, *\"3\"*)");
                ExceptionMessageContains("ParamsMethod(1, **)");
            }
        }

        public class When_checking_call_to_method_with_valuetype_params : Context
        {
            protected override void ConfigureContext()
            {
                Sample.IntParamsMethod(1, 2, 3);
                Sample.IntParamsMethod(new[] {4, 5});
                Sample.IntParamsMethod();
            }

            protected override void ExpectedCall()
            {
                Sample.Received().IntParamsMethod(1, 200, 300);
            }

            [Test]
            public void Should_show_expected_call()
            {
                ExceptionMessageContains("IntParamsMethod(1, 200, 300)");
            }

            [Test]
            public void Should_show_non_matching_calls_with_params_expanded()
            {
                ExceptionMessageContains("IntParamsMethod(1, *2*, *3*)");
                ExceptionMessageContains("IntParamsMethod(*4*, *5*)");
                ExceptionMessageContains("IntParamsMethod(**)");
            }
        }

        public class When_checking_call_to_method_with_params_specified_as_an_array : Context
        {
            protected override void ConfigureContext()
            {
                Sample.ParamsMethod(2, "hello", "everybody");
            }

            protected override void ExpectedCall()
            {
                Sample.Received().ParamsMethod(1, Arg.Is(new[] {"hello", "world"}));
            }

            [Test]
            public void Should_show_expected_call()
            {
                ExceptionMessageContains("ParamsMethod(1, String[])");
            }

            [Test]
            public void Should_show_non_matching_calls_as_per_specification_rather_than_as_individual_params()
            {
                ExceptionMessageContains("ParamsMethod(*2*, *String[]*)");
            }
        }

        public class When_checking_delegate_call : Context
        {
            private Func<int, string, string> _func;

            protected override void ConfigureContext()
            {
                _func = Substitute.For<Func<int, string, string>>();
                _func(1, "def");
                _func(2, "def");
                _func(3, "abc");
            }

            protected override void ExpectedCall()
            {
                _func.Received()(1, "abc");
            }

            [Test]
            public void Should_show_expected_call()
            {
                ExceptionMessageContains("Invoke(1, \"abc\")");
            }

            [Test]
            public void Should_show_non_matching_calls()
            {
                ExceptionMessageContains("Invoke(1, *\"def\"*)");
                ExceptionMessageContains("Invoke(*2*, *\"def\"*)");
                ExceptionMessageContains("Invoke(*3*, \"abc\")");

            }
        }

        public abstract class Context
        {
            protected const string ExpectedCallMessagePrefix = "Expected to receive a call matching:\n\t";

            protected ISample Sample;
            private ReceivedCallsException _exception;

            [SetUp]
            public void SetUp()
            {
                Sample = Substitute.For<ISample>();
                ConfigureContext();
                try
                {
                    ExpectedCall();
                }
                catch (ReceivedCallsException ex)
                {
                    _exception = ex;
                }
            }

            protected abstract void ExpectedCall();

            protected virtual void ConfigureContext()
            {
            }

            protected void ExceptionMessageContains(string expected)
            {
                Assert.That(_exception.Message, Is.StringContaining(expected));
            }

            protected void ExceptionMessageDoesNotContain(string s)
            {
                Assert.That(_exception.Message, Is.Not.StringContaining(s));
            }
            protected void ExceptionMessageMatchesRegex(string pattern)
            {
                Assert.That(_exception.Message, Is.StringMatching(pattern));
            }
        }
    }
}
