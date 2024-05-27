using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

/// <summary>
/// Issue from https://github.com/nsubstitute/NSubstitute/issues/378.
/// </summary>
public class Issue378_InValueTypes
{
    public readonly struct Struct(int value)
    {
        public int Value { get; } = value;
    }

    public interface IStructByReadOnlyRefConsumer { void Consume(in Struct value); }

    public interface IStructByValueConsumer { void Consume(Struct value); }

    public delegate void DelegateStructByReadOnlyRefConsumer(in Struct value);

    public delegate void DelegateStructByReadOnlyRefConsumerMultipleArgs(in Struct value1, in Struct value2);

    [Test]
    public void IStructByReadOnlyRefConsumer_Test()
    {
        var value = new Struct(42);

        var subs = Substitute.For<IStructByReadOnlyRefConsumer>();
        subs.Consume(in value);
    }

    [Test]
    public void IStructByValueConsumer_Test()
    {
        var value = new Struct(42);

        var subs = Substitute.For<IStructByValueConsumer>();
        subs.Consume(value);
    }

    [Test]
    public void DelegateByReadOnlyRefConsumer_Test()
    {
        var value = new Struct(42);

        var subs = Substitute.For<DelegateStructByReadOnlyRefConsumer>();
        subs.Invoke(in value);
    }

    [Test]
    public void InterfaceReadOnlyRefCannotBeModified()
    {
        var readOnlyValue = new Struct(42);

        var subs = Substitute.For<IStructByReadOnlyRefConsumer>();
        subs.When(x => x.Consume(Arg.Any<Struct>())).Do(c => { c[0] = new Struct(24); });

        subs.Consume(in readOnlyValue);

        Assert.That(readOnlyValue.Value, Is.EqualTo(42));
    }

    [Test]
    public void DelegateReadOnlyRefCannotBeModified()
    {
        var readOnlyValue = new Struct(42);

        var subs = Substitute.For<DelegateStructByReadOnlyRefConsumer>();
        subs.When(x => x.Invoke(Arg.Any<Struct>())).Do(c => { c[0] = new Struct(24); });

        subs.Invoke(in readOnlyValue);

        Assert.That(readOnlyValue.Value, Is.EqualTo(42));
    }

    [Test]
    public void DelegateMultipleReadOnlyRefCannotBeModified()
    {
        var readOnlyValue1 = new Struct(42);
        var readOnlyValue2 = new Struct(42);

        var subs = Substitute.For<DelegateStructByReadOnlyRefConsumerMultipleArgs>();
        subs.When(x => x.Invoke(Arg.Any<Struct>(), Arg.Any<Struct>()))
            .Do(c => { c[0] = new Struct(24); c[1] = new Struct(24); });

        subs.Invoke(in readOnlyValue1, in readOnlyValue2);

        Assert.That(readOnlyValue1.Value, Is.EqualTo(42));
        Assert.That(readOnlyValue2.Value, Is.EqualTo(42));
    }
}