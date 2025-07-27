using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue291_CannotReconfigureThrowingConfiguration
{
    // Based on: https://stackoverflow.com/q/42686269/906

    public interface IRequest { }
    public interface IResponse { }
    public interface IDeliver { IResponse Send(IRequest msg); }

    public class Message1 : IRequest { }
    public class Message2 : IRequest { }
    public class Response : IResponse { }

    [Test]
    public void ShouldBePossibleToReConfigureThrowingConfiguration()
    {
        // Arrange
        var response = new Response();
        var deliver = Substitute.For<IDeliver>();

        // Act
        deliver.Send(Arg.Any<Message1>()).Throws<InvalidOperationException>();
        deliver.Send(Arg.Any<Message2>()).Returns(response);

        // Assert
        Assert.Throws<InvalidOperationException>(() => deliver.Send(new Message1()));
        Assert.That(deliver.Send(new Message2()), Is.SameAs(response));
    }

    [Test]
    public void ShouldBePossibleToConfigureConstantAfterThrowForAny()
    {
        // Arrange
        var something = Substitute.For<ISomething>();

        // Act
        something.Echo(Arg.Any<int>()).Throws<InvalidOperationException>();
        something.Echo(Arg.Is(42)).Returns("42");

        // Assert
        Assert.Throws<InvalidOperationException>(() => something.Echo(100));
        Assert.That(something.Echo(42), Is.EqualTo("42"));
    }
}