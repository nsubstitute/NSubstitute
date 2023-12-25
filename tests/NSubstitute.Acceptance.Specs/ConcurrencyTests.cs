using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Exceptions;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class ConcurrencyTests
{
    [Test]
    public async Task Call_between_invocation_and_received_does_not_cause_issue()
    {
        // Arrange
        var subs = Substitute.For<ISomething>();

        var backgroundReady = new AutoResetEvent(false);

        // Act
        // 1
        var dummy = subs.Say("ping");

        var bg = RunInOtherThread(() =>
        {
            // 2
            subs.Echo(42);
            backgroundReady.Set();
        });

        backgroundReady.WaitOne();

        // 3
        dummy.Returns("pong");

        // Assert
        var actualResult = subs.Say("ping");

        Assert.That(actualResult, Is.EqualTo("pong"));
        await bg;
    }

    [Test]
    public async Task Background_invocation_does_not_delete_specification()
    {
        // Arrange
        var subs = Substitute.For<ISomething>();

        var backgroundReady = new AutoResetEvent(false);

        // Act
        // 1
        var dummy = subs.Say(Arg.Any<string>());

        var bg = RunInOtherThread(() =>
        {
            // 2
            subs.Say("hello");
            backgroundReady.Set();
        });

        backgroundReady.WaitOne();

        // 3
        dummy.Returns("42");

        // Assert
        Assert.That(subs.Say("Alex"), Is.EqualTo("42"));
        await bg;
    }

    [Test]
    public async Task Both_threads_can_configure_returns_concurrently()
    {
        // Arrange
        var subs = Substitute.For<ISomething>();

        var foregroundReady = new AutoResetEvent(false);
        var backgroundReady = new AutoResetEvent(false);

        // Act
        // 1
        var dummy = subs.Say("ping");

        var bg = RunInOtherThread(() =>
        {
            // 2
            var d = subs.Echo(42);
            SignalAndWait(backgroundReady, foregroundReady);

            // 4
            d.Returns("42");
            backgroundReady.Set();
        });

        backgroundReady.WaitOne();

        // 3
        dummy.Returns("pong");
        SignalAndWait(foregroundReady, backgroundReady);

        // Assert
        Assert.That(subs.Say("ping"), Is.EqualTo("pong"));
        Assert.That(subs.Echo(42), Is.EqualTo("42"));
        await bg;
    }

    [Test]
    public async Task Both_threads_can_verify_received_calls_concurrently()
    {
        // arrange
        var subs = Substitute.For<ISomething>();

        var foregroundReady = new AutoResetEvent(false);
        var backgroundReady = new AutoResetEvent(false);

        // act
        // 1
        subs.Add(1, 2);
        subs.Echo(42);

        subs.Received();

        var bg = RunInOtherThread(() =>
        {
            // 2
            subs.Received();
            SignalAndWait(backgroundReady, foregroundReady);

            // 4
            // Make exceptional situation, as otherwise it isn't clear whether call is actually verified.
            Assert.Throws<ReceivedCallsException>(() => subs.Echo(99)); // This call is checked for being received.
        });

        backgroundReady.WaitOne();

        // 3
        subs.Add(1, 2); // This call is checked for being received.
        foregroundReady.Set();

        await bg;
    }

    [Test]
    public async Task Both_threads_can_verify_received_calls_with_any_args_concurrently()
    {
        // arrange
        var subs = Substitute.For<ISomething>();

        var foregroundReady = new AutoResetEvent(false);
        var backgroundReady = new AutoResetEvent(false);

        // act
        // 1
        subs.Add(1, 2);
        subs.Echo(42);

        subs.ReceivedWithAnyArgs();

        var bg = RunInOtherThread(() =>
        {
            // 2
            subs.DidNotReceiveWithAnyArgs();
            SignalAndWait(backgroundReady, foregroundReady);

            // 4
            // Make exceptional situation, as otherwise it isn't clear whether call is actually verified.
            Assert.Throws<ReceivedCallsException>(() => subs.Echo(default)); // This call is checked for being received.
        });

        backgroundReady.WaitOne();

        // 3
        subs.Add(default, default); // This call is checked for being received.
        foregroundReady.Set();

        await bg;
    }

    [Test]
    public async Task Can_configure_using_Configure_method_concurrently()
    {
        // Arrange
        var subs = Substitute.ForPartsOf<TypeWhichThrows>();

        var foregroundReady = new AutoResetEvent(false);
        var backgroundReady = new AutoResetEvent(false);

        // Act
        // 1
        subs.Configure();

        var bg = RunInOtherThread(() =>
        {
            // 2
            subs.Configure();
            SignalAndWait(backgroundReady, foregroundReady);

            // 4
            subs.EchoMethodB(42).Returns(42);
            backgroundReady.Set();
        });

        backgroundReady.WaitOne();

        // 3
        subs.EchoMethodA(42).Returns(42);
        SignalAndWait(foregroundReady, backgroundReady);

        // Assert
        // 5
        var resultA = subs.EchoMethodA(42);
        var resultB = subs.EchoMethodB(42);
        Assert.That(resultA, Is.EqualTo(42));
        Assert.That(resultB, Is.EqualTo(42));
        await bg;
    }

    [Test]
    public async Task Configure_in_one_thread_should_not_affect_substitute_in_other_thread()
    {
        // Arrange
        var subs = Substitute.For<ISomething>();
        var backgroundReady = new AutoResetEvent(false);

        // Act
        // 1
        subs.Echo(42).Returns("42");

        var bg = RunInOtherThread(() =>
        {
            // 2
            subs.Configure();
            backgroundReady.Set();
        });

        backgroundReady.WaitOne();
        // 3
        var result = subs.Echo(42);

        // Assert
        Assert.That(result, Is.EqualTo("42"));
        await bg;
    }

    [Test]
    public async Task Configuration_works_fine_for_async_methods()
    {
        // Arrange
        var subs = Substitute.For<ISomething>();

        // Act
        subs.EchoAsync(42).Returns("42");

        // Assert
        var result = await subs.EchoAsync(42);
        Assert.That(result, Is.EqualTo("42"));
    }

    private static Task RunInOtherThread(Action action)
    {
        return Task.Factory.StartNew(action);
    }

    private static void SignalAndWait(EventWaitHandle toSignal, EventWaitHandle toWait)
    {
        toSignal.Set();
        toWait.WaitOne();
    }

    public class TypeWhichThrows
    {
        public virtual int EchoMethodA(int value) => throw new NotImplementedException();
        public virtual int EchoMethodB(int value) => throw new NotImplementedException();
    }
}