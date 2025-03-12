using NSubstitute.Exceptions;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class TypeForwarding
{
    [Test]
    public void UseImplementedNonVirtualMethod()
    {
        var testAbstractClass = Substitute.ForTypeForwardingTo<ITestInterface, TestSealedNonVirtualClass>();
        Assert.That(testAbstractClass.MethodReturnsSameInt(1), Is.EqualTo(1));
        Assert.That(testAbstractClass.CalledTimes, Is.EqualTo(1));
        testAbstractClass.Received().MethodReturnsSameInt(1);
        Assert.That(testAbstractClass.CalledTimes, Is.EqualTo(1));
    }

    [Test]
    public void UseSubstitutedNonVirtualMethod()
    {
        var testInterface = Substitute.ForTypeForwardingTo<ITestInterface, TestSealedNonVirtualClass>();
        testInterface.Configure().MethodReturnsSameInt(1).Returns(2);
        Assert.That(testInterface.MethodReturnsSameInt(1), Is.EqualTo(2));
        Assert.That(testInterface.MethodReturnsSameInt(3), Is.EqualTo(3));
        testInterface.ReceivedWithAnyArgs(2).MethodReturnsSameInt(default);
        Assert.That(testInterface.CalledTimes, Is.EqualTo(1));
    }

    [Test]
    public void UseSubstitutedNonVirtualMethodHonorsDoNotCallBase()
    {
        var testInterface = Substitute.ForTypeForwardingTo<ITestInterface, TestSealedNonVirtualClass>();
        testInterface.Configure().MethodReturnsSameInt(1).Returns(2);
        testInterface.WhenForAnyArgs(x => x.MethodReturnsSameInt(default)).DoNotCallBase();
        Assert.That(testInterface.MethodReturnsSameInt(1), Is.EqualTo(2));
        Assert.That(testInterface.MethodReturnsSameInt(3), Is.EqualTo(0));
        testInterface.ReceivedWithAnyArgs(2).MethodReturnsSameInt(default);
        Assert.That(testInterface.CalledTimes, Is.EqualTo(0));
    }

    [Test]
    public void PartialSubstituteCallsConstructorWithParameters()
    {
        var testInterface = Substitute.ForTypeForwardingTo<ITestInterface, TestSealedNonVirtualClass>(50);
        Assert.That(testInterface.MethodReturnsSameInt(1), Is.EqualTo(1));
        Assert.That(testInterface.CalledTimes, Is.EqualTo(51));
    }

    [Test]
    public void PartialSubstituteFailsIfClassDoesntImplementInterface()
    {
        Assert.Throws<CanNotForwardCallsToClassNotImplementingInterfaceException>(
            () => Substitute.ForTypeForwardingTo<ITestInterface, TestRandomConcreteClass>());
    }

    [Test]
    public void PartialSubstituteFailsIfClassIsAbstract()
    {
        Assert.Throws<CanNotForwardCallsToAbstractClassException>(
            () => Substitute.ForTypeForwardingTo<ITestInterface, TestAbstractClassWithInterface>(), "The provided class is abstract.");
    }

    public interface ITestInterface
    {
        public int CalledTimes { get; set; }

        void VoidTestMethod();
        int TestMethodReturnsInt();
        int MethodReturnsSameInt(int i);
    }

    public sealed class TestSealedNonVirtualClass : ITestInterface
    {
        public TestSealedNonVirtualClass(int initialCounter) => CalledTimes = initialCounter;
        public TestSealedNonVirtualClass() { }

        public int CalledTimes { get; set; }

        public int TestMethodReturnsInt() => throw new NotImplementedException();

        public void VoidTestMethod() => throw new NotImplementedException();
        public int MethodReturnsSameInt(int i)
        {
            CalledTimes++;
            return i;
        }
    }

    public abstract class TestAbstractClassWithInterface : ITestInterface
    {
        public int CalledTimes { get; set; }

        public abstract int MethodReturnsSameInt(int i);

        public abstract int TestMethodReturnsInt();

        public abstract void VoidTestMethod();
    }

    public class TestRandomConcreteClass { }

    public abstract class TestAbstractClass { }
}