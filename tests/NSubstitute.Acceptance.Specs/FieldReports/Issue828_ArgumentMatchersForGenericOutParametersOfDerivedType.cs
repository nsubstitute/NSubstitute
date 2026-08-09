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
}
