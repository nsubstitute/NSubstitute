using System;
#if (NET4 || NET45 || NETSTANDARD1_5)
using System.Threading.Tasks;
#endif
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

#if (NET4 || NET45 || NETSTANDARD1_5)
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

        [Test]
        public void Return_multiple_results_from_funcs_for_any_arguments_async()
        {
            _something.EchoAsync(1).ReturnsForAnyArgs(_ => "first", _ => "second");

            Assert.That(_something.EchoAsync(2).Result, Is.EqualTo("first"));
            Assert.That(_something.EchoAsync(724).Result, Is.EqualTo("second"));
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
            Assert.That(exception.Message, Is.StringContaining(expectedMessagePrefix));
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

#if NET45 || NET4 || NETSTANDARD1_5
        [Test]
        public void Return_a_wrapped_async_result()
        {
            _something.CountAsync().Returns(3);

            Assert.That(_something.CountAsync(), Is.TypeOf<Task<int>>());
            Assert.That(_something.CountAsync().Result, Is.EqualTo(3));
        }

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
#endif

        [SetUp]
        public void SetUp()
        {
            _something = Substitute.For<ISomething>();
        }
    }
}
