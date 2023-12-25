using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue533_TaskExceptionMightBeSwallowed
{
    [Test]
    public void ClientScenario_ShouldFailWithRedundantArgMatcherException()
    {
        var substitute = Substitute.For<IMainModule>();

        Assert.Throws<RedundantArgumentMatcherException>(() =>
            MainModuleExtensions
                .Work(substitute, Arg.Any<Guid>(), Arg.Any<string>())
                .Returns(Guid.Empty)
        );
    }

    [Test]
    public void ShouldRethrowOriginalSubstituteException_TaskValue()
    {
        var substitute = Substitute.For<ISomething>();

        substitute.Say("42"); // If task exception is not unwrapped, then type mismatch exception will be throw.
        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            Arg.Any<int>();
            InvokeEchoAsync(substitute, Arg.Any<int>()).Returns("42");
        });
    }

    [Test]
    public void ShouldRethrowOriginalSubstituteException_TaskValueForAny()
    {
        var substitute = Substitute.For<ISomething>();

        substitute.Say("42"); // If task exception is not unwrapped, then type mismatch exception will be throw.
        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            Arg.Any<int>();
            InvokeEchoAsync(substitute, Arg.Any<int>()).ReturnsForAnyArgs("42");
        });
    }

    [Test]
    public void ShouldRethrowOriginalSubstituteException_TaskCallback()
    {
        var substitute = Substitute.For<ISomething>();

        substitute.Say("42"); // If task exception is not unwrapped, then type mismatch exception will be throw.
        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            Arg.Any<int>();
            InvokeEchoAsync(substitute, Arg.Any<int>()).Returns(c => "42");
        });
    }

    [Test]
    public void ShouldRethrowOriginalSubstituteException_TaskCallbackForAny()
    {
        var substitute = Substitute.For<ISomething>();

        substitute.Say("42"); // If task exception is not unwrapped, then type mismatch exception will be throw.
        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            Arg.Any<int>();
            InvokeEchoAsync(substitute, Arg.Any<int>()).ReturnsForAnyArgs(c => "42");
        });
    }

    [Test]
    public void ShouldRethrowOriginalSubstituteException_ValueTaskValue()
    {
        var substitute = Substitute.For<ISomething>();

        substitute.Say("42"); // If task exception is not unwrapped, then type mismatch exception will be throw.
        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            Arg.Any<int>();
            InvokeEchoValueTaskAsync(substitute, Arg.Any<int>()).Returns("42");
        });
    }

    [Test]
    public void ShouldRethrowOriginalSubstituteException_ValueTaskValueForAny()
    {
        var substitute = Substitute.For<ISomething>();

        substitute.Say("42"); // If task exception is not unwrapped, then type mismatch exception will be throw.
        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            Arg.Any<int>();
            InvokeEchoValueTaskAsync(substitute, Arg.Any<int>()).ReturnsForAnyArgs("42");
        });
    }

    [Test]
    public void ShouldRethrowOriginalSubstituteException_ValueTaskCallback()
    {
        var substitute = Substitute.For<ISomething>();

        substitute.Say("42"); // If task exception is not unwrapped, then type mismatch exception will be throw.
        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            Arg.Any<int>();
            InvokeEchoValueTaskAsync(substitute, Arg.Any<int>()).Returns(c => "42");
        });
    }

    [Test]
    public void ShouldRethrowOriginalSubstituteException_ValueTaskCallbackForAny()
    {
        var substitute = Substitute.For<ISomething>();

        substitute.Say("42"); // If task exception is not unwrapped, then type mismatch exception will be throw.
        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            Arg.Any<int>();
            InvokeEchoValueTaskAsync(substitute, Arg.Any<int>()).ReturnsForAnyArgs(c => "42");
        });
    }

    [Test]
    public void ShouldSwallowCustomExceptionThrownFromTask_TaskValue()
    {
        var substitute = Substitute.ForPartsOf<ClassWithThrowingMethods>();

        Assert.DoesNotThrow(() =>
            substitute.GetValue().Returns(42));
    }

    [Test]
    public void ShouldSwallowCustomExceptionThrownFromTask_TaskValueAnyArg()
    {
        var substitute = Substitute.ForPartsOf<ClassWithThrowingMethods>();

        Assert.DoesNotThrow(() =>
            substitute.GetValue().ReturnsForAnyArgs(42));
    }

    [Test]
    public void ShouldSwallowCustomExceptionThrownFromTask_TaskCallback()
    {
        var substitute = Substitute.ForPartsOf<ClassWithThrowingMethods>();

        Assert.DoesNotThrow(() =>
            substitute.GetValue().Returns(c => 42));
    }

    [Test]
    public void ShouldSwallowCustomExceptionThrownFromTask_TaskCallbackAnyArg()
    {
        var substitute = Substitute.ForPartsOf<ClassWithThrowingMethods>();

        Assert.DoesNotThrow(() =>
            substitute.GetValue().ReturnsForAnyArgs(c => 42));
    }

    [Test]
    public void ShouldSwallowCustomExceptionThrownFromTask_ValueTaskValue()
    {
        var substitute = Substitute.ForPartsOf<ClassWithThrowingMethods>();

        Assert.DoesNotThrow(() =>
            substitute.GetValueValueTask().Returns(42));
    }

    [Test]
    public void ShouldSwallowCustomExceptionThrownFromTask_ValueTaskValueAnyArg()
    {
        var substitute = Substitute.ForPartsOf<ClassWithThrowingMethods>();

        Assert.DoesNotThrow(() =>
            substitute.GetValueValueTask().ReturnsForAnyArgs(42));
    }

    [Test]
    public void ShouldSwallowCustomExceptionThrownFromTask_ValueTaskCallback()
    {
        var substitute = Substitute.ForPartsOf<ClassWithThrowingMethods>();

        Assert.DoesNotThrow(() =>
            substitute.GetValueValueTask().Returns(c => 42));
    }

    [Test]
    public void ShouldSwallowCustomExceptionThrownFromTask_ValueTaskCallbackAnyArg()
    {
        var substitute = Substitute.ForPartsOf<ClassWithThrowingMethods>();

        Assert.DoesNotThrow(() =>
            substitute.GetValueValueTask().ReturnsForAnyArgs(c => 42));
    }

    private static async Task<string> InvokeEchoAsync(ISomething something, int value)
    {
        return await something.EchoAsync(value);
    }

    private static async ValueTask<string> InvokeEchoValueTaskAsync(ISomething something, int value)
    {
        return await something.EchoValueTaskAsync(value);
    }

    public interface IMainModule
    {
        IWorker Worker { get; }
    }

    public interface IWorker
    {
        Task DoSomethingAsync(Guid userId);
    }

    private static class MainModuleExtensions
    {
        public static async Task<Guid> Work(
            /* this - extensions are not supported for nested types */ IMainModule mainModule,
            Guid userId, string s)
        {
            await mainModule.Worker.DoSomethingAsync(userId);
            return Guid.NewGuid();
        }
    }

    public class ClassWithThrowingMethods
    {
#pragma warning disable 1998 // Async is required here to wrap exception in task.
        public virtual async Task<int> GetValue() => throw new NotImplementedException();
        public virtual async ValueTask<int> GetValueValueTask() => throw new NotImplementedException();
#pragma warning restore 1998
    }
}