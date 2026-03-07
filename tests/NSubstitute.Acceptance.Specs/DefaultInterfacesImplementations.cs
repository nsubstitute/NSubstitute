using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSubstitute.Acceptance.Specs;

#if NET8_0_OR_GREATER

[TestFixture]
public class DefaultInterfacesImplementations
{
    [Test]
    public void DefaultInterface_SetupReturns_UsesReturns()
    {
        // Arrange
        var for1 = Substitute.For<IMyInterface>();
        for1.GetText().Returns("new text");

        // Act
        var result = for1.GetText();

        // Assert
        Assert.That(result, Is.EqualTo("new text"));
    }

    public interface IMyInterface
    {
        string GetText()
        {
            return "default text";
        }
    }
}
#endif