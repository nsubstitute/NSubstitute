using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue974_ReturnsForGenericMethodWithRelatedReturnTypes
{
    // Based on: https://github.com/nsubstitute/NSubstitute/issues/974

    public interface IResult { }
    public interface IResult<T> : IResult { }

    public interface IRequest<TResponse> { }
    public class NonGenericRequest : IRequest<IResult> { }
    public class GenericRequest : IRequest<IResult<int>> { }

    public interface ISender
    {
        TResponse Send<TResponse>(IRequest<TResponse> request);
    }

    [Test]
    public void ShouldNotThrowWhenConfiguringReturnsForClosedGenericMethodsWithRelatedReturnTypes()
    {
        // Arrange
        var sender = Substitute.For<ISender>();
        var firstResult = Substitute.For<IResult>();
        var secondResult = Substitute.For<IResult<int>>();

        // Act
        sender.Send(Arg.Any<NonGenericRequest>()).Returns(firstResult);
        sender.Send(Arg.Any<GenericRequest>()).Returns(secondResult);

        // Assert
        Assert.That(sender.Send(new NonGenericRequest()), Is.SameAs(firstResult));
        Assert.That(sender.Send(new GenericRequest()), Is.SameAs(secondResult));
    }
}
