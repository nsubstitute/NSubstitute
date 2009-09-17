using NUnit.Framework;

namespace NSubstituteSpike
{
    [TestFixture]
    public class ProxyWithNonVirtualMethodsFixture
    {
        private Nerf _subNerf;

        [SetUp]
        public void SetUp()
        {
            var factory = new LinFuSubstitutionFactory();
            _subNerf = factory.Create<Nerf>();
        }

        [Test]
        public void TestNonVirtMethod()
        {
            Assert.That(_subNerf.Glurp(2), Is.EqualTo(null));
        }
    }

    public class Nerf
    {
        public string Glurp(int i)
        {
            return "Glurp " + i;
        }
    }

}