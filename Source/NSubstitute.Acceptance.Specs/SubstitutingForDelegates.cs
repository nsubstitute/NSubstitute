using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class SubstitutingForDelegates
    {
        [Test]
        [Pending]
        public void SubForAction()
        {
            var action = Substitute.For<Action>();
            action();
            action.Received()();
        }
        
        [Test]
        [Pending]
        public void SubForFunc()
        {
            var func = Substitute.For<Func<int, string>>();
            func(1).Returns("1");
            
            Assert.That(func(1), Is.EqualTo("1"));
            func.Received()(1);
        }
    }
}