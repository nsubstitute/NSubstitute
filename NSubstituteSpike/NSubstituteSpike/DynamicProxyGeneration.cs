using System;
using NUnit.Framework;

namespace NSubstituteSpike
{
    public class LinFuProxy : DynamicProxyGeneration
    {
        protected override ISubstitutionFactory CreateFactory()
        {
            return new LinFuSubstitutionFactory();
        }
    }
    public class CastleProxy : DynamicProxyGeneration
    {
        protected override ISubstitutionFactory CreateFactory()
        {
            return new CastleSubstitutionFactory();
        }
    }

    [TestFixture]
    public abstract class DynamicProxyGeneration
    {
        private ISubstitutionFactory _factory;
        private IFoo _subFoo;

        [SetUp]
        public void SetUp()
        {
            _factory = CreateFactory();
            _subFoo = _factory.Create<IFoo>();
        }

        protected abstract ISubstitutionFactory CreateFactory();

        [Test]
        public void NoArgMethodCallWithSimpleReturn()
        {
            Assert.That(_subFoo.Calculate(), Is.EqualTo(0));
        }

        [Test]
        public void MethodCallWithArgsAndSimpleReturn()
        {
            Assert.That(_subFoo.Concat("a", "b"), Is.EqualTo(null));
        }

        [Test]
        public void NoArgMethodCallWithCustomObjectReturned()
        {
            Bar bar = _subFoo.CreateBar();
            Assert.That(bar, Is.EqualTo(null));
        }

        [Test]
        public void GetProperty()
        {
            Guid id = _subFoo.SomeId;
            Assert.That(id, Is.EqualTo(Guid.Empty));
        }

        [Test]
        public void SetProperty()
        {
            _subFoo.SomeId = Guid.NewGuid();
            Assert.That(_subFoo.SomeId, Is.EqualTo(Guid.Empty));
        }
    }    
}