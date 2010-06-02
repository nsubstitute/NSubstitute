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
            string AProperty { get; set; }
        }

        public class When_no_calls_have_been_made_at_all : Context
        {
            protected override void ExpectedCall()
            {
                Sample.Received().SampleMethod(5);
            }

            [Test][Pending]
            public void Should_report_the_method_we_were_expecting()
            {
                ExceptionMessageMatchesRegex(@"Expected to receive call:\n\tSampleMethod\(5\)");
            }

            [Test][Pending]
            public void Should_report_that_we_did_not_receive_any_calls()
            {
                ExceptionMessageMatchesRegex(@"Actually received:\n\tNo calls");
            }
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

            [Test][Pending]
            public void Should_list_all_calls_made_to_substitute()
            {
                ExceptionMessageMatchesRegex(@"Actually received:\n\tAnotherMethod\(.*?\)\n\tAnotherMethod\(.*?\)");   
            }
        }

        public class When_checking_call_to_a_property : Context
        {
            protected override void ConfigureContext()
            {
                Sample.AProperty = "a";
            }

            protected override void ExpectedCall()
            {
                Sample.Received().AProperty = "b";
            }

            [Test][Pending]
            public void Should_list_expected_property_set()
            {
                ExceptionMessageMatchesRegex(@"Expected to receive call:\n\tAProprety \= b");
            }

            [Test][Pending]
            public void Should_list_actual_calls()
            {
                ExceptionMessageMatchesRegex(@"Actual calls:\n\tAProperty \= a");
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
                Assert.That(_exception.Message, Text.Contains(expected));
            }
            protected void ExceptionMessageMatchesRegex(string pattern)
            {
                Assert.That(_exception.Message, Text.Matches(pattern));
            }
        }        
    }
}