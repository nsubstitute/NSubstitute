using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class ParameterMatching
    {
        private ISomething _something;

        [Test]
        public void Return_result_for_any_parameter()
        {
            _something.Echo(Arg.Any<int>()).Returns("anything");

            Assert.That(_something.Echo(1), Is.EqualTo("anything"), "First return");
            Assert.That(_something.Echo(2), Is.EqualTo("anything"), "Second return");
        }

        [Test]
        public void Return_result_for_specific_parameter()
        {
            _something.Echo(Arg.Is(3)).Returns("three");
            _something.Echo(4).Returns("four");

            Assert.That(_something.Echo(3), Is.EqualTo("three"), "First return");
            Assert.That(_something.Echo(4), Is.EqualTo("four"), "Second return");
        }

        [Test]
        public void Return_result_for_parameter_matching_predicate()
        {
            _something.Echo(Arg.Is<int>(x => x <= 3)).Returns("small");
            _something.Echo(Arg.Is<int>(x => x > 3)).Returns("big");

            Assert.That(_something.Echo(1), Is.EqualTo("small"), "First return");
            Assert.That(_something.Echo(4), Is.EqualTo("big"), "Second return");
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
        public void Received_for_any_parameter()
        {
            _something.Echo(7);

            _something.Received().Echo(Arg.Any<int>());
        }

        [Test]
        public void Received_for_specific_parameter()
        {
            _something.Echo(3);
            
            _something.Received().Echo(Arg.Is(3));
        }

        [Test]
        public void Received_for_parameter_matching_predicate()
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
            _something.Received().WithParams(1, new[] {first, second});
            _something.Received().WithParams(1, Arg.Any<string[]>());
            _something.Received().WithParams(1, Arg.Is<string[]>(x => x.Length == 2));
            _something.DidNotReceive().WithParams(2, first, second);
            _something.DidNotReceive().WithParams(2, first, Arg.Any<string>());
            _something.DidNotReceive().WithParams(1, first, first);
            _something.DidNotReceive().WithParams(1, null);
            _something.DidNotReceive().WithParams(1, Arg.Is<string[]>(x => x.Length > 3));
        }

        [Test]
        [Pending]
        public void Resolve_setter_arg_matcher_with_more_specific_type_than_member_signature()
        {
            const string value = "some string";
            const string key = "key";

            _something[key] = value;

            _something.Received()[key] = Arg.Is(value);
        }

        [SetUp]
        public void SetUp()
        {
            _something = Substitute.For<ISomething>();
        }
    }
}