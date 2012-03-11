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
        const int _rpm = 7000;

        [SetUp]
        public void SetUp()
        {
            _engine = Substitute.For<IEngine>();
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
        public void Query_returns_one_call_with_any_arguments()
        {
            _engine.Rev();
            _engine.RevAt(_rpm);
            _engine.RevAt(_rpm + 100);

            IEnumerable<string> methodCallNames = GetReceivedCallMethodNames(x => x.RevAt(Arg.Any<int>()));

            Assert.That(methodCallNames.Count(), Is.EqualTo(2));
        }
    }
}
