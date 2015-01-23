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
        public void SubstituteWideConfigure()
        {
            var sub = Substitute.For<IInterface>();
            sub.Configure(call => call.GetReturnType() == typeof(int) ? RouteAction.Return(5) : RouteAction.Continue());

            Assert.AreEqual(5, sub.ReturnsInt());
        }

        [Test]
        [Pending]
        public void ExtensionBasedOnConfigure()
        {
            var sub = Substitute.For<IInterface>();
            sub.ReturnDefaultForAll(5);

            Assert.AreEqual(5, sub.ReturnsInt());
        }
    }

    public static class Extensions
    {
        public static TSubstitute ReturnDefaultForAll<TSubstitute, TReturnType>(this TSubstitute substitute, TReturnType defaultValue)
        {
            substitute.Configure(call => call.GetReturnType() == typeof(TReturnType) ? RouteAction.Return(defaultValue) : RouteAction.Continue());
            return substitute;
        }

    }
}