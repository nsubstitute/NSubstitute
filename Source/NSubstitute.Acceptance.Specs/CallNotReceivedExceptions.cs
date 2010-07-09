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
                ExceptionMessageContains("Expected to receive call:\n\tSampleMethod(5)");
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
                ExceptionMessageContains("Expected to receive call:\n\tSampleMethod(5)");
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
                ExceptionMessageContains("Expected to receive call:\n\tSampleMethod(\"string\", 1, List<String>)");
            }

            [Test]
            public void Should_indicate_which_args_are_different()
            {
                ExceptionMessageContains("SampleMethod(*\"different\"*, 1, *List<String>*)");
                ExceptionMessageContains("SampleMethod(\"string\", *7*, *List<String>*)");
            }
        }

        public class When_checking_call_to_a_property : Context
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
                ExceptionMessageContains("Expected to receive call:\n\tAProperty = 5");
            }

            [Test]
            public void Should_list_actual_calls()
            {
                ExceptionMessageContains("Property = *1*");
            }
        }

        public abstract class Context
        {
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