using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class ClearSubstitute
    {
        public interface ICalculator
        {
            int Add(int a, int b);
        }

        [Test]
        public void Can_clear_received_calls_on_a_substitute()
        {
            var substitute = Substitute.For<ICalculator>();
            substitute.Add(1, 1);
            substitute.Add(2, 2);
            substitute.Received().Add(1, 1);
            substitute.ClearSubstitutions(ClearOptions.ReceivedCalls);
            Assert.Throws<ReceivedCallsException>(() => substitute.Received().Add(1, 1));
        }

        [Test]
        public void Clear_results()
        {
            var substitute = Substitute.For<ICalculator>();
            substitute.Add(1, 1).Returns(12);
            substitute.ClearSubstitutions(ClearOptions.ReturnValues);
            Assert.AreEqual(0, substitute.Add(1, 1));
        }

        [Test]
        public void Clear_callbacks()
        {
            var count = 0;
            var substitute = Substitute.For<ICalculator>();
            substitute.When(x => x.Add(1, 1)).Do(x => count++);
            substitute.ClearSubstitutions(ClearOptions.CallActions);
            substitute.Add(1, 1);
            substitute.Add(1, 1);
            substitute.Add(1, 1);
            Assert.AreEqual(0, count);
        }
    }
}