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
                ExceptionMessageContains("Actually received 1 matching call:\r\n\t" + "SampleMethod(2)");
            }

            [Test]
            public void Should_list_actual_related_calls()
            {
                ExceptionMessageContains("Received 1 non-matching call (non-matching arguments indicated with '*' characters):\r\n\t" + "SampleMethod(*1*)");
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
                ExceptionMessageContains("Actually received 3 matching calls:\r\n\t" + "SampleMethod(2)");
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

        [Pending]
        public class When_checking_call_to_method_with_params : Context
        {
            protected override void ConfigureContext()
            {
                Sample.ParamsMethod(2, "hello", "everybody");
                Sample.ParamsMethod(1, new[] {"hello", "everybody"});
                Sample.ParamsMethod(3, "1", "2", "3");
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
                ExceptionMessageContains("ParamsMethod(*2*, *\"hello\", \"everybody\"*)");
                ExceptionMessageContains("ParamsMethod(1, *\"hello\", \"everybody\"*)");
                ExceptionMessageContains("ParamsMethod(*3*, *\"1\", \"2\", \"3\"*)");
            }
        }

        public class When_using_custom_describable_matcher : Context
        {
            private Car _expectedCar;
            private IGarage _sub;
            private Car _delorean;
            private Car _prius2010;

            public class Car
            {
                public Car(string make, string model, int year) { Make = make; Model = model; Year = year; }
                public string Make { get; set; }
                public string Model { get; set; }
                public int Year { get; set; }
            }

            public interface IGarage { void Add(Car c, int quantity); }

            private class CarMatcher : IArgumentMatcher<Car>, IDescribeNonMatches
            {
                private readonly Car _expectedCar;
                public CarMatcher(Car expectedCar) { _expectedCar = expectedCar; }

                public bool IsSatisfiedBy(Car argument)
                {
                    return _expectedCar.Make == argument.Make && _expectedCar.Model == argument.Model && _expectedCar.Year == argument.Year;
                }

                public string DescribeFor(object argument)
                {
                    return "Expected: " + FormatCar(_expectedCar) + "\nActual: " + FormatCar((Car)argument);
                }

                public string FormatCar(Car c) { return string.Format("{0} {1} ({2})", c.Make, c.Model, c.Year); }
                public override string ToString() { return "specific Car"; }
            }

            protected override void ConfigureContext()
            {
                _expectedCar = new Car("Toyota", "Prius", 2012);
                _delorean = new Car("DeLorean", "DMC-12", 1981);
                _prius2010 = new Car("Toyota", "Prius", 2010);

                _sub = Substitute.For<IGarage>();
                _sub.Add(_delorean, 1);
                _sub.Add(_prius2010, 10);
            }

            protected override void ExpectedCall()
            {
                _sub.Received().Add(Arg.Matches(new CarMatcher(_expectedCar)), 10);
            }

            [Test]
            public void Should_show_matcher_string_in_expected_call()
            {
                ExceptionMessageContains(ExpectedCallMessagePrefix + "Add(specific Car, 10)");
            }

            [Test]
            public void Should_show_matcher_description_for_non_matching_Prius()
            {
                ExceptionMessageContains("arg[0]: Expected: Toyota Prius (2012)");
                ExceptionMessageContains("Actual: Toyota Prius (2010)");
            }

            [Test]
            public void Should_show_matcher_description_for_non_matching_DeLorean()
            {
                ExceptionMessageContains("arg[0]: Expected: Toyota Prius (2012)");
                ExceptionMessageContains("Actual: DeLorean DMC-12 (1981)");
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