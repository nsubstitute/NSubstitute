using NSubstitute.Acceptance.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
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
            Assert.AreEqual("00", target.Echo(0));
            Assert.AreEqual("10", target.Echo(1));
            Assert.AreEqual("01", target.Echo(0));
            Assert.AreEqual("11", target.Echo(1));
            Assert.AreEqual("02", target.Echo(0));
            Assert.AreEqual("12", target.Echo(1));
        }
    }
}