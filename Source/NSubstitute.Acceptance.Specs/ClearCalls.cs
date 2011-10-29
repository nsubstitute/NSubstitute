using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class ClearCalls
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
            substitute.ClearReceivedCalls();
            Assert.Throws<ReceivedCallsException>(() => substitute.Received().Add(1, 1));
        }
    }
}