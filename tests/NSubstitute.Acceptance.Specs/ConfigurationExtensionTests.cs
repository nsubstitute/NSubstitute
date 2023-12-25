using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Core;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class ConfigurationExtensionTests
{
    public class TypeWithThrowingMembers
    {
        public virtual int GetValue() => throw new NotImplementedException();
        public virtual int Value => throw new NotImplementedException();
    }

    public class ThrowingCallHandler : ICallHandler
    {
        public RouteAction Handle(ICall call) => throw new NotSupportedException();
    }

    [Test]
    public void Should_not_return_the_previously_configured_result()
    {
        // Arrange
        var mock = Substitute.For<ISomething>();

        // Act
        mock.Echo(Arg.Any<int>()).Returns("1", "2", "3");
        mock.Configure().Echo(42).Returns("42");

        // Assert
        Assert.That(mock.Echo(10), Is.EqualTo("1"));
        Assert.That(mock.Echo(10), Is.EqualTo("2"));
        Assert.That(mock.Echo(42), Is.EqualTo("42"));
    }

    [Test]
    public void Should_be_possible_to_reconfigure_configured_throwing_calls()
    {
        // Arrange
        var mock = Substitute.For<ISomething>();

        // Act
        mock.Echo(Arg.Any<int>()).Throws<InvalidOperationException>();
        mock.Configure().Echo(42).Returns("42");

        // Assert
        Assert.That(mock.Echo(42), Is.EqualTo("42"));
    }

    [Test]
    public void Should_be_possible_to_configure_base_throwing_method()
    {
        // Arrange
        var mock = Substitute.ForPartsOf<TypeWithThrowingMembers>();

        // Act
        mock.Configure().GetValue().Returns(42);

        // Assert
        Assert.That(mock.GetValue(), Is.EqualTo(42));
    }

    [Test]
    public void Should_be_possible_to_configure_base_throwing_property()
    {
        // Arrange
        var mock = Substitute.ForPartsOf<TypeWithThrowingMembers>();

        // Act
        mock.Configure().Value.Returns(42);

        // Assert
        Assert.That(mock.Value, Is.EqualTo(42));
    }

    [Test]
    public void Should_be_possible_to_disable_custom_call_handler_for_specification_call()
    {
        // Arrange
        var mock = Substitute.For<ISomething>();

        var callRouter = SubstitutionContext.Current.GetCallRouterFor(mock);
        callRouter.RegisterCustomCallHandlerFactory(_ => new ThrowingCallHandler());

        // Act
        mock.Configure().Count().Returns(42);

        // Assert
        Assert.That(mock.Count(), Is.EqualTo(42));
    }
}