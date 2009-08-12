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

    public interface IFoo
    {
        int Calculate();
        string Concat(string a, string b);
        Bar CreateBar();
        Guid SomeId { get; set; }

    }
    public class Bar {}
}
