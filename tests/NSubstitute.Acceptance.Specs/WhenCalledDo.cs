using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Core;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class WhenCalledDo
{
    private ISomething _something;


    [Test]
    public void Execute_when_called_async()
    {
        var called = false;
        _something.When(substitute => substitute.Echo(1)).Do(async info =>
        {
            await Task.Delay(100);
            called = true;
        });

        Assert.That(called, Is.False, "Called");
        _something.Echo(1);
        Assert.That(called, Is.True, "Called");
    }

    [Test]
    public void Execute_when_called()
    {
        var called = false;
        _something.When(substitute => substitute.Echo(1)).Do(info =>
        {
            Thread.Sleep(100);
            called = true;
        });

        Assert.That(called, Is.False, "Called");
        _something.Echo(1);
        Assert.That(called, Is.True, "Called");
    }

    [Test]
    public void Capture_arguments_when_called()
    {
        int firstArgument = 0;
        _something.When(substitute => substitute.Echo(1)).Do(info => firstArgument = (int)info[0]);

        Assert.That(firstArgument, Is.EqualTo(0), "firstArgument");
        _something.Echo(1);
        Assert.That(firstArgument, Is.EqualTo(1), "firstArgument");
    }

    [Test]
    public void Run_multiple_actions_when_called()
    {
        int called = 0;
        _something.When(x => x.Echo(Arg.Any<int>())).Do(x => called++);
        _something.When(x => x.Echo(4)).Do(x => called++);
        _something.WhenForAnyArgs(x => x.Echo(1234)).Do(x => called++);

        Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
        _something.Echo(4);
        Assert.That(called, Is.EqualTo(3));
    }

    [Test]
    public void Only_do_matching_actions()
    {
        int called = 0;
        _something.When(x => x.Echo(Arg.Any<int>())).Do(x => called++);
        _something.When(x => x.Echo(4)).Do(x => called++);

        Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
        _something.Echo(1);
        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public void Execute_when_called_for_any_args()
    {
        var called = false;
        _something.WhenForAnyArgs(x => x.Echo(1)).Do(x => called = true);

        Assert.That(called, Is.False, "Called");
        _something.Echo(1234);
        Assert.That(called, Is.True, "Called");
    }

    [Test]
    public void Throw_exception_when_Throw_with_generic_exception()
    {
        int called = 0;
        _something.When(x => x.Echo(Arg.Any<int>())).Do(x => called++);
        var expectedException = _something.When(x => x.Echo(Arg.Any<int>())).Throw<ArgumentException>();

        Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
        ArgumentException actualException = Assert.Throws<ArgumentException>(() => _something.Echo(1234));
        Assert.That(actualException, Is.EqualTo(expectedException));
        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public void Throw_exception_when_Throws_with_generic_exception()
    {
        int called = 0;
        _something.When(x => x.Echo(Arg.Any<int>())).Do(x => called++);
        var expectedException = _something.When(x => x.Echo(Arg.Any<int>())).Throws<ArgumentException>();

        Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
        ArgumentException actualException = Assert.Throws<ArgumentException>(() => _something.Echo(1234));
        Assert.That(actualException, Is.EqualTo(expectedException));
        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public void Throw_exception_when_Throw_with_specific_exception()
    {
        var exception = new IndexOutOfRangeException("Test");
        int called = 0;
        _something.When(x => x.Echo(Arg.Any<int>())).Do(x => called++);
        _something.When(x => x.Echo(Arg.Any<int>())).Throw(exception);

        Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
        IndexOutOfRangeException thrownException = Assert.Throws<IndexOutOfRangeException>(() => _something.Echo(1234));
        Assert.That(thrownException, Is.EqualTo(exception));
        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public void Throw_exception_when_Throws_with_specific_exception()
    {
        var exception = new IndexOutOfRangeException("Test");
        int called = 0;
        _something.When(x => x.Echo(Arg.Any<int>())).Do(x => called++);
        _something.When(x => x.Echo(Arg.Any<int>())).Throws(exception);

        Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
        IndexOutOfRangeException thrownException = Assert.Throws<IndexOutOfRangeException>(() => _something.Echo(1234));
        Assert.That(thrownException, Is.EqualTo(exception));
        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public void Throw_exception_when_Throw_with_exception_generator()
    {
        Func<CallInfo, Exception> createException = ci => new ArgumentException("Argument: " + ci.Args()[0]);
        int called = 0;
        _something.When(x => x.Echo(Arg.Any<int>())).Do(x => called++);
        _something.When(x => x.Echo(Arg.Any<int>())).Throw(createException);

        Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
        ArgumentException thrownException = Assert.Throws<ArgumentException>(() => _something.Echo(1234));
        Assert.That(thrownException.Message, Is.EqualTo("Argument: 1234"));
        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public void Throw_exception_when_Throws_with_exception_generator()
    {
        Func<CallInfo, Exception> createException = ci => new ArgumentException("Argument: " + ci.Args()[0]);
        int called = 0;
        _something.When(x => x.Echo(Arg.Any<int>())).Do(x => called++);
        _something.When(x => x.Echo(Arg.Any<int>())).Throws(createException);

        Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
        ArgumentException thrownException = Assert.Throws<ArgumentException>(() => _something.Echo(1234));
        Assert.That(thrownException.Message, Is.EqualTo("Argument: 1234"));
        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public async Task Can_configure_async_methods_nicely()
    {
        int called = 0;
        _something.When(x => x.SayAsync(Arg.Any<string>())).Do(c => called++);

        await _something.SayAsync("Boo");

        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public async Task Can_configure_async_valueTask_methods_nicely()
    {
        int called = 0;
        _something.When(x => x.SayValueTaskAsync(Arg.Any<string>())).Do(c => called++);

        await _something.SayValueTaskAsync("Boo");

        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public async Task Can_configure_async_methods_for_any_args_nicely()
    {
        int called = 0;
        _something.WhenForAnyArgs(x => x.SayAsync(default)).Do(c => called++);

        await _something.SayAsync("Boo");

        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public async Task Can_configure_async_valueTask_methods_for_any_args_nicely()
    {
        int called = 0;
        _something.WhenForAnyArgs(x => x.SayValueTaskAsync(default)).Do(c => called++);

        await _something.SayValueTaskAsync("Boo");

        Assert.That(called, Is.EqualTo(1));
    }

    [SetUp]
    public void SetUp()
    {
        _something = Substitute.For<ISomething>();
    }
}