using System;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class PartialSubs
    {
        [Test]
        public void CanNotCreatePartialSubForInterface()
        {
            Assert.Throws<CanNotPartiallySubForInterfaceOrDelegateException>(() => Substitute.ForPartsOf<ITestInterface>());
        }

        [Test]
        public void CanNotCreatePartialSubForDelegate()
        {
            Assert.Throws<CanNotPartiallySubForInterfaceOrDelegateException>(() => Substitute.ForPartsOf<Action>());
        }

        [Test]
        public void CanNotCreatePartialSubForEventHander()
        {
            Assert.Throws<CanNotPartiallySubForInterfaceOrDelegateException>(() => Substitute.ForPartsOf<EventHandler>());
        }

        [Test]
        public void CallAbstractMethod()
        {
            var testAbstractClass = Substitute.ForPartsOf<TestAbstractClass>();
            testAbstractClass.VoidAbstractMethod();
            var result = testAbstractClass.AbstractMethodReturnsSameInt(123);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void SpyOnAbstractMethod()
        {
            var wasCalled = false;
            var testAbstractClass = Substitute.ForPartsOf<TestAbstractClass>();
            testAbstractClass.When(x => x.VoidAbstractMethod()).Do(x => wasCalled = true); ;
            testAbstractClass.VoidAbstractMethod();
            Assert.That(wasCalled);
        }

        [Test]
        public void ShouldCallBaseImplementationForVirtualMember()
        {
            var testClass = Substitute.ForPartsOf<TestClass>();
            testClass.VoidVirtualMethod();
            testClass.VoidVirtualMethod();
            testClass.VoidVirtualMethod();
            Assert.That(testClass.CalledTimes, Is.EqualTo(3));
        }

        [Test]
        public void StopCallingBaseImplementationForVirtualMember()
        {
            var testClass = Substitute.ForPartsOf<TestClass>();
            testClass.When(x => x.VoidVirtualMethod()).DoNotCallBase();
            testClass.VoidVirtualMethod();
            testClass.VoidVirtualMethod();
            Assert.That(testClass.CalledTimes, Is.EqualTo(0));
        }

        [Test]
        public void StopCallingBaseAndDoSomethingInstead()
        {
            var wasCalled = false;
            var testClass = Substitute.ForPartsOf<TestClass>();
            testClass.When(x => x.VoidAbstractMethod())
                .Do(x => wasCalled = true);
            testClass.When(x => x.VoidAbstractMethod())
                .DoNotCallBase();
            testClass.VoidAbstractMethod();
            Assert.That(testClass.CalledTimes, Is.EqualTo(0));
            Assert.That(wasCalled);
        }

        [Test]
        public void UseImplementedVirtualMethod()
        {
            var testAbstractClass = Substitute.ForPartsOf<TestAbstractClass>();
            Assert.That(testAbstractClass.MethodReturnsSameInt(1), Is.EqualTo(1));
            Assert.That(testAbstractClass.CalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void ReturnDefaultForUnimplementedAbstractMethod()
        {
            var testAbstractClass = Substitute.ForPartsOf<TestAbstractClass>();
            Assert.AreEqual(default(int), testAbstractClass.AbstractMethodReturnsSameInt(1));
        }

        [Test]
        public void OverrideBaseMethodsReturnValueStopsItFromCallingBase()
        {
            var testAbstractClass = Substitute.ForPartsOf<TestAbstractClass>();
            testAbstractClass.MethodReturnsSameInt(Arg.Any<int>()).Returns(x => 42);
            Assert.That(testAbstractClass.MethodReturnsSameInt(1), Is.EqualTo(42));
            Assert.That(testAbstractClass.CalledTimes, Is.EqualTo(0));
        }

        [Test]
        public void OverrideVirtualMethodReturnValueStopsItFromCallingBase()
        {
            var testClass = Substitute.ForPartsOf<TestClass>();
            testClass.VirtualMethodReturnsSameInt(Arg.Any<int>()).Returns(2);
            Assert.That(testClass.VirtualMethodReturnsSameInt(1), Is.EqualTo(2));
        }

        [Test]
        public void OverrideWithMultipleReturns()
        {
            var testClass = Substitute.ForPartsOf<TestClass>();
            testClass.VirtualMethodReturnsSameInt(Arg.Any<int>())
                        .Returns(x => 1, x => 2, x => 3);
            Assert.That(testClass.VirtualMethodReturnsSameInt(10), Is.EqualTo(1));
            Assert.That(testClass.VirtualMethodReturnsSameInt(10), Is.EqualTo(2));
            Assert.That(testClass.VirtualMethodReturnsSameInt(10), Is.EqualTo(3));
            Assert.That(testClass.VirtualMethodReturnsSameInt(10), Is.EqualTo(3));
            Assert.That(testClass.CalledTimes, Is.EqualTo(0));
        }

        [Test]
        public void DoNotCallBaseWhenConfiguringReturnsIfAnArgMatcherIsPresent()
        {
            var testClass = Substitute.ForPartsOf<TestClass>();
            testClass.MethodReturnsSameInt(Arg.Is(2)).Returns(4);

            var result = testClass.MethodReturnsSameInt(2);

            Assert.That(testClass.CalledTimes, Is.EqualTo(0), "should not call base as NSub guesses we are specifying a call");
            Assert.That(result, Is.EqualTo(4));
        }

        [Test]
        public void DoNotCallBaseWhenUsingWhenDoNotCallBase()
        {
            var testClass = Substitute.ForPartsOf<TestClass>();
            testClass.When(x => x.MethodReturnsSameInt(2)).DoNotCallBase();

            var result = testClass.MethodReturnsSameInt(2);

            Assert.That(testClass.CalledTimes, Is.EqualTo(0), "should not call base");
            Assert.That(result, Is.EqualTo(default(int)));
        
        }

        public interface ITestInterface
        {
            void VoidTestMethod();
            int TestMethodReturnsInt();
        }

        public abstract class TestAbstractClass
        {
            public int CalledTimes { get; set; }

            public abstract void VoidAbstractMethod();

            public abstract int AbstractMethodReturnsSameInt(int i);

            public virtual int MethodReturnsSameInt(int i)
            {
                CalledTimes++;
                return i;
            }
        }

        public class TestClass : TestAbstractClass
        {

            public virtual int VirtualMethodReturnsSameInt(int i)
            {
                CalledTimes++;
                return i;
            }

            public virtual object VirtualMethodReturnsSameObject(object o)
            {
                CalledTimes++;
                return o;
            }

            public virtual void VoidVirtualMethod()
            {
                CalledTimes++;
            }

            public override void VoidAbstractMethod()
            {
                CalledTimes++;
            }

            public override int AbstractMethodReturnsSameInt(int i)
            {
                CalledTimes++;
                return i;
            }
        }
    }
}