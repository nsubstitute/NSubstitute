using System;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class CallBaseForMember
    {
        [Test]
        public void Given_SubstituteForInterface_When_MethodIsCalled_Then_ShouldThrowException()
        {
            var testAbstractClass = Substitute.For<ITestInterface>();
            Assert.Throws<CouldNotCallBaseException>(() => testAbstractClass.CallBaseFor().TestMethod());
        }

        [Test]
        public void Given_SubstituteForAbstractClass_When_AbstractMethodIsCalled_Then_ShouldThrowException()
        {
            var testAbstractClass = Substitute.For<TestAbstractClass>();
            Assert.Throws<CouldNotCallBaseException>(() => testAbstractClass.CallBaseFor().AbstractMethod());
        }

        [Test]
        public void Given_SubstituteForAction_When_ActionIsCalled_Then_ShouldThrowException()
        {
            var action = Substitute.For<Action>();
            Assert.Throws<CouldNotCallBaseException>(() => action.CallBaseFor().Invoke());
        }

        [Test]
        public void Given_SubstituteForFunc_When_FuncIsCalled_Then_ShouldThrowException()
        {
            var func = Substitute.For<Func<int>>();
            Assert.Throws<CouldNotCallBaseException>(() => func.CallBaseFor().Invoke());
        }

        [Test]
        public void Given_SubstituteForEventHandler_When_EventHandlerIsCalled_Then_ShouldThrowException()
        {
            var eventHandler = Substitute.For<EventHandler>();
            Assert.Throws<CouldNotCallBaseException>(() =>
                eventHandler.CallBaseFor().Invoke(Arg.Any<object>(), Arg.Any<EventArgs>()));
        }

        [Test]
        public void
            Given_SubstituteForAbstractClass_When_MethodWithImplementationIsCalled_Then_ShouldCallBaseImplementation
            ()
        {
            var testAbstractClass = Substitute.For<TestAbstractClass>();
            testAbstractClass.CallBaseFor().MethodWithImplementation(Arg.Any<int>());
            Assert.That(testAbstractClass.MethodWithImplementation(1), Is.EqualTo(1));
        }

        [Test]
        public void Given_SubstituteForClass_When_VirtualMethodIsCalled_Then_ShouldCallBaseImplementation()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.CallBaseFor().VirtualMethod();
            testClass.VirtualMethod();
            Assert.That(testClass.CalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void Given_SubstituteForClass_When_CallBaseUsed_ShouldNotCallRealImplementation()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.CallBaseFor().VirtualMethod();
            testClass.CallBaseFor().VirtualMethod();

            Assert.That(testClass.CalledTimes, Is.EqualTo(0));
        }

        [Test]
        public void Given_SubstituteForClass_When_VirtualMethodIsSetupTwice_Then_ShouldSetupCorrectly()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.CallBaseFor().VirtualMethod();
            testClass.CallBaseFor().VirtualMethod();

            testClass.VirtualMethod();
            Assert.That(testClass.CalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void Given_SubstituteForClass_When_AbstractMethodIsCalled_Then_ShouldCallBaseImplementation()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.CallBaseFor().AbstractMethod();
            testClass.AbstractMethod();
            Assert.That(testClass.CalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void Given_SubstituteForClass_When_VirtualMethodIsCalledTwice_Then_ShouldCallBaseImplementationTwice()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.CallBaseFor().VirtualMethod();
            testClass.VirtualMethod();
            testClass.VirtualMethod();
            Assert.That(testClass.CalledTimes, Is.EqualTo(2));
        }

        [Test]
        public void
            Given_SubstituteForClass_When_VirtualMethodsReturnValueIsOverwritten_Then_ShouldNotCallBaseImplementation
            ()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.CallBaseFor().VirtualMethodReturnsSameInt(Arg.Any<int>());
            testClass.VirtualMethodReturnsSameInt(Arg.Any<int>()).Returns(1);
            Assert.That(testClass.CalledTimes, Is.EqualTo(0));
        }

        [Test]
        public void
            Given_SubstituteForClass_And_ReturnValueIsOverwritten_When_VirtualMethodIsCalled_Then_ShouldReturnOverwrittenValue
            ()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.CallBaseFor().VirtualMethodReturnsSameInt(Arg.Any<int>());
            testClass.VirtualMethodReturnsSameInt(Arg.Any<int>()).Returns(2);
            Assert.That(testClass.VirtualMethodReturnsSameInt(1), Is.EqualTo(2));
        }

        [Test]
        public void
            Given_SubstituteForClass_And_ObjectReturnValueIsOverwritten_When_VirtualMethodIsCalled_Then_ShouldReturnOverwrittenValue
            ()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.CallBaseFor().VirtualMethodReturnsSameObject(Arg.Any<object>());
            var objectToReturn = new object();
            testClass.VirtualMethodReturnsSameObject(Arg.Any<object>()).Returns(objectToReturn);
            Assert.That(testClass.VirtualMethodReturnsSameObject(new object()), Is.EqualTo(objectToReturn));
        }

        [Test]
        public void
            Given_SubstituteForClass_And_SubstituteDoesNotOverwriteReturn_When_VirtualMethodIsCalled_Then_ShouldCallBaseImplementation
            ()
        {
            var testClass = Substitute.For<TestClass>();
            var objectCallBase = new object();
            testClass.CallBaseFor().VirtualMethodReturnsSameObject(Arg.Is(objectCallBase));
            var objectCallSubstitute = new object();
            testClass.VirtualMethodReturnsSameObject(Arg.Is(objectCallSubstitute)).Returns(new object());
            Assert.That(testClass.VirtualMethodReturnsSameObject(objectCallBase), Is.EqualTo(objectCallBase));
        }

        [Test]
        public void
            Given_SubstituteForClass_And_SubstituteDoesOverwriteReturn_When_VirtualMethodIsCalled_Then_ShouldReturnOverwrittenValue
            ()
        {
            var testClass = Substitute.For<TestClass>();
            var objectCallBase = new object();
            testClass.CallBaseFor().VirtualMethodReturnsSameObject(Arg.Is(objectCallBase));
            var objectToReturn = new object();
            testClass.VirtualMethodReturnsSameObject(Arg.Is(objectCallBase)).Returns(objectToReturn);
            Assert.That(testClass.VirtualMethodReturnsSameObject(objectCallBase), Is.EqualTo(objectToReturn));
        }

        [Test]
        public void
            Given_SubstituteForClass_And_VirtualMethodReturnValueForSpecifiedArg_When_VirtualMethodIsCalledWithSpecifiedArg_Then_ShouldReturnOverwrittenValue
            ()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.CallBaseFor().VirtualMethodReturnsSameInt(Arg.Any<int>());
            testClass.VirtualMethodReturnsSameInt(Arg.Is(1)).Returns(2);
            // substitute has higher priority than base implementation
            Assert.That(testClass.VirtualMethodReturnsSameInt(1), Is.EqualTo(2));
            Assert.That(testClass.CalledTimes, Is.EqualTo(0));
        }

        [Test]
        public void
            Given_SubstituteForClass_And_VirtualMethodReturnValueForSpecifiedArg_When_VirtualMethodIsCalledWithoutSpecifiedArg_Then_ShouldCallBaseImplementation
            ()
        {
            var testClass = Substitute.For<TestClass>();
            testClass.CallBaseFor().VirtualMethodReturnsSameInt(Arg.Any<int>());
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

            public virtual int MethodWithImplementation(int i)
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

            public override void AbstractMethod()
            {
                CalledTimes++;
            }
        }
    }
}