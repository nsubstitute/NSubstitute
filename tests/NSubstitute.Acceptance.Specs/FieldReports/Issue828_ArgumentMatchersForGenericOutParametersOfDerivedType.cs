using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue828_ArgumentMatchersForGenericOutParametersOfDerivedType
{
    // Based on: https://github.com/nsubstitute/NSubstitute/issues/828

    public interface IOutBase { }
    public interface IOutDerived : IOutBase { }

    public interface IServiceBase
    {
        void TryGet<T>(out T value) where T : IOutBase;
    }

    public interface IServiceDerived : IServiceBase
    {
        new void TryGet<T>(out T value) where T : IOutDerived;
    }

    [Test]
    public void ShouldDistinguishBetweenBaseAndDerivedGenericOverloads()
    {
        var service = Substitute.For<IServiceDerived>();

        ((IServiceDerived)service).TryGet(out IOutDerived _); // Call derived version
        ((IServiceBase)service).TryGet(out IOutBase _);       // Call base version

        // Each overload is a distinct method, so each should only match its own call
        // even though the argument (a null out value) is compatible with both.
        service.Received(1).TryGet(out Arg.Any<IOutDerived>());
        ((IServiceBase)service).Received(1).TryGet(out Arg.Any<IOutBase>());
    }

    public abstract class BaseClass
    {
        public virtual T Echo<T>(T value) => value;
    }

    public class DerivedClass : BaseClass
    {
        public override T Echo<T>(T value) => value;
    }

    [Test]
    public void ShouldStillMatchOverriddenGenericMethodReachedViaDifferentDeclaringTypes()
    {
        // Overriding a generic method (unlike 'new' hiding) keeps a single slot, so the
        // spec and the recorded call resolve to the same generic method definition whether
        // the substitute is used through the derived or the base type. Comparing generic
        // method definitions (rather than names) must not break this.
        var sub = Substitute.For<DerivedClass>();

        sub.Echo(Arg.Any<int>()).Returns(42);

        Assert.That(sub.Echo(5), Is.EqualTo(42));
        sub.Received(1).Echo(Arg.Any<int>());
        ((BaseClass)sub).Received(1).Echo(Arg.Any<int>());
    }
}
