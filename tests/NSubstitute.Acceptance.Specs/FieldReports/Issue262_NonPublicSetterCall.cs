using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

/// <summary>
/// Scenarios for the issue: https://github.com/nsubstitute/NSubstitute/issues/626
///
/// Do not test internal members, as we don't want to set InternalsVisibleTo attribute.
/// </summary>
public class Issue262_NonPublicSetterCall
{
    [Test]
    public void ShouldHandleProtectedProperties()
    {
        var subs = Substitute.For<TestClass>();

        subs.SetProtectedProp(42);

        var result = subs.GetProtectedProp();
        Assert.That(result, Is.EqualTo(expected: 42));
    }

    [Test]
    public void ShouldHandlePropertyWithProtectedSetter()
    {
        var subs = Substitute.For<TestClass>();

        subs.SetProtectedSetterProp(42);

        var result = subs.ProtectedSetterProp;
        Assert.That(result, Is.EqualTo(expected: 42));
    }
    [Test]
    public void ShouldHandlePropertyWithProtectedGetter()
    {
        var subs = Substitute.For<TestClass>();

        subs.ProtectedGetterProp = 42;

        var result = subs.GetProtectedGetterProp();
        Assert.That(result, Is.EqualTo(expected: 42));
    }

    public abstract class TestClass
    {
        protected abstract int ProtectedProp { get; set; }
        public void SetProtectedProp(int value) => ProtectedProp = value;
        public int GetProtectedProp() => ProtectedProp;

        public abstract int ProtectedSetterProp { get; protected set; }
        public void SetProtectedSetterProp(int value) => ProtectedSetterProp = value;

        public abstract int ProtectedGetterProp { protected get; set; }
        public int GetProtectedGetterProp() => ProtectedGetterProp;
    }
}