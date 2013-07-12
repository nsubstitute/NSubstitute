using System;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class SubstituteTests
    {
        [TestFixture]
        public class ForTests
        {
            [TestFixture]
            public class ThatCallsBaseByDefaultTests
            {
                [Test]
                public void Given_SubstituteForInterface_When_MethodIsCalled_Then_ShouldThrowException()
                {
                    var testAbstractClass = Substitute.For<ITestInterface>(ThatCallsBase.ByDefault);
                    Assert.Throws<CouldNotCallBaseException>(testAbstractClass.TestMethod);
                }

                [Test]
                public void Given_SubstituteForAbstractClass_When_AbstractMethodIsCalled_Then_ShouldThrowException()
                {
                    var testAbstractClass = Substitute.For<TestAbstractClass>(ThatCallsBase.ByDefault);
                    Assert.Throws<CouldNotCallBaseException>(testAbstractClass.AbstractMethod);
                }

                [Test]
                public void Given_SubstituteForAction_When_ActionIsCalled_Then_ShouldThrowException()
                {
                    var action = Substitute.For<Action>(ThatCallsBase.ByDefault);
                    Assert.Throws<CouldNotCallBaseException>(() => action());
                }

                [Test]
                public void Given_SubstituteForFunc_When_FuncIsCalled_Then_ShouldThrowException()
                {
                    var func = Substitute.For<Func<int>>(ThatCallsBase.ByDefault);
                    Assert.Throws<CouldNotCallBaseException>(() => func());
                }

                [Test]
                public void Given_SubstituteForEventHandler_When_EventHandlerIsCalled_Then_ShouldThrowException()
                {
                    var eventHandler = Substitute.For<EventHandler>(ThatCallsBase.ByDefault);
                    Assert.Throws<CouldNotCallBaseException>(() => eventHandler.Invoke(null, null));
                }

                [Test]
                public void Given_SubstituteForAbstractClass_When_MethodWithImplementationIsCalled_Then_ShouldCallBaseImplementation()
                {
                    var testAbstractClass = Substitute.For<TestAbstractClass>(ThatCallsBase.ByDefault);
                    Assert.That(testAbstractClass.MethodWithImplementation(1), Is.EqualTo(1));
                }

                [Test]
                public void Given_SubstituteForClass_When_VirtualMethodIsCalled_Then_ShouldCallBaseImplementation()
                {
                    var testClass = Substitute.For<TestClass>(ThatCallsBase.ByDefault);
                    testClass.VirtualMethod();
                    Assert.That(testClass.CalledTimes, Is.EqualTo(1));
                }

                [Test]
                public void Given_SubstituteForClass_When_AbstractMethodIsCalled_Then_ShouldCallBaseImplementation()
                {
                    var testClass = Substitute.For<TestClass>(ThatCallsBase.ByDefault);
                    testClass.AbstractMethod();
                    Assert.That(testClass.CalledTimes, Is.EqualTo(1));
                }

                [Test]
                public void Given_SubstituteForClass_When_NotVirtualMethodIsCalled_Then_ShouldCallBaseImplementation()
                {
                    var testClass = Substitute.For<TestClass>(ThatCallsBase.ByDefault);
                    testClass.NotVirtualMethodReturnsSameInt(1);
                    Assert.That(testClass.CalledTimes, Is.EqualTo(1));
                }

                [Test]
                public void Given_SubstituteForClass_When_VirtualMethodIsCalledTwice_Then_ShouldCallBaseImplementationTwice()
                {
                    var testClass = Substitute.For<TestClass>(ThatCallsBase.ByDefault);
                    testClass.VirtualMethod();
                    testClass.VirtualMethod();
                    Assert.That(testClass.CalledTimes, Is.EqualTo(2));
                }

                [Test]
                public void Given_SubstituteForClass_When_VirtualMethodsReturnValueIsOverwritten_Then_ShouldNotCallBaseImplementation()
                {
                    var testClass = Substitute.For<TestClass>(ThatCallsBase.ByDefault);
                    testClass.VirtualMethodReturnsSameInt(Arg.Any<int>()).Returns(1);
                    Assert.That(testClass.CalledTimes, Is.EqualTo(0));
                }

                [Test]
                public void Given_SubstituteForClass_And_ReturnValueIsOverwritten_When_VirtualMethodIsCalled_Then_ShouldReturnOverwrittenValue()
                {
                    var testClass = Substitute.For<TestClass>(ThatCallsBase.ByDefault);
                    testClass.VirtualMethodReturnsSameInt(Arg.Any<int>()).Returns(2);
                    Assert.That(testClass.VirtualMethodReturnsSameInt(1), Is.EqualTo(2));
                }

                [Test]
                public void Given_SubstituteForClass_And_VirtualMethodReturnValueForSpecifiedArg_When_VirtualMethodIsCalledWithSpecifiedArg_Then_ShouldNotCallBaseImplementation()
                {
                    var testClass = Substitute.For<TestClass>(ThatCallsBase.ByDefault);
                    testClass.VirtualMethodReturnsSameInt(Arg.Is(1)).Returns(2);
                    Assert.That(testClass.VirtualMethodReturnsSameInt(1), Is.EqualTo(2));
                    Assert.That(testClass.CalledTimes, Is.EqualTo(0));
                }

                [Test]
                public void Given_SubstituteForClass_And_VirtualMethodReturnValueForSpecifiedArg_When_VirtualMethodIsCalledWithoutSpecifiedArg_Then_ShouldCallBaseImplementation()
                {
                    var testClass = Substitute.For<TestClass>(ThatCallsBase.ByDefault);
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
    }
}
