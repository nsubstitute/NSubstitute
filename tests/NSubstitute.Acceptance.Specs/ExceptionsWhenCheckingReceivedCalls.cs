using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class ExceptionsWhenCheckingReceivedCalls
{
    public interface IFoo
    {
        void Bar();
        void Zap(int a);
    }

    IFoo _foo;

    [SetUp]
    public void SetUp()
    {
        _foo = Substitute.For<IFoo>();
    }

    [Test]
    public void Expected_call_was_not_received()
    {
        ShouldThrow(() => _foo.Received().Bar());
    }

    [Test]
    public void Unexpected_call_was_received()
    {
        _foo.Bar();

        ShouldThrow(() => _foo.DidNotReceive().Bar());
        ShouldThrow(() => _foo.Received(0).Bar());
    }

    [Test]
    public void Expected_call_was_received_too_many_times()
    {
        _foo.Bar();
        _foo.Bar();
        _foo.Bar();

        ShouldThrow(() => _foo.Received(2).Bar());
    }

    [Test]
    public void Expected_call_was_not_received_enough_times()
    {
        _foo.Bar();

        ShouldThrow(() => _foo.Received(2).Bar());
    }

    [Test]
    public void Expected_single_call()
    {
        _foo.Bar();
        _foo.Bar();
        ShouldThrow(() => _foo.Received(1).Bar());
    }

    [Test]
    public void Expected_call_not_received_enough_times_and_other_related_calls_received()
    {
        _foo.Zap(1);
        _foo.Zap(1);
        _foo.Zap(1);
        _foo.Zap(2);

        ShouldThrow(() => _foo.Received(4).Zap(1));
    }

    [Test]
    public void Expected_call_received_enough_times_and_other_related_calls_received()
    {
        _foo.Zap(1);
        _foo.Zap(1);
        _foo.Zap(1);
        _foo.Zap(2);

        ShouldThrow(() => _foo.Received(2).Zap(1));
    }

    [Test]
    public void Expected_call_specified_with_argument_matchers_not_received_enough_times()
    {
        _foo.Zap(1);
        _foo.Zap(2);
        _foo.Zap(3);
        _foo.Zap(4);

        ShouldThrow(() => _foo.Received(4).Zap(Arg.Is<int>(x => x < 4)));
    }

    [Test]
    public void Expected_call_specified_with_argument_matchers_received_enough_times()
    {
        _foo.Zap(1);
        _foo.Zap(2);
        _foo.Zap(3);
        _foo.Zap(4);

        _foo.Received(3).Zap(Arg.Is<int>(x => x < 4));
    }

    void ShouldThrow(Action action)
    {
        try
        {
            action();
        }
        catch (ReceivedCallsException)
        {
            return;
        }
        Assert.Fail("Exception not thrown.");
    }
}