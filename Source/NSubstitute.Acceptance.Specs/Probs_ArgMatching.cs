using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class Probs_ArgMatching
    {
        public interface IFoo
        {
            string Bar(int a);
            string Zee<T>(Expression<Func<T>> a);
        }

        [Test]
        public void TestBar()
        {
            var sub = Substitute.For<IFoo>();
            sub.Bar(Arg.Is<int>(x => x == 1)).Returns("1");
            sub.Bar(Arg.Is<int>(x => x == 2)).Returns("2");


            Assert.AreEqual(sub.Bar(2), "2");
            Assert.AreEqual(sub.Bar(1), "1");
            Assert.AreEqual(sub.Bar(0), "");
        }

        [Test]
        public void TestZee()
        {
            var sub = Substitute.For<IFoo>();
            sub.Zee(Arg.Is<Expression<Func<int>>>(x => ExpressionValueMatches(x, 1))).Returns("1");
            sub.Zee(Arg.Is<Expression<Func<int>>>(x => ExpressionValueMatches(x, 2))).Returns("2");

            Assert.AreEqual(sub.Zee(() => 2), "2");
            Assert.AreEqual(sub.Zee(() => 1), "1");
            Assert.AreEqual(sub.Zee(() => 0), "");
        }

        private bool ExpressionValueMatches<T>(Expression<Func<T>> x, T y)
        {
            return x != null && x.Compile()().Equals(y);
        }
    }
}