using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class WhenCallBase
    {
        private TestClass _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = Substitute.For<TestClass>();
        }

        [Test]
        public void Check_that_base_is_called_when_virtual_method_is_called()
        {
            _testClass.When(c => c.VirtualMethod()).CallBase();
            Assert.IsFalse(_testClass.IsCalled);
            _testClass.VirtualMethod();
            Assert.IsTrue(_testClass.IsCalled);
        }

        [Test]
        public void Check_that_base_is_called_when_abstract_method_is_called()
        {
            _testClass.When(c => c.AbstractMethod()).CallBase();
            Assert.IsFalse(_testClass.IsCalled);
            _testClass.AbstractMethod();
            Assert.IsTrue(_testClass.IsCalled);
        }

        [Test]
        public void Check_that_base_is_called_when_regular_method_is_called()
        {
            _testClass.When(c => c.Method()).CallBase();
            Assert.IsTrue(_testClass.IsCalled);
            _testClass.Method();
            Assert.IsTrue(_testClass.IsCalled);
        }

        [Test]
        public void Check_that_base_is_called_when_virtual_method_is_called_with_satisfied_arg()
        {
            _testClass.When(c => c.VirtualMethodWithArgs(Arg.Is(1))).CallBase();
            Assert.IsFalse(_testClass.IsCalled);
            _testClass.VirtualMethodWithArgs(1);
            Assert.IsTrue(_testClass.IsCalled);
        }

        [Test]
        [Pending]
        public void Check_that_base_is_not_called_when_virtual_method_is_called_with_no_satisfied_arg()
        {
            _testClass.When(c => c.VirtualMethodWithArgs(Arg.Is(1))).CallBase();
            Assert.IsFalse(_testClass.IsCalled);
            _testClass.VirtualMethodWithArgs(2);
            Assert.IsFalse(_testClass.IsCalled);
        }

        public abstract class AbstractTestClass
        {
            public abstract void AbstractMethod();
        }

        public class TestClass : AbstractTestClass
        {
            public bool IsCalled { get; set; }

            public virtual void VirtualMethod()
            {
                IsCalled = true;
            }

            public void Method()
            {
                IsCalled = true;
            }

            public virtual void VirtualMethodWithArgs(int i)
            {
                IsCalled = true;
            }

            public override void AbstractMethod()
            {
                IsCalled = true;
            }
        }
    }
}