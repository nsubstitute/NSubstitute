using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NSubstitute.Acceptance.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class ReceivedCallsQueryModel
    {
        private IEngine _engine;
        private Foo _foo;
        const int _rpm = 7000;

        [SetUp]
        public void SetUp()
        {
            _engine = Substitute.For<IEngine>();
            _foo = Substitute.For<Foo>();
        }

        private IEnumerable<string> GetReceivedCallMethodNames(Expression<Action<IEngine>> call)
        {
            IEnumerable<string> methodCallNames = _engine.ReceivedCalls(call).Select(c => c.GetMethodInfo().Name);
            
            return methodCallNames;
        }

        [Test]
        public void Query_returns_the_call()
        {
            _engine.Rev();

            IEnumerable<string> methodCallNames = GetReceivedCallMethodNames(x => x.Rev());

            Assert.That(methodCallNames.Count(), Is.EqualTo(1));
            Assert.That(methodCallNames.Single(), Is.EqualTo("Rev"));
        }

        [Test]
        public void Query_returns_the_call_with_a_return_parameter()
        {
            _engine.GetCapacityInLitres();

            IEnumerable<string> methodCallNames = GetReceivedCallMethodNames(x => x.GetCapacityInLitres());

            Assert.That(methodCallNames.Count(), Is.EqualTo(1));
            Assert.That(methodCallNames.Single(), Is.EqualTo("GetCapacityInLitres"));
        }

        [Test]
        public void Query_returns_an_empty_list_when_no_calls_are_found()
        {
            _engine.Rev();

            IEnumerable<string> methodCallNames = GetReceivedCallMethodNames(x => x.Idle());

            Assert.That(methodCallNames.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Query_returns_one_matching_call_only()
        {
            _engine.Rev();
            _engine.Idle();

            IEnumerable<string> methodCallNames = GetReceivedCallMethodNames(x => x.Rev());

            Assert.That(methodCallNames.Count(), Is.EqualTo(1));
            Assert.That(methodCallNames.Single(), Is.EqualTo("Rev"));
        }

        [Test]
        public void Query_returns_one_call_with_matching_argument()
        {
            _engine.RevAt(_rpm);
            _engine.RevAt(_rpm + 100);

            IEnumerable<string> methodCallNames = GetReceivedCallMethodNames(x => x.RevAt(_rpm));

            Assert.That(methodCallNames.Count(), Is.EqualTo(1));
            Assert.That(methodCallNames.Single(), Is.EqualTo("RevAt"));
        }

        [Test]
        public void Query_returns_call_with_matching_argument_from_a_method_call()
        {
            _engine.RevAt(_rpm);

            IEnumerable<string> methodCallNames = GetReceivedCallMethodNames(x => x.RevAt(GetRpm()));

            Assert.That(methodCallNames.Count(), Is.EqualTo(1));
            Assert.That(methodCallNames.Single(), Is.EqualTo("RevAt"));
        }

        private int GetRpm()
        {
            return _rpm;
        }

        [Test]
        public void Query_returns_one_call_with_any_arguments()
        {
            _engine.Rev();
            _engine.RevAt(_rpm);
            _engine.RevAt(_rpm + 100);

            IEnumerable<string> methodCallNames = GetReceivedCallMethodNames(x => x.RevAt(Arg.Any<int>()));

            Assert.That(methodCallNames.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Query_returns_one_call_with_specific_arguments()
        {
            _engine.Rev();
            _engine.RevAt(_rpm);
            _engine.RevAt(_rpm + 100);

            IEnumerable<string> methodCallNames = GetReceivedCallMethodNames(x => x.RevAt(Arg.Is<int>(m => m >= _rpm)));

            Assert.That(methodCallNames.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Query_with_constant_argument_and_argument_spec()
        {
            _foo.Bar(1, 2);

            var calls = _foo.ReceivedCalls(x => x.Bar(Arg.Any<int>(), 2));

            Assert.That(calls.Count(), Is.EqualTo(1));
        }

        public interface Foo
        {
            void Bar(int a, int b);
        }
    }
}
