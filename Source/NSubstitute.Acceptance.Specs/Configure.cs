using NSubstitute.Core;

using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class Configure
    {
        public interface IInterface
        {
            int ReturnsInt();
        }

        [Test]
        [Pending]
        public void CanNotCreatePartialSubForInterface()
        {
            Substitute.Configure(call => call.GetReturnType() == typeof(int) ? RouteAction.Return(5) : RouteAction.Continue());

            var sub = Substitute.For<IInterface>();

            Assert.AreEqual(5, sub.ReturnsInt());
        }
    }
}