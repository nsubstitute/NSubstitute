using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class Issue38_SettingNullReturnValue
    {
        public interface IDoSomething
        {
            object Something();
        }

        [Test]
        public void CanSetCallToReturnNull()
        {
            var doSomething = Substitute.For<IDoSomething>();
            doSomething.Something().Returns(null);
            var result = doSomething.Something();
            Assert.That(result, Is.Null);
        }
    }
}