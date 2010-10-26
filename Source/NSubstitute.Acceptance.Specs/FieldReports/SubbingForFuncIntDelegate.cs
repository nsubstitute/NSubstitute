using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class SubbingForFuncIntDelegate
    {
        [Test][Pending]
        public void Should_not_given_weird_nullref_exceptions()
        {
            var func = Substitute.For<Func<int>>();
            func().Returns(10);
            Assert.AreEqual(10, func());
        }
    }
}