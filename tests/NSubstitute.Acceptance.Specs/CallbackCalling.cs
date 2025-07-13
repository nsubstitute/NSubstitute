using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Core;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class CallbackCalling
{
    private ISomething _something;

    [Test]
    public void Execute_when_called()
    {
        var called = false;
        _something.When(substitute => substitute.Echo(1)).Do(Callback.Always(info => called = true));

        Assert.That(called, Is.False, "Called");
        _something.Echo(1);
        Assert.That(called, Is.True, "Called");
    }

    [Test]
    public void Capture_arguments_when_called()
    {
        int firstArgument = 0;
        _something.When(substitute => substitute.Echo(1)).Do(Callback.Always(info => firstArgument = (int)info[0]));

        Assert.That(firstArgument, Is.EqualTo(0), "firstArgument");
        _something.Echo(1);
        Assert.That(firstArgument, Is.EqualTo(1), "firstArgument");
    }

    [Test]
    public void Run_multiple_actions_when_called()
    {
        int called = 0;
        _something.When(x => x.Echo(Arg.Any<int>())).Do(Callback.Always(x => called++));
        _something.When(x => x.Echo(4)).Do(Callback.Always(x => called++));
        _something.WhenForAnyArgs(x => x.Echo(1234)).Do(Callback.Always(x => called++));

        Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
        _something.Echo(4);
        Assert.That(called, Is.EqualTo(3));
    }

    [Test]
    public void Only_do_matching_actions()
    {
        int called = 0;
        _something.When(x => x.Echo(Arg.Any<int>())).Do(Callback.Always(x => called++));
        _something.When(x => x.Echo(4)).Do(Callback.Always(x => called++));

        Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
        _something.Echo(1);
        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public void Execute_when_called_for_any_args()
    {
        var called = false;
        _something.WhenForAnyArgs(x => x.Echo(1)).Do(Callback.Always(x => called = true));

        Assert.That(called, Is.False, "Called");
        _something.Echo(1234);
        Assert.That(called, Is.True, "Called");
    }

    [Test]
    public void Throw_exception_when_Throw_with_generic_exception()
    {
        int called = 0;
        _something.When(x => x.Echo(Arg.Any<int>())).Do(Callback.Always(x => called++));
        var expectedException = new ArgumentException();
        _something.When(x => x.Echo(Arg.Any<int>())).Do(Callback.FirstThrow(expectedException));

        Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
        var actualException = Assert.Throws<ArgumentException>(() => _something.Echo(1234));
        Assert.That(actualException, Is.EqualTo(expectedException));
        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public void Throw_exception_when_Throw_with_specific_exception()
    {
        var exception = new IndexOutOfRangeException("Test");
        int called = 0;
        _something.When(x => x.Echo(Arg.Any<int>())).Do(Callback.Always(x => called++));
        _something.When(x => x.Echo(Arg.Any<int>())).Do(Callback.FirstThrow(exception));

        Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
        var thrownException = Assert.Throws<IndexOutOfRangeException>(() => _something.Echo(1234));
        Assert.That(thrownException, Is.EqualTo(exception));
        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public void Throw_exception_when_Throw_with_exception_generator()
    {
        Func<CallInfo, Exception> createException = ci => new ArgumentException("Argument: " + ci.Args()[0]);
        int called = 0;
        _something.When(x => x.Echo(Arg.Any<int>())).Do(Callback.Always(x => called++));
        _something.When(x => x.Echo(Arg.Any<int>())).Do(Callback.AlwaysThrow(createException));

        Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
        var thrownException = Assert.Throws<ArgumentException>(() => _something.Echo(1234));
        Assert.That(thrownException.Message, Is.EqualTo("Argument: 1234"));
        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public void FirstIsRunOnce()
    {
        var calls = new List<int>();
        int callCount = 0;
        _something.When(x => x.Count()).Do(Callback.First(x => calls.Add(++callCount)));

        Assert.That(callCount, Is.EqualTo(0), "Should not have been called yet");
        _something.Count();
        _something.Count();
        Assert.That(calls, Is.EqualTo(new List<int> { 1 }));
    }

    [Test]
    public void AlwaysIsCalledAlways()
    {
        var calls = new List<int>();
        int callCount = 0;
        _something.When(x => x.Count()).Do(Callback.Always(x => calls.Add(++callCount)));

        Assert.That(callCount, Is.EqualTo(0), "Should not have been called yet");
        _something.Count();
        _something.Count();
        Assert.That(calls, Is.EqualTo(new List<int> { 1, 2 }));
    }

    [Test]
    public void ThenIsCalledAfterFirst()
    {
        var calls = new List<int>();
        int callCount = 0;
        _something.When(x => x.Count()).Do(
            Callback
                .First(x => calls.Add(++callCount))
                .Then(x => calls.Add(++callCount))
        );

        Assert.That(callCount, Is.EqualTo(0), "Should not have been called yet");
        _something.Count();
        _something.Count();
        Assert.That(calls, Is.EqualTo(new List<int> { 1, 2 }));
    }

    [Test]
    public void TwoThensAreCalledInRightOrderAfterFirst()
    {
        bool first = false;
        bool then = false;
        bool secondThen = false;
        _something.When(x => x.Count()).Do(
            Callback
                .First(x => first = true)
                .Then(x => then = true)
                .Then(x => secondThen = true)
        );

        _something.Count();
        Assert.That(first, Is.True);
        _something.Count();
        Assert.That(then, Is.True);
        _something.Count();
        Assert.That(secondThen, Is.True);
    }

    [Test]
    public void AlwaysIsCalledEvenIfFirstAndThenIsDefined()
    {
        bool first = false;
        bool then = false;
        int callCount = 0;
        _something.When(x => x.Count()).Do(
            Callback
                .First(x => first = true)
                .Then(x => then = true)
                .AndAlways(x => callCount++)
        );

        _something.Count();
        Assert.That(first, Is.True);
        _something.Count();
        Assert.That(then, Is.True);
        _something.Count();
        Assert.That(callCount, Is.EqualTo(3));
    }

    [Test]
    public void ThenIsCalledAfterFirstThrow()
    {
        var exception = new Exception();
        var calls = new List<int>();
        int callCount = 0;
        _something.When(x => x.Count()).Do(
            Callback
                .FirstThrow(exception)
                .Then(x => calls.Add(++callCount))
        );

        Assert.That(callCount, Is.EqualTo(0), "Should not have been called yet");
        var ex = Assert.Throws<Exception>(() => _something.Count());
        Assert.That(ex, Is.SameAs(exception));

        _something.Count();
        Assert.That(calls, Is.EqualTo(new List<int> { 1 }));
    }

    [Test]
    public void ThenIsCalledAfterThenThrow()
    {
        var exception = new Exception();
        var calls = new List<int>();
        int callCount = 0;
        _something.When(x => x.Count()).Do(
            Callback
                .First(info => calls.Add(++callCount))
                .ThenThrow(exception)
                .Then(x => calls.Add(++callCount))
        );

        Assert.That(callCount, Is.EqualTo(0), "Should not have been called yet");
        _something.Count();
        var ex = Assert.Throws<Exception>(() => _something.Count());
        Assert.That(ex, Is.SameAs(exception));

        _something.Count();
        Assert.That(calls, Is.EqualTo(new List<int> { 1, 2 }));
    }

    [Test]
    public void AlwaysIsCalledEvenThrowIfIsDefined()
    {
        var exception = new Exception();
        int callCount = 0;
        _something.When(x => x.Count()).Do(
            Callback
                .FirstThrow(exception)
                .ThenThrow(exception)
                .AndAlways(x => callCount++)
        );

        Assert.Throws<Exception>(() => _something.Count());
        Assert.Throws<Exception>(() => _something.Count());
        Assert.That(callCount, Is.EqualTo(2));
    }

    [Test]
    public void WhenRunOutOfCallbacks()
    {
        var calls = new List<string>();

        _something.When(x => x.Count())
            .Do(
                Callback.First(x => calls.Add("1"))
                        .Then(x => calls.Add("2"))
                        .Then(x => calls.Add("3"))
            );

        for (int i = 0; i < 10; i++)
        {
            _something.Count();
        }
        Assert.That(calls, Is.EqualTo(new[] { "1", "2", "3" }));
    }

    [Test]
    public void WhenToldToKeepDoingSomething()
    {
        var calls = new List<string>();

        _something.When(x => x.Count())
            .Do(
                Callback.First(x => calls.Add("1"))
                        .Then(x => calls.Add("2"))
                        .Then(x => calls.Add("3"))
                        .ThenKeepDoing(x => calls.Add("+"))
            );

        for (int i = 0; i < 5; i++)
        {
            _something.Count();
        }
        Assert.That(calls, Is.EqualTo(new[] { "1", "2", "3", "+", "+" }));
    }

    [Test]
    public void WhenToldToKeepThrowingAfterCallbacks()
    {
        var calls = new List<string>();
        var ex = new Exception();
        _something.When(x => x.Count())
            .Do(
                Callback.First(x => calls.Add("1"))
                        .Then(x => calls.Add("2"))
                        .ThenKeepThrowing(x => ex)
            );

        _something.Count();
        _something.Count();
        Assert.Throws<Exception>(() => _something.Count());
        Assert.Throws<Exception>(() => _something.Count());
        Assert.Throws<Exception>(() => _something.Count());
        Assert.That(calls, Is.EqualTo(new[] { "1", "2" }));
    }

    [Test]
    public void KeepDoingAndAlwaysDo()
    {
        var calls = new List<string>();
        var count = 0;

        _something.When(x => x.Count())
            .Do(
                Callback.First(x => calls.Add("1"))
                        .ThenKeepDoing(x => calls.Add("+"))
                        .AndAlways(x => count++)
            );

        for (int i = 0; i < 5; i++)
        {
            _something.Count();
        }
        Assert.That(calls, Is.EqualTo(new[] { "1", "+", "+", "+", "+" }));
        Assert.That(count, Is.EqualTo(5));
    }

    [SetUp]
    public void SetUp()
    {
        _something = Substitute.For<ISomething>();
    }
}