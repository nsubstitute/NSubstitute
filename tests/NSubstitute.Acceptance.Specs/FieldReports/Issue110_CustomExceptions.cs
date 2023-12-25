using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue110_CustomExceptions
{
    public class MyException : Exception { }

    public interface IThrow { void Throw(); }
    public interface IDoStuff { event Action StuffDone; }

    [Test]
    public void ThrowExceptionWithoutSerialisationConstructor()
    {
        var ithrow = Substitute.For<IThrow>();
        var doStuff = Substitute.For<IDoStuff>();

        ithrow.When(x => x.Throw()).Do(x => { throw new MyException(); });
        doStuff.StuffDone += ithrow.Throw;

        Assert.Throws<MyException>(() => doStuff.StuffDone += Raise.Event<Action>());
    }
}