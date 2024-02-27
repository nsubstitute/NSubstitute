using NSubstitute.Acceptance.Specs.Infrastructure;
using NUnit.Framework;
using NUnit.Framework.Legacy;

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
        ClassicAssert.AreEqual("00", target.Echo(0));
        ClassicAssert.AreEqual("10", target.Echo(1));
        ClassicAssert.AreEqual("01", target.Echo(0));
        ClassicAssert.AreEqual("11", target.Echo(1));
        ClassicAssert.AreEqual("02", target.Echo(0));
        ClassicAssert.AreEqual("12", target.Echo(1));
    }
}