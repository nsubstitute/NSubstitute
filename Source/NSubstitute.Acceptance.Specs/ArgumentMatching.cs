using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class ArgumentMatching
    {
        private ISomething _something;

        [Test]
        public void Return_result_for_any_argument()
        {
            _something.Echo(Arg.Any<int>()).Returns("anything");

            Assert.That(_something.Echo(1), Is.EqualTo("anything"), "First return");
            Assert.That(_something.Echo(2), Is.EqualTo("anything"), "Second return");
        }

        [Test]
        public void Return_result_for_specific_argument()
        {
            _something.Echo(Arg.Is(3)).Returns("three");
            _something.Echo(4).Returns("four");

            Assert.That(_something.Echo(3), Is.EqualTo("three"), "First return");
            Assert.That(_something.Echo(4), Is.EqualTo("four"), "Second return");
        }

        [Test]
        public void Return_result_for_argument_matching_predicate()
        {
            _something.Echo(Arg.Is<int>(x => x <= 3)).Returns("small");
            _something.Echo(Arg.Is<int>(x => x > 3)).Returns("big");

            Assert.That(_something.Echo(1), Is.EqualTo("small"), "First return");
            Assert.That(_something.Echo(4), Is.EqualTo("big"), "Second return");
        }

        [Test]
        public void Should_not_match_when_arg_matcher_throws()
        {
            _something.Say(Arg.Is<string>(x => x.Length < 2)).Returns("?");

            Assert.That(_something.Say("e"), Is.EqualTo("?"));
            Assert.That(_something.Say("eh"), Is.EqualTo(string.Empty));
            Assert.That(_something.Say(null), Is.EqualTo(string.Empty));
        }

        [Test]
        public void Return_result_with_only_one_matcher_for_that_type()
        {
            _something.Funky(Arg.Any<float>(), 12, "Lots", null).Returns(42);

            Assert.That(_something.Funky(123.456f, 12, "Lots", null), Is.EqualTo(42));
            Assert.That(_something.Funky(0.0f, 12, "Lots", null), Is.EqualTo(42));
            Assert.That(_something.Funky(0.0f, 11, "Lots", null), Is.EqualTo(0));
        }

        [Test]
        public void Received_for_any_argument()
        {
            _something.Echo(7);

            _something.Received().Echo(Arg.Any<int>());
        }

        [Test]
        public void Received_for_specific_argument()
        {
            _something.Echo(3);

            _something.Received().Echo(Arg.Is(3));
        }

        [Test]
        public void Received_for_argument_matching_predicate()
        {
            _something.Echo(7);

            _something.Received().Echo(Arg.Is<int>(x => x > 3));
        }

        [Test]
        public void Received_for_only_one_matcher_for_that_type()
        {
            _something.Funky(123.456f, 12, "Lots", null);

            _something.Received().Funky(Arg.Any<float>(), 12, "Lots", null);
        }

#if (NET45 || NETSTANDARD1_5)
        [Test]
        public void Received_for_async_method_can_be_awaited()
        {
            TestReceivedAsync().Wait();
        }

        private async System.Threading.Tasks.Task TestReceivedAsync()
        {
            await _something.Async();
            await _something.Received().Async();
        }

        [Test]
        public void DidNotReceive_for_async_method_can_be_awaited()
        {
            TestDidNotReceiveAsync().Wait();
        }

        private async System.Threading.Tasks.Task TestDidNotReceiveAsync()
        {
            await _something.DidNotReceive().Async();
        }
#endif

        [Test]
        public void Resolve_potentially_ambiguous_matches_by_checking_for_non_default_argument_values()
        {
            _something.Add(10, Arg.Any<int>()).Returns(1);

            Assert.That(_something.Add(10, 5), Is.EqualTo(1));
        }

        [Test]
        public void Received_should_compare_elements_for_params_arguments()
        {
            const string first = "first";
            const string second = "second";
            _something.WithParams(1, first, second);

            _something.Received().WithParams(1, first, second);
            _something.Received().WithParams(1, Arg.Any<string>(), second);
            _something.Received().WithParams(1, first, Arg.Any<string>());
            _something.Received().WithParams(1, new[] { first, second });
            _something.Received().WithParams(1, Arg.Any<string[]>());
            _something.Received().WithParams(1, Arg.Is<string[]>(x => x.Length == 2));
            _something.DidNotReceive().WithParams(2, first, second);
            _something.DidNotReceive().WithParams(2, first, Arg.Any<string>());
            _something.DidNotReceive().WithParams(1, first, first);
            _something.DidNotReceive().WithParams(1, null);
            _something.DidNotReceive().WithParams(1, Arg.Is<string[]>(x => x.Length > 3));
        }

        [Test]
        public void Throw_with_ambiguous_arguments_when_given_an_arg_matcher_and_a_default_arg_value()
        {
            Assert.Throws<AmbiguousArgumentsException>(() =>
               {
                   _something.Add(0, Arg.Any<int>()).Returns(1);
                   //Should not make it here, as it can't work out which arg the matcher refers to.
                   //If it does this will throw an AssertionException rather than AmbiguousArgumentsException.
                   Assert.That(_something.Add(0, 5), Is.EqualTo(1));
               }
                );
        }

        [Test]
        public void Returns_should_work_with_params()
        {
            _something.WithParams(Arg.Any<int>(), Arg.Is<string>(x => x == "one")).Returns("fred");

            Assert.That(_something.WithParams(1, "one"), Is.EqualTo("fred"));
        }

        [Test]
        public void Resolve_setter_arg_matcher_with_more_specific_type_than_member_signature()
        {
            const string value = "some string";
            const string key = "key";

            _something[key] = value;

            _something.Received()[key] = Arg.Is(value);
        }

        [Test]
        public void Resolve_argument_matcher_for_more_specific_type()
        {
            _something.Anything("Hello");
            _something.Received().Anything(Arg.Any<string>());
            _something.DidNotReceive().Anything(Arg.Any<int>());
        }

        [Test]
        public void Set_returns_using_more_specific_type_matcher()
        {
            _something.Anything(Arg.Is<string>(x => x.Contains("world"))).Returns(123);

            Assert.That(_something.Anything("Hello world!"), Is.EqualTo(123));
            Assert.That(_something.Anything("Howdy"), Is.EqualTo(0));
            Assert.That(_something.Anything(2), Is.EqualTo(0));
        }

        [Test]
        public void Override_subclass_arg_matcher_when_returning_for_any_args()
        {
            _something.Anything(Arg.Any<string>()).ReturnsForAnyArgs(123);

            Assert.That(_something.Anything(new object()), Is.EqualTo(123));
        }

        [Test]
        public void Nullable_args_null_value()
        {
            _something.WithNullableArg(Arg.Any<int?>()).ReturnsForAnyArgs(123);

            Assert.That(_something.WithNullableArg(null), Is.EqualTo(123));
        }

        [Test]
        public void Nullable_args_notnull_value()
        {
            _something.WithNullableArg(Arg.Any<int?>()).ReturnsForAnyArgs(123);

            Assert.That(_something.WithNullableArg(234), Is.EqualTo(123));
        }

        [SetUp]
        public void SetUp()
        {
            _something = Substitute.For<ISomething>();
        }
    }
}