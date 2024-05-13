

using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Extensions;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace NSubstitute.Acceptance.Specs;

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
        ClassicAssert.AreEqual(0, result);
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
        public void UseImplementedNonVirtualMethod()
        {
            var testAbstractClass = Substitute.ForPartsOf<ITestInterface, TestNonVirtualClass>();
            Assert.That(testAbstractClass.MethodReturnsSameInt(1), Is.EqualTo(1));
            Assert.That(testAbstractClass.CalledTimes, Is.EqualTo(1));
            testAbstractClass.Received().MethodReturnsSameInt(1);
            Assert.That(testAbstractClass.CalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void UseSubstitutedNonVirtualMethod()
        {
            var testInterface = Substitute.ForPartsOf<ITestInterface, TestNonVirtualClass>();
            testInterface.Configure().MethodReturnsSameInt(1).Returns(2);
            Assert.That(testInterface.MethodReturnsSameInt(1), Is.EqualTo(2));
            Assert.That(testInterface.MethodReturnsSameInt(3), Is.EqualTo(3));
            testInterface.ReceivedWithAnyArgs(2).MethodReturnsSameInt(default);
            Assert.That(testInterface.CalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void UseSubstitutedNonVirtualMethodHonorsDoNotCallBase()
        {
            var testInterface = Substitute.ForPartsOf<ITestInterface, TestNonVirtualClass>();
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
            var testInterface = Substitute.ForPartsOf<ITestInterface, TestNonVirtualClass>(50);
            Assert.That(testInterface.MethodReturnsSameInt(1), Is.EqualTo(1));
            Assert.That(testInterface.CalledTimes, Is.EqualTo(51));
        }

        [Test]
        public void PartialSubstituteFailsIfClassDoesntImplementInterface()
        {
            Assert.Throws<SubstituteException>(
                () => Substitute.ForPartsOf<ITestInterface, TestAbstractClass>());
        }


        [Test]
        public void PartialSubstituteFailsIfClassIsAbstract()
        {
            Assert.Throws<SubstituteException>(
                () => Substitute.ForPartsOf<ITestInterface, TestAbstractClassWithInterface>(), "The provided class is abstract.");
        }

    [Test]
    public void ReturnDefaultForUnimplementedAbstractMethod()
    {
        var testAbstractClass = Substitute.ForPartsOf<TestAbstractClass>();
        ClassicAssert.AreEqual(default(int), testAbstractClass.AbstractMethodReturnsSameInt(1));
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
    public void DoNotCallBaseWhenConfiguringReturnsIfConfigureHintIsPresent()
    {
        var testClass = Substitute.ForPartsOf<TestClass>();
        testClass.Configure().MethodReturnsSameInt(2).Returns(4);

        var result = testClass.MethodReturnsSameInt(2);

        Assert.That(testClass.CalledTimes, Is.EqualTo(0), "should not call base as NSub knows we are specifying a call");
        Assert.That(result, Is.EqualTo(4));
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

    [Test]
    public void DoNotCallBaseByDefaultWhenDisabledViaRouter()
    {
        var substitute = Substitute.ForPartsOf<TestAbstractClass>();
        SubstitutionContext.Current.GetCallRouterFor(substitute).CallBaseByDefault = false;

        substitute.MethodReturnsSameInt(42);

        Assert.That(substitute.CalledTimes, Is.EqualTo(0));
    }

    [Test]
    public void CallBaseByDefaultWhenEnabledViaRouter()
    {
        var substitute = Substitute.For<TestAbstractClass>();
        SubstitutionContext.Current.GetCallRouterFor(substitute).CallBaseByDefault = true;

        substitute.MethodReturnsSameInt(42);

        Assert.That(substitute.CalledTimes, Is.EqualTo(1));
    }

    [Test]
    public void DoNotCallBaseWhenEnabledViaRouterButExplicitlyDisabled()
    {
        var substitute = Substitute.For<TestAbstractClass>();
        SubstitutionContext.Current.GetCallRouterFor(substitute).CallBaseByDefault = true;
        substitute.When(x => x.MethodReturnsSameInt(Arg.Any<int>())).DoNotCallBase();

        substitute.MethodReturnsSameInt(42);

        Assert.That(substitute.CalledTimes, Is.EqualTo(0));
    }

    [Test]
    public void CallBaseForNonPartialProxyWhenExplicitlyEnabled()
    {
        var substitute = Substitute.For<TestAbstractClass>();
        substitute.When(x => x.MethodReturnsSameInt(Arg.Any<int>())).CallBase();

        substitute.MethodReturnsSameInt(42);

        Assert.That(substitute.CalledTimes, Is.EqualTo(1));
    }

    [Test]
    public void CallBaseWhenDisabledViaRouterButExplicitlyEnabled()
    {
        var substitute = Substitute.For<TestAbstractClass>();
        SubstitutionContext.Current.GetCallRouterFor(substitute).CallBaseByDefault = false;
        substitute.When(x => x.MethodReturnsSameInt(Arg.Any<int>())).CallBase();

        substitute.MethodReturnsSameInt(42);

        Assert.That(substitute.CalledTimes, Is.EqualTo(1));
    }

    [Test]
    public void DoNotCallBaseWhenExplicitlyEnabledAndThenDisabled()
    {
        var substitute = Substitute.For<TestAbstractClass>();
        SubstitutionContext.Current.GetCallRouterFor(substitute).CallBaseByDefault = true;

        substitute.When(x => x.MethodReturnsSameInt(Arg.Any<int>())).CallBase();
        substitute.When(x => x.MethodReturnsSameInt(Arg.Any<int>())).DoNotCallBase();
        substitute.MethodReturnsSameInt(42);

        Assert.That(substitute.CalledTimes, Is.EqualTo(0));
    }

    [Test]
    public void CallBaseWhenExplicitlyDisabledAndThenEnabled()
    {
        var substitute = Substitute.For<TestAbstractClass>();
        SubstitutionContext.Current.GetCallRouterFor(substitute).CallBaseByDefault = true;

        substitute.When(x => x.MethodReturnsSameInt(Arg.Any<int>())).DoNotCallBase();
        substitute.When(x => x.MethodReturnsSameInt(Arg.Any<int>())).CallBase();
        substitute.MethodReturnsSameInt(42);

        Assert.That(substitute.CalledTimes, Is.EqualTo(1));
    }

    [Test]
    public void ShouldThrowExceptionIfTryToNotCallBaseForAbstractMethod()
    {
        var substitute = Substitute.For<TestAbstractClass>();

        var ex = Assert.Throws<CouldNotConfigureCallBaseException>(
            () => substitute.When(x => x.AbstractMethodReturnsSameInt(Arg.Any<int>())).DoNotCallBase());
        Assert.That(ex.Message, Contains.Substring("base method implementation is missing"));
    }

    [Test]
    public void ShouldThrowExceptionIfTryToNotCallBaseForInterfaceProxy()
    {
        var substitute = Substitute.For<ITestInterface>();

        var ex = Assert.Throws<CouldNotConfigureCallBaseException>(
            () => substitute.When(x => x.TestMethodReturnsInt()).DoNotCallBase());
        Assert.That(ex.Message, Contains.Substring("base method implementation is missing"));
    }

    [Test]
    public void ShouldThrowExceptionIfTryToCallBaseForAbstractMethod()
    {
        var substitute = Substitute.For<TestAbstractClass>();

        var ex = Assert.Throws<CouldNotConfigureCallBaseException>(
            () => substitute.When(x => x.AbstractMethodReturnsSameInt(Arg.Any<int>())).CallBase());
        Assert.That(ex.Message, Contains.Substring("base method implementation is missing"));
    }

    [Test]
    public void ShouldThrowExceptionIfTryToCallBaseForInterfaceProxy()
    {
        var substitute = Substitute.For<ITestInterface>();

        var ex = Assert.Throws<CouldNotConfigureCallBaseException>(
            () => substitute.When(x => x.TestMethodReturnsInt()).CallBase());
        Assert.That(ex.Message, Contains.Substring("base method implementation is missing"));
    }

    [Test]
    public void ShouldThrowExceptionIfConfigureGlobalCallBaseForInterfaceProxy()
    {
        var substitute = Substitute.For<ITestInterface>();
        var callRouter = SubstitutionContext.Current.GetCallRouterFor(substitute);

        var ex = Assert.Throws<CouldNotConfigureCallBaseException>(
            () => callRouter.CallBaseByDefault = false);
        Assert.That(ex.Message, Contains.Substring("can be configured for a class substitute only"));
    }

    [Test]
    public void ShouldThrowExceptionIfConfigureGlobalCallBaseForDelegateProxy()
    {
        var substitute = Substitute.For<Func<int>>();
        var callRouter = SubstitutionContext.Current.GetCallRouterFor(substitute);

        var ex = Assert.Throws<CouldNotConfigureCallBaseException>(
            () => callRouter.CallBaseByDefault = false);
        Assert.That(ex.Message, Contains.Substring("can be configured for a class substitute only"));
    }

    public interface ITestInterface
    {
            public int CalledTimes { get; set; }

        void VoidTestMethod();
        int TestMethodReturnsInt();
            int MethodReturnsSameInt(int i);
        }

        public class TestNonVirtualClass : ITestInterface
        {
            public TestNonVirtualClass() { }
            public TestNonVirtualClass(int initialCounter) => CalledTimes = initialCounter;

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