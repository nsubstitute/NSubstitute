using System.Collections.Generic;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class ArgCaptureFromMatcher
    {
        private readonly object _someObject = new object();
        private IFoo _sub;

        public interface IFoo
        {
            void Bar(string a, int b, object c);
            int Zap(object c);
        }

        [SetUp]
        public void SetUp()
        {
            _sub = Substitute.For<IFoo>();
        }

        [Test]
        public void Should_capture_argument_as_directed_and_if_pain_persists_see_your_coder()
        {
            string stringArg = null;
            _sub.Bar(Arg.Capture<string>(x => stringArg = x), 1, _someObject);

            _sub.Bar("hello world", 1, _someObject);

            Assert.That(stringArg, Is.EqualTo("hello world"));
        }

        [Test]
        public void Setting_an_argument_capture_should_not_count_as_a_call()
        {
            string stringArg = null;
            _sub.Bar(Arg.Capture<string>(x => stringArg = x), 1, _someObject);

            _sub.DidNotReceiveWithAnyArgs().Bar(null, 0, null);
            Assert.That(stringArg, Is.Null);
        }

        [Test]
        public void Should_call_action_with_each_matching_call()
        {
            var stringArgs = new List<string>();
            _sub.Bar(Arg.Capture<string>(x => stringArgs.Add(x)), 1, _someObject);

            _sub.Bar("hello", 1, _someObject);
            _sub.Bar("world", 1, _someObject);
            _sub.Bar("don't use this because call doesn't match", -123, _someObject);

            Assert.That(stringArgs, Is.EqualTo(new[] { "hello", "world" }));
        }

        [Test]
        public void Capture_args_with_when_for_any_args()
        {
            string stringArg = null;
            _sub.WhenForAnyArgs(x => x.Bar(Arg.Capture<string>(arg => stringArg = arg), 1, _someObject)).Do(x => { });

            _sub.Bar("hello world", 42, null);

            Assert.That(stringArg, Is.EqualTo("hello world"));
        }

        [Test]
        public void Capture_args_when_action_requires_more_specific_type_should_only_run_action_when_arg_is_of_compatible_type()
        {
            string stringArg = null;
            _sub.Zap(Arg.Capture<string>(arg => stringArg = arg));

            _sub.Zap("hello world");
            _sub.Zap(new object());

            Assert.That(stringArg, Is.EqualTo("hello world"));
        }

        [Test]
        public void Capture_args_with_returns_for_any_args()
        {
            var stringArgLength = 0;
            _sub.Zap(Arg.Capture<string>(arg => stringArgLength = arg.Length)).ReturnsForAnyArgs(1);

            _sub.Zap(new object());
            Assert.That(stringArgLength, Is.EqualTo(0));

            _sub.Zap("hello");
            Assert.That(stringArgLength, Is.EqualTo("hello".Length));
        }

        [Test]
        public void Override_capture_subclass_with_returns_for_any_args()
        {
            var stringArgLength = 0;
            _sub.Zap(Arg.Capture<string>(arg => stringArgLength = arg.Length)).ReturnsForAnyArgs(1);

            var result = _sub.Zap(new object());
            Assert.That(stringArgLength, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(1));
        }
    }
}