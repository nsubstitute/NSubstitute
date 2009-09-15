using System;
using System.Text;
using NUnit.Framework;

namespace NSubstituteSpike
{
    [TestFixture]
    public class Spike
    {
        [Test]
        public void NoStubbedReturn()
        {
            var subFoo = Substitute.For<IFoo>();
            Assert.That(subFoo.Calculate(), Is.EqualTo(default(int)));
        }

        [Test]
        public void StubReturn()
        {
            var subFoo = Substitute.For<IFoo>();
            subFoo.Calculate().Return(5);
            Assert.That(subFoo.Calculate(), Is.EqualTo(5));
        }

        [Test]
        public void StubMultipleReturns()
        {
            var subFoo = Substitute.For<IFoo>();
            subFoo.Calculate().Return(10);
            subFoo.Concat("a", "b").Return("hello");

            Assert.That(subFoo.Calculate(), Is.EqualTo(10));
            Assert.That(subFoo.Concat("a", "b"), Is.EqualTo("hello"));
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

    public interface IFoo
    {
        int Calculate();
        string Concat(string a, string b);
        Bar CreateBar();
        Guid SomeId { get; set; }

    }
    public class Bar {}
}
