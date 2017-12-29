﻿using System;
using System.Threading.Tasks;
using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Exceptions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class ReturningResults
    {
        private ISomething _something;

        [Test]
        public void Return_a_single_result()
        {
            _something.Count().Returns(3);

            Assert.That(_something.Count(), Is.EqualTo(3), "First return");
            Assert.That(_something.Count(), Is.EqualTo(3), "Second return");
        }

        [Test]
        public void Return_multiple_results_from_different_calls()
        {
            _something.Echo(1).Returns("one");
            _something.Echo(2).Returns("two");

            Assert.That(_something.Echo(1), Is.EqualTo("one"), "First return");
            Assert.That(_something.Echo(2), Is.EqualTo("two"), "Second return");
        }

        [Test]
        public void Return_multiple_results_from_the_same_call()
        {
            _something.Count().Returns(1, 2, 3);

            Assert.That(_something.Count(), Is.EqualTo(1), "First return");
            Assert.That(_something.Count(), Is.EqualTo(2), "Second return");
            Assert.That(_something.Count(), Is.EqualTo(3), "Third return");
            Assert.That(_something.Count(), Is.EqualTo(3), "Fourth return");
        }

        [Test]
        public void Return_multiple_results_from_funcs()
        {
            _something.Count().Returns(
                _ => 1,
                _ => 2,
                _ => 3);

            Assert.That(_something.Count(), Is.EqualTo(1), "First return");
            Assert.That(_something.Count(), Is.EqualTo(2), "Second return");
            Assert.That(_something.Count(), Is.EqualTo(3), "Third return");
            Assert.That(_something.Count(), Is.EqualTo(3), "Fourth return");
        }

        [Test]
        public void Return_multiple_results_from_funcs_throws_exception()
        {
            _something.Count().Returns(
                _ => 1,
                _ => { throw new Exception("Count() execution failed."); },
                _ => 3);

            Assert.That(_something.Count(), Is.EqualTo(1), "First return");
            Assert.Throws<Exception>(() => this._something.Count());
            Assert.That(_something.Count(), Is.EqualTo(3), "Third return");
        }

        [Test]
        public void Return_multiple_results_from_funcs_for_any_arguments()
        {
            _something.Echo(1).ReturnsForAnyArgs(_ => "first", _ => "second");

            Assert.That(_something.Echo(2), Is.EqualTo("first"));
            Assert.That(_something.Echo(724), Is.EqualTo("second"));
        }

        [Test]
        public void Return_result_for_any_arguments()
        {
            _something.Echo(1).ReturnsForAnyArgs("always");

            Assert.That(_something.Echo(1), Is.EqualTo("always"));
            Assert.That(_something.Echo(2), Is.EqualTo("always"));
            Assert.That(_something.Echo(724), Is.EqualTo("always"));
        }

        [Test]
        public void Return_multiple_results_for_any_arguments()
        {
            _something.Echo(1).ReturnsForAnyArgs("first", "second");

            Assert.That(_something.Echo(2), Is.EqualTo("first"));
            Assert.That(_something.Echo(724), Is.EqualTo("second"));
        }

        [Test]
        public void Return_result_for_any_arguments_async()
        {
            _something.EchoAsync(1).ReturnsForAnyArgs("always");

            Assert.That(_something.EchoAsync(1).Result, Is.EqualTo("always"));
            Assert.That(_something.EchoAsync(2).Result, Is.EqualTo("always"));
            Assert.That(_something.EchoAsync(724).Result, Is.EqualTo("always"));
        }

        [Test]
        public void Return_multiple_results_for_any_arguments_async()
        {
            _something.EchoAsync(1).ReturnsForAnyArgs("first", "second");

            Assert.That(_something.EchoAsync(2).Result, Is.EqualTo("first"));
            Assert.That(_something.EchoAsync(724).Result, Is.EqualTo("second"));
        }

#if !NET40
        [Test]
        public void Return_result_for_any_arguments_async_ValueTask()
        {
            _something.EchoValueTaskAsync(1).ReturnsForAnyArgs("always");

            Assert.That(_something.EchoValueTaskAsync(1).Result, Is.EqualTo("always"));
            Assert.That(_something.EchoValueTaskAsync(2).Result, Is.EqualTo("always"));
            Assert.That(_something.EchoValueTaskAsync(724).Result, Is.EqualTo("always"));
        }

        [Test]
        public void Return_multiple_results_for_any_arguments_async_ValueTask()
        {
            _something.EchoValueTaskAsync(1).ReturnsForAnyArgs("first", "second");

            Assert.That(_something.EchoValueTaskAsync(2).Result, Is.EqualTo("first"));
            Assert.That(_something.EchoValueTaskAsync(724).Result, Is.EqualTo("second"));
        }
#endif

        [Test]
        public void Return_multiple_results_from_funcs_for_any_arguments_async()
        {
            _something.EchoAsync(1).ReturnsForAnyArgs(_ => "first", _ => "second");

            Assert.That(_something.EchoAsync(2).Result, Is.EqualTo("first"));
            Assert.That(_something.EchoAsync(724).Result, Is.EqualTo("second"));
        }

#if !NET40
        [Test]
        public void Return_multiple_results_from_funcs_for_any_arguments_async_ValueTask()
        {
            _something.EchoValueTaskAsync(1).ReturnsForAnyArgs(_ => "first", _ => "second");

            Assert.That(_something.EchoValueTaskAsync(2).Result, Is.EqualTo("first"));
            Assert.That(_something.EchoValueTaskAsync(724).Result, Is.EqualTo("second"));
        }
#endif

        [Test]
        public void Return_calculated_results_for_any_arguments()
        {
            _something.Echo(-2).ReturnsForAnyArgs(x => x[0].ToString());

            Assert.That(_something.Echo(12), Is.EqualTo(12.ToString()));
            Assert.That(_something.Echo(123), Is.EqualTo(123.ToString()));
        }

        [Test]
        [Pending, Explicit]
        public void Return_specific_value_for_tostring()
        {
            _something.ToString().Returns("this string");
            Assert.That(_something.ToString(), Is.EqualTo("this string"));
        }

        [Test]
        public void Throw_when_blatantly_misusing_returns()
        {
            const string expectedMessagePrefix = "Could not find a call to return from.";

            var exception = Assert.Throws<CouldNotSetReturnDueToNoLastCallException>(() =>
              {
                  //Start with legitimate call to Returns (so the static context will not have any residual calls stored).
                  _something.Echo(1).Returns("one");
                  //Now we'll misuse Returns.
                  "".Returns("I shouldn't be calling returns like this!");
              });
            Assert.That(exception.Message, Does.Contain(expectedMessagePrefix));
        }

        [Test]
        public void Returns_Null_for_string_parameter()
        {
            const string stringValue = "something";
            _something.Say(stringValue).ReturnsNull();

            Assert.That(_something.Say("something"), Is.Null);
        }

        [Test]
        public void Returns_Null_for_method_returning_class()
        {
            _something.SomeAction().ReturnsNull();
            
            Assert.That(_something.SomeAction(), Is.Null);
        }

        [Test]
        public void Returns_Null_for_any_args_when_string_parameter()
        {
            _something.Say("text").ReturnsNullForAnyArgs();

            Assert.That(_something.Say("something"), Is.Null);
        }

        [Test]
        public void Returns_Null_for_any_args_when_class_returned()
        {
            _something.SomeActionWithParams(2, "text").ReturnsNullForAnyArgs();

            Assert.That(_something.SomeActionWithParams(123, "something else"), Is.Null);
        }

        [Test]
        public void Returns_Null_for_string_parameter_async()
        {
            const string stringValue = "something";
            _something.SayAsync(stringValue).ReturnsNull();

            Assert.That(_something.SayAsync("something").Result, Is.Null);
        }

#if !NET40
        [Test]
        public void Returns_Null_for_string_parameter_async_ValueTask()
        {
            const string stringValue = "something";
            _something.SayValueTaskAsync(stringValue).ReturnsNull();

            Assert.That(_something.SayValueTaskAsync("something").Result, Is.Null);
        }
#endif

        [Test]
        public void Returns_Null_for_method_returning_class_async()
        {
            _something.SomeActionAsync().ReturnsNull();

            Assert.That(_something.SomeActionAsync().Result, Is.Null);
        }

#if !NET40
        [Test]
        public void Returns_Null_for_method_returning_class_async_ValueTask()
        {
            _something.SomeActionValueTaskAsync().ReturnsNull();

            Assert.That(_something.SomeActionValueTaskAsync().Result, Is.Null);
        }
#endif

        [Test]
        public void Returns_Null_for_any_args_when_string_parameter_async()
        {
            _something.SayAsync("text").ReturnsNullForAnyArgs();

            Assert.That(_something.SayAsync("something").Result, Is.Null);
        }

        [Test]
        public void Returns_Null_for_any_args_when_class_returned_async()
        {
            _something.SomeActionWithParamsAsync(2, "text").ReturnsNullForAnyArgs();

            Assert.That(_something.SomeActionWithParamsAsync(123, "something else").Result, Is.Null);
        }

#if !NET40
        [Test]
        public void Returns_Null_for_any_args_when_string_parameter_async_ValueTask()
        {
            _something.SayValueTaskAsync("text").ReturnsNullForAnyArgs();

            Assert.That(_something.SayValueTaskAsync("something").Result, Is.Null);
        }

        [Test]
        public void Returns_Null_for_any_args_when_class_returned_async_ValueTask()
        {
            _something.SomeActionWithParamsValueTaskAsync(2, "text").ReturnsNullForAnyArgs();

            Assert.That(_something.SomeActionWithParamsValueTaskAsync(123, "something else").Result, Is.Null);
        }
#endif

        [Test]
        public void Return_a_wrapped_async_result()
        {
            _something.CountAsync().Returns(3);

            Assert.That(_something.CountAsync(), Is.TypeOf<Task<int>>());
            Assert.That(_something.CountAsync().Result, Is.EqualTo(3));
        }

#if !NET40
        [Test]
        public void Return_a_wrapped_ValueTask_async_result()
        {
            _something.CountValueTaskAsync().Returns(3);

            Assert.That(_something.CountValueTaskAsync(), Is.TypeOf<ValueTask<int>>());
            Assert.That(_something.CountValueTaskAsync().Result, Is.EqualTo(3));
        }
#endif

        [Test]
        public void Return_multiple_async_results_from_funcs()
        {
            _something.CountAsync().Returns(
                _ => 1,
                _ => 2,
                _ => 3);

            Assert.That(_something.CountAsync().Result, Is.EqualTo(1), "First return");
            Assert.That(_something.CountAsync().Result, Is.EqualTo(2), "Second return");
            Assert.That(_something.CountAsync().Result, Is.EqualTo(3), "Third return");
            Assert.That(_something.CountAsync().Result, Is.EqualTo(3), "Fourth return");
        }

#if !NET40
        [Test]
        public void Return_multiple_ValueTask_async_results_from_funcs()
        {
            _something.CountValueTaskAsync().Returns(
                _ => 1,
                _ => 2,
                _ => 3);

            Assert.That(_something.CountValueTaskAsync().Result, Is.EqualTo(1), "First return");
            Assert.That(_something.CountValueTaskAsync().Result, Is.EqualTo(2), "Second return");
            Assert.That(_something.CountValueTaskAsync().Result, Is.EqualTo(3), "Third return");
            Assert.That(_something.CountValueTaskAsync().Result, Is.EqualTo(3), "Fourth return");
        }
#endif

        [SetUp]
        public void SetUp()
        {
            _something = Substitute.For<ISomething>();
        }
    }
}
