using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class SubstitutePartial
    {
        //todo remove crooked tests, add exact Substitute.Partial tests, more tests

        [Test]
        public void SubstitutePartialIntegrationTest()
        {
            var testClass = Substitute.Partial<TestClass>();
            testClass.VirtualMethod();
            Assert.That(testClass.CalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void SubstitutePartialIntegrationTest2()
        {
            var testClass = Substitute.Partial<TestClass>();
            Assert.That(testClass.VirtualMethod(), Is.EqualTo(1));
            Assert.That(testClass.CalledTimes, Is.EqualTo(1));
            testClass.VirtualMethod().Returns(5);
            Assert.That(testClass.CalledTimes, Is.EqualTo(2)); //we can not avoid base call above, can we?
            Assert.That(testClass.VirtualMethod(), Is.EqualTo(5));
            Assert.That(testClass.CalledTimes, Is.EqualTo(2)); 
        }

        public abstract class AbstractTestClass
        {
            public abstract void AbstractMethod();
        }

        public class TestClass : AbstractTestClass
        {
            public int CalledTimes { get; set; }

            public virtual int VirtualMethod()
            {
                CalledTimes++;
                return 1;
            }

            public void Method()
            {
                CalledTimes++;
            }

            public virtual void VirtualMethodWithArgs(int i)
            {
                CalledTimes++;
            }

            public override void AbstractMethod()
            {
                CalledTimes++;
            }
        }

    }
}
