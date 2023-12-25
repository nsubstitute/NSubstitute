using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue569_QueryShouldNotInvokeConfiguredResult
{
    [Test]
    public void Should_not_invoke_configured_handler_in_query_original()
    {
        // Arrange
        var substitute = Substitute.For<Func<int>>();
        substitute.Invoke().Returns(x => throw new Exception("SOME EXCEPTION"));

        // Act
        try
        {
            substitute.Invoke();
            Assert.Fail();
        }
        catch (Exception)
        {
            // ignored
        }

        // Assert
        Assert.DoesNotThrow(() =>
        {
            Received.InOrder(() =>
            {
                substitute.Invoke();
            });
        });
    }

    [Test]
    public void Should_not_throw_configured_exception_in_query()
    {
        // Arrange
        var substitute = Substitute.For<ISomething>();
        substitute.Echo(Arg.Any<int>()).Throws<InvalidOperationException>();
        try
        {
            substitute.Echo(42);
        }
        catch (InvalidOperationException)
        {
            // Ignore
        }

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            Received.InOrder(() =>
            {
                substitute.Echo(Arg.Any<int>());
            });
        });
    }

    [Test]
    public void Should_not_invoke_configured_argument_actions_in_query()
    {
        // Arrange
        var substitute = Substitute.For<ISomething>();
        bool wasCalled = false;
        substitute.Echo(Arg.Do<int>(_ => wasCalled = true));
        substitute.Echo(42);

        // Act
        wasCalled = false;
        Received.InOrder(() =>
        {
            substitute.Echo(42);
        });

        // Assert
        Assert.That(wasCalled, Is.False);
    }
}