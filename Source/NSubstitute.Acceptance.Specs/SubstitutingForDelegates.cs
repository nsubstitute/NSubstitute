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
    }
}