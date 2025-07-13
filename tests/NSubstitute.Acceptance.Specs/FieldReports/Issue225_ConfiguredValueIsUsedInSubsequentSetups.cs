using NSubstitute.Acceptance.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue225_ConfiguredValueIsUsedInSubsequentSetups
{
    [Test]
    public void ShouldNotUseTheConfiguredValueDuringSubsequentSetup()
    {
        // Arrange
        var target = Substitute.For<ISomething>();

        // Act
        target.Echo(Arg.Is(0)).Returns("00", "01", "02");
        target.Echo(Arg.Is(1)).Returns("10", "11", "12");

        // Assert
        Assert.That(target.Echo(0), Is.EqualTo("00"));
        Assert.That(target.Echo(1), Is.EqualTo("10"));
        Assert.That(target.Echo(0), Is.EqualTo("01"));
        Assert.That(target.Echo(1), Is.EqualTo("11"));
        Assert.That(target.Echo(0), Is.EqualTo("02"));
        Assert.That(target.Echo(1), Is.EqualTo("12"));
    }
}