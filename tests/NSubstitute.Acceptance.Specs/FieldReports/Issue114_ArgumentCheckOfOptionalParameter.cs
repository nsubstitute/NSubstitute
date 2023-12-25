using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue114_ArgumentCheckOfOptionalParameter
{
    public interface IInterface
    {
        void MethodWithOptionalParameter(object obligatory, object optional = null);
    }

    [Test]
    public void PassArgumentCheckForOptionalParameter()
    {
        var substitute = Substitute.For<IInterface>();
        substitute.MethodWithOptionalParameter(new object());
        substitute.Received().MethodWithOptionalParameter(Arg.Any<object>());
        substitute.Received().MethodWithOptionalParameter(Arg.Any<object>(), null);
        substitute.Received().MethodWithOptionalParameter(Arg.Any<object>());
        substitute.ReceivedWithAnyArgs().MethodWithOptionalParameter(null);
        substitute.ReceivedWithAnyArgs().MethodWithOptionalParameter(null, null);
    }

    [Test]
    public void MatchWhenAllArgumentsSpecified()
    {
        var substitute = Substitute.For<IInterface>();
        substitute.MethodWithOptionalParameter(new object(), 2);
        substitute.Received().MethodWithOptionalParameter(Arg.Any<object>(), Arg.Any<object>());
        substitute.Received().MethodWithOptionalParameter(Arg.Any<object>(), 2);
    }

    [Test]
    public void DetectsMismatchedArgs()
    {
        var substitute = Substitute.For<IInterface>();
        substitute.MethodWithOptionalParameter(new object(), 2);
        substitute.DidNotReceive().MethodWithOptionalParameter(Arg.Any<object>());
        substitute.DidNotReceive().MethodWithOptionalParameter(Arg.Any<object>(), 3);
        substitute.DidNotReceive().MethodWithOptionalParameter(Arg.Any<object>(), "asdf");
    }
}