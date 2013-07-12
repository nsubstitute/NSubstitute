using System;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{

    [TestFixture]
    public class WhenCallBase
    {
        [Test]
        public void Given_SubstituteForInterface_When_MethodIsCalled_Then_ShouldThrowException()
        {
            var testAbstractClass = Substitute.For<ITestInterface>();
            testAbstractClass.When(s => s.TestMethod()).CallBase();
            Assert.Throws<CouldNotCallBaseException>(testAbstractClass.TestMethod);
        }

        [Test]
        public void Given_SubstituteForAbstractClass_When_AbstractMethodIsCalled_Then_ShouldThrowException()
        {
            var testAbstractClass = Substitute.For<TestAbstractClass>();
            testAbstractClass.When(s => s.AbstractMethod()).CallBase();
            Assert.Throws<CouldNotCallBaseException>(testAbstractClass.AbstractMethod);
        }

        [Test]
        public void Given_SubstituteForAction_When_ActionIsCalled_Then_ShouldThrowException()
        {
            var action = Substitute.For<Action>();
            action.When(s => s()).CallBase();
            Assert.Throws<CouldNotCallBaseException>(() => action());
        }

        [Test]
        public void Given_SubstituteForFunc_When_FuncIsCalled_Then_ShouldThrowException()
        {
            var func = Substitute.For<Func<int>>();
            func.When(s => s()).CallBase();
            Assert.Throws<CouldNotCallBaseException>(() => func());
        }

        [Test]
        public void Given_SubstituteForEventHandler_When_EventHandlerIsCalled_Then_ShouldThrowException()
        {
            var eventHandler = Substitute.For<EventHandler>();
            eventHandler.When(s => s.Invoke(Arg.Any<object>(), Arg.Any<EventArgs>())).CallBase();
            Assert.Throws<CouldNotCallBaseException>(() => eventHandler.Invoke(null, null));
        }

        [Test]
        public void Given_SubstituteForAbstractClass_When_MethodWithImplementationIsCalled_Then_ShouldCallBaseImplementation()
        {
            var testAbstractClass = Substitute.For<TestAbstractClass>();
            testAbstractClass.When(s => s.MethodWithImplementation(Arg.Any<int>())).CallBase();
            Assert.That(testAbstractClass.MethodWithImplementation(1), Is.EqualTo(1));
        }

        [Test]
        public void Given_SubstituteForClass_When_VirtualMethodIsCalled_Then_ShouldCallBaseImplementation()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.When(s => s.VirtualMethod()).CallBase();
            testClass.VirtualMethod();
            Assert.That(testClass.CalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void Given_SubstituteForClass_When_VirtualMethodIsSetupTwice_Then_ShouldSetupCorrectly()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.When(s => s.VirtualMethod()).CallBase();
            testClass.When(s => s.VirtualMethod()).CallBase();
        }

        [Test]
        public void Given_SubstituteForClass_When_AbstractMethodIsCalled_Then_ShouldCallBaseImplementation()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.When(s => s.AbstractMethod()).CallBase();
            testClass.AbstractMethod();
            Assert.That(testClass.CalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void Given_SubstituteForClass_When_NotVirtualMethodIsSetup_And_NotVirtualMethodIsCalled_Then_ShouldCallBaseImplementationTwice()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.When(s => s.NotVirtualMethodReturnsSameInt(Arg.Any<int>())).CallBase();
            testClass.NotVirtualMethodReturnsSameInt(1);
            Assert.That(testClass.CalledTimes, Is.EqualTo(2));
        }

        [Test]
        public void Given_SubstituteForClass_When_VirtualMethodIsCalledTwice_Then_ShouldCallBaseImplementationTwice()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.When(s => s.VirtualMethod()).CallBase();
            testClass.VirtualMethod();
            testClass.VirtualMethod();
            Assert.That(testClass.CalledTimes, Is.EqualTo(2));
        }

        [Test]
        public void Given_SubstituteForClass_When_VirtualMethodsReturnValueIsOverwritten_Then_ShouldNotCallBaseImplementation()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.When(s => s.VirtualMethodReturnsSameInt(Arg.Any<int>())).CallBase();
            testClass.VirtualMethodReturnsSameInt(Arg.Any<int>()).Returns(1);
            Assert.That(testClass.CalledTimes, Is.EqualTo(0));
        }

        [Test]
        public void Given_SubstituteForClass_And_ReturnValueIsOverwritten_When_VirtualMethodIsCalled_Then_ShouldReturnOverwrittenValue()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.When(s => s.VirtualMethodReturnsSameInt(Arg.Any<int>())).CallBase();
            testClass.VirtualMethodReturnsSameInt(Arg.Any<int>()).Returns(2);
            Assert.That(testClass.VirtualMethodReturnsSameInt(1), Is.EqualTo(2));
        }

        [Test]
        [Pending, Explicit]
        // test fails only when executed in batch
        public void Given_SubstituteForClass_And_ObjectReturnValueIsOverwritten_When_VirtualMethodIsCalled_Then_ShouldReturnOverwrittenValue()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.When(s => s.VirtualMethodReturnsSameObject(Arg.Any<object>())).CallBase();
            var objectToReturn = new object();
            testClass.VirtualMethodReturnsSameObject(Arg.Any<object>()).Returns(objectToReturn);
            Assert.That(testClass.VirtualMethodReturnsSameObject(new object()), Is.EqualTo(objectToReturn));
        }

        [Test]
        [Pending, Explicit]
        public void Given_SubstituteForClass_And_SubstituteDoesNotOverwriteReturn_When_VirtualMethodIsCalled_Then_ShouldCallBaseImplementation()
        {
            var testClass = Substitute.For<TestClass>();
            var objectCallBase = new object();
            testClass.When(s => s.VirtualMethodReturnsSameObject(Arg.Is(objectCallBase))).CallBase();
            var objectCallSubstitute = new object();
            testClass.VirtualMethodReturnsSameObject(Arg.Is(objectCallSubstitute)).Returns(new object());
            Assert.That(testClass.VirtualMethodReturnsSameObject(objectCallBase), Is.EqualTo(objectCallBase));
        }

        [Test]
        public void Given_SubstituteForClass_And_SubstituteDoesOverwriteReturn_When_VirtualMethodIsCalled_Then_ShouldReturnOverwrittenValue()
        {
            var testClass = Substitute.For<TestClass>();
            var objectCallBase = new object();
            testClass.When(s => s.VirtualMethodReturnsSameObject(Arg.Is(objectCallBase))).CallBase();
            var objectToReturn = new object();
            testClass.VirtualMethodReturnsSameObject(Arg.Is(objectCallBase)).Returns(objectToReturn);
            Assert.That(testClass.VirtualMethodReturnsSameObject(objectCallBase), Is.EqualTo(objectToReturn));
        }

        [Test]
        public void Given_SubstituteForClass_And_VirtualMethodReturnValueForSpecifiedArg_When_VirtualMethodIsCalledWithSpecifiedArg_Then_ShouldCallBaseImplementation()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.When(s => s.VirtualMethodReturnsSameInt(Arg.Any<int>())).CallBase();
            testClass.VirtualMethodReturnsSameInt(Arg.Is(1)).Returns(2);
            Assert.That(testClass.VirtualMethodReturnsSameInt(1), Is.EqualTo(1));
            Assert.That(testClass.CalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void Given_SubstituteForClass_And_VirtualMethodReturnValueForSpecifiedArg_When_VirtualMethodIsCalledWithoutSpecifiedArg_Then_ShouldCallBaseImplementation()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.When(s => s.VirtualMethodReturnsSameInt(Arg.Any<int>())).CallBase();
            testClass.VirtualMethodReturnsSameInt(Arg.Is(1)).Returns(2);
            Assert.That(testClass.VirtualMethodReturnsSameInt(3), Is.EqualTo(3));
            Assert.That(testClass.CalledTimes, Is.EqualTo(1));
        }

        public interface ITestInterface
        {
            void TestMethod();
        }

        public abstract class TestAbstractClass
        {
            public abstract void AbstractMethod();
            public int MethodWithImplementation(int i)
            {
                return i;
            }
        }

        public class TestClass : TestAbstractClass
        {
            public int CalledTimes { get; set; }

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

            public virtual void VirtualMethod()
            {
                CalledTimes++;
            }

            public int NotVirtualMethodReturnsSameInt(int i)
            {
                CalledTimes++;
                return i;
            }

            public override void AbstractMethod()
            {
                CalledTimes++;
            }
        }
    }
}