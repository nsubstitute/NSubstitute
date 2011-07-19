using System;
using System.Collections.Generic;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class CallNotReceivedExceptions
    {
        public interface ISample
        {
            void SampleMethod();
            void SampleMethod(int i);
            void SampleMethod(string s, int i, IList<string> strings);
            void AnotherMethod(IList<string> strings);
            int AProperty { get; set; }
            int this[string a, string b] { get; set; }
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
            public void Should_report_actual_calls()
            {
                ExceptionMessageContains("SampleMethod(*1*)");
                ExceptionMessageContains("SampleMethod(*2*)");
            }
        }

        public class When_calls_have_been_made_to_expected_member_but_with_some_different_args : Context
        {
            readonly IList<string> _strings = new List<string> { "a", "b"};

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
            public void Should_list_actual_calls()
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
            public void Should_list_actual_calls()
            {
                ExceptionMessageContains("this[*\"c\"*, *\"d\"*]"); 
                ExceptionMessageContains("this[\"a\", *\"d\"*]"); 
            }
        }

        public abstract class Context
        {
            protected const string ExpectedCallMessagePrefix = "Expected to receive call:\n\t";
            protected ISample Sample;
            private CallNotReceivedException _exception;

            [SetUp]
            public void SetUp()
            {
                Sample = Substitute.For<ISample>();
                ConfigureContext();
                try
                {
                    ExpectedCall();
                } 
                catch (CallNotReceivedException ex)
                {
                    _exception = ex;
                }
            }

            protected abstract void ExpectedCall();
            protected virtual void ConfigureContext() {}
            protected void ExceptionMessageContains(string expected)
            {
                Assert.That(_exception.Message, Is.StringContaining(expected));
            }
            protected void ExceptionMessageMatchesRegex(string pattern)
            {
                Assert.That(_exception.Message, Is.StringMatching(pattern));
            }
        }        
    }
}