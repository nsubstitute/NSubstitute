using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class ClearCalls
    {
        [Test]
        public void Can_clear_received_calls_on_a_substitute()
        {
            var substitute = Substitute.For<ICalculator>();
            substitute.Add(1, 1);
            substitute.Add(2, 2);
            substitute.Received().Add(1, 1);
            substitute.ClearReceivedCalls();
            Assert.Throws<CallNotReceivedException>(() => substitute.Received().Add(1, 1));
        } 
    }
}