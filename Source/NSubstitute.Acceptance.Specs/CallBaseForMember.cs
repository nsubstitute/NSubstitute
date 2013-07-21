using System;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    // todo move tests to proper place, add more?
    public class CallBaseForMember
    {
        public class WhenCalledDoCallBase
        {
            [Test]
            public void Given_SubstituteForInterface_When_MethodIsCalled_Then_ShouldThrowException()
            {
                var testInterface = Substitute.For<ITestInterface>();
                testInterface.When(x => x.VoidTestMethod()).Do(x => x.CallBase());
                Assert.Throws<CouldNotCallBaseException>(testInterface.VoidTestMethod);
            }

            [Test]
            public void Given_SubstituteForAbstractClass_When_AbstractMethodIsCalled_Then_ShouldThrowException()
            {
                var testAbstractClass = Substitute.For<TestAbstractClass>();
                testAbstractClass.When(x => x.VoidAbstractMethod()).Do(x => x.CallBase());
                Assert.Throws<CouldNotCallBaseException>(testAbstractClass.VoidAbstractMethod);
            }

            [Test]
            public void Given_SubstituteForAction_When_ActionIsCalled_Then_ShouldThrowException()
            {
                var action = Substitute.For<Action>();
                action.When(x => x.Invoke()).Do(x => x.CallBase());
                Assert.Throws<CouldNotCallBaseException>(() => action());
            }

            [Test]
            public void Given_SubstituteForFunc_When_FuncIsCalled_Then_ShouldThrowException()
            {
                var func = Substitute.For<Func<int>>();
                func.When(x => x.Invoke()).Do(x => x.CallBase());
                Assert.Throws<CouldNotCallBaseException>(() => func());
            }

            [Test]
            public void Given_SubstituteForEventHandler_When_EventHandlerIsCalled_Then_ShouldThrowException()
            {
                var eventHandler = Substitute.For<EventHandler>();
                eventHandler.When(x => x.Invoke(Arg.Any<object>(), Arg.Any<EventArgs>())).Do(x => x.CallBase());
                Assert.Throws<CouldNotCallBaseException>(() => eventHandler(null, null));
            }

            [Test]
            public void Given_SubstituteForClass_When_VirtualMethodIsCalled_Then_ShouldCallBaseImplementation()
            {
                var testClass = Substitute.For<TestClass>();
                testClass.When(x => x.VoidVirtualMethod()).Do(x => x.CallBase());
                testClass.VoidVirtualMethod();
                Assert.That(testClass.CalledTimes, Is.EqualTo(1));
            }

            [Test]
            public void Given_SubstituteForClass_When_VirtualMethodIsSetup_ShouldNotCallRealImplementation()
            {
                var testClass = Substitute.For<TestClass>();
                testClass.When(x => x.VoidVirtualMethod()).Do(x => x.CallBase());
                testClass.When(x => x.VoidVirtualMethod()).Do(x => x.CallBase());

                Assert.That(testClass.CalledTimes, Is.EqualTo(0));
            }

            [Test]
            public void Given_SubstituteForClass_When_VirtualMethodIsSetupTwice_Then_ShouldSetupCorrectly()
            {
                var testClass = Substitute.For<TestClass>();
                testClass.When(x => x.VoidVirtualMethod()).Do(x => x.CallBase());
                testClass.When(x => x.VoidVirtualMethod()).Do(x => x.CallBase());

                testClass.VoidVirtualMethod();
                // I doubt that we should call base implementation just once here
                // as .When().Do() syntax purpose is to invoke actions and not Return/Callbase
                // and just by accident we have two actions that call base implemenation
                Assert.That(testClass.CalledTimes, Is.EqualTo(2));
            }

            [Test]
            public void Given_SubstituteForClass_When_AbstractMethodIsCalled_Then_ShouldCallBaseImplementation()
            {
                var testClass = Substitute.For<TestClass>();
                testClass.When(x => x.VoidAbstractMethod()).Do(x => x.CallBase());
                testClass.VoidAbstractMethod();
                Assert.That(testClass.CalledTimes, Is.EqualTo(1));
            }

            [Test]
            public void Given_SubstituteForClass_When_VirtualMethodIsCalledTwice_Then_ShouldCallBaseImplementationTwice()
            {
                var testClass = Substitute.For<TestClass>();
                testClass.When(x => x.VoidVirtualMethod()).Do(x => x.CallBase());
                testClass.VoidVirtualMethod();
                testClass.VoidVirtualMethod();
                Assert.That(testClass.CalledTimes, Is.EqualTo(2));
            }

            [Test]
            public void Given_SubstituteForAbstractClass_When_MethodWithImplementationIsCalled_Then_ShouldCallBaseImplementation()
            {
                var testAbstractClass = Substitute.For<TestAbstractClass>();
                testAbstractClass.When(x => x.MethodReturnsSameInt(Arg.Any<int>())).Do(x => x.CallBase());
                // .When().Do()  syntax does not return value from base implementation
                Assert.That(testAbstractClass.MethodReturnsSameInt(1), Is.EqualTo(default(int)));
                // but it calls base implementation
                Assert.That(testAbstractClass.CalledTimes, Is.EqualTo(1));
            }
        }

        public class ReturnsCallBase
        {
            [Test]
            public void Given_SubstituteForInterface_When_MethodIsCalled_Then_ShouldThrowException()
            {
                var testInterface = Substitute.For<ITestInterface>();
                testInterface.TestMethodReturnsInt().Returns(x => x.CallBase());
                Assert.Throws<CouldNotCallBaseException>(() => testInterface.TestMethodReturnsInt());
            }

            [Test]
            public void Given_SubstituteForAbstractClass_When_AbstractMethodIsCalled_Then_ShouldThrowException()
            {
                var testAbstractClass = Substitute.For<TestAbstractClass>();
                testAbstractClass.AbstractMethodReturnsSameInt(Arg.Any<int>()).Returns(x => x.CallBase());
                Assert.Throws<CouldNotCallBaseException>(() => testAbstractClass.AbstractMethodReturnsSameInt(1));
            }

            [Test]
            public void Given_SubstituteForFunc_When_FuncIsCalled_Then_ShouldThrowException()
            {
                var func = Substitute.For<Func<int>>();
                func().Returns(x => x.CallBase());
                Assert.Throws<CouldNotCallBaseException>(() => func());
            }

            [Test]
            public void Given_SubstituteForAbstractClass_When_MethodWithImplementationIsCalled_Then_ShouldCallBaseImplementation()
            {
                var testAbstractClass = Substitute.For<TestAbstractClass>();
                testAbstractClass.MethodReturnsSameInt(Arg.Any<int>()).Returns(x => x.CallBase());
                Assert.That(testAbstractClass.MethodReturnsSameInt(1), Is.EqualTo(1));
                Assert.That(testAbstractClass.CalledTimes, Is.EqualTo(1));
            }

            [Test]
            [Ignore("Should/can we avoid call base here?")]
            public void
                Given_SubstituteForClass_When_VirtualMethodsReturnValueIsOverwritten_Then_ShouldNotCallBaseImplementation
                ()
            {
                var testClass = Substitute.For<TestClass>();
                testClass.VirtualMethodReturnsSameInt(Arg.Any<int>()).Returns(x => x.CallBase());
                testClass.VirtualMethodReturnsSameInt(Arg.Any<int>()).Returns(1);
                Assert.That(testClass.CalledTimes, Is.EqualTo(0));
            }

            [Test]
            public void
                Given_SubstituteForClass_And_ReturnValueIsOverwritten_When_VirtualMethodIsCalled_Then_ShouldReturnOverwrittenValue
                ()
            {
                var testClass = Substitute.For<TestClass>();
                testClass.VirtualMethodReturnsSameInt(Arg.Any<int>()).Returns(x => x.CallBase());
                testClass.VirtualMethodReturnsSameInt(Arg.Any<int>()).Returns(2);
                Assert.That(testClass.VirtualMethodReturnsSameInt(1), Is.EqualTo(2));
            }

            [Test]
            public void
                Given_SubstituteForClass_And_VirtualMethodReturnsFuncs_When_VirtualMethodIsCalled_Then_ShouldReturnCorrectValues
                ()
            {
                var testClass = Substitute.For<TestClass>();
                testClass.VirtualMethodReturnsSameInt(Arg.Any<int>())
                            .Returns(
                                x => x.CallBase(),
                                x => 1,
                                x => { throw new Exception(); });
                Assert.That(testClass.VirtualMethodReturnsSameInt(2), Is.EqualTo(2));
                Assert.That(testClass.VirtualMethodReturnsSameInt(2), Is.EqualTo(1));
                Assert.Throws<Exception>(() => testClass.VirtualMethodReturnsSameInt(1));
            }

            [Test]
            public void
                Given_SubstituteForClass_And_VirtualMethodReturnsFuncs_When_VirtualMethodIsCalled_Then_ShouldCallBaseCorrect
                ()
            {
                var testClass = Substitute.For<TestClass>();
                testClass.VirtualMethodReturnsSameInt(Arg.Any<int>())
                            .Returns(
                                x => x.CallBase(),
                                x => 1,
                                x => 2);
                testClass.VirtualMethodReturnsSameInt(1);
                testClass.VirtualMethodReturnsSameInt(1);
                testClass.VirtualMethodReturnsSameInt(1);
                Assert.That(testClass.CalledTimes, Is.EqualTo(1));
            }
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