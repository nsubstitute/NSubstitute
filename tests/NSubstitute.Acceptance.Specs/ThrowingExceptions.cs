using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class ThrowingExceptions
{
    private ISomething _something;

    [Test]
    public void ThrowException()
    {
        _something.Count().Throws(new Exception());

        Assert.Catch<Exception>(() => _something.Count());
    }

    [Test]
    public void ThrowOtherException()
    {
        _something.Count().Throws(new InvalidOperationException());

        Assert.Catch<InvalidOperationException>(() => _something.Count());
    }

    [Test]
    public void ThrowExceptionWithDefaultConstructor()
    {
        _something.Count().Throws<ArgumentException>();

        Assert.Catch<ArgumentException>(() => _something.Count());
    }

    [Test]
    public void ThrowExceptionWithMessage()
    {
        const string exceptionMessage = "This is exception's message";

        _something.Count().Throws(new Exception(exceptionMessage));

        Exception exceptionThrown = Assert.Catch<Exception>(() => _something.Count());
        Assert.That(exceptionThrown.Message, Is.EqualTo(exceptionMessage));
    }

    [Test]
    public void ThrowExceptionWithInnerException()
    {
        ArgumentException innerException = new ArgumentException();
        _something.Count().Throws(new Exception("Exception message", innerException));

        Exception exceptionThrown = Assert.Catch<Exception>(() => _something.Count());

        Assert.That(exceptionThrown.InnerException, Is.Not.Null);
        Assert.That(exceptionThrown.InnerException, Is.InstanceOf<ArgumentException>());
    }

    [Test]
    public void ThrowExceptionUsingFactoryFunc()
    {
        _something.Anything(null).Throws(ci => new ArgumentException("Args:" + ci.Args()[0]));

        Assert.Catch<ArgumentException>(() => _something.Anything(null));
    }

    [Test]
    public void DoesNotThrowForNonMatchingArgs()
    {
        _something.Anything(12).Throws(new Exception());

        Assert.Catch<Exception>(() => _something.Anything(12));
        Assert.DoesNotThrow(() => _something.Anything(11));
    }

    [Test]
    public void ThrowExceptionForAnyArgs()
    {
        _something.Anything(12).ThrowsForAnyArgs(new Exception());

        Assert.Catch<Exception>(() => _something.Anything(null));
        Assert.Catch<Exception>(() => _something.Anything(12));
    }

    [Test]
    public void ThrowExceptionWithDefaultConstructorForAnyArgs()
    {
        _something.Anything(12).ThrowsForAnyArgs<InvalidOperationException>();

        Assert.Catch<InvalidOperationException>(() => _something.Anything(new object()));
    }

    [Test]
    public void ThrowExceptionCreatedByFactoryFuncForAnyArgs()
    {
        _something.Anything(null).ThrowsForAnyArgs(ci => new ArgumentException("Args:" + ci.Args()[0]));

        Assert.Catch<ArgumentException>(() => _something.Anything(new object()));
    }

    [SetUp]
    public void SetUp()
    {
        _something = Substitute.For<ISomething>();
    }

    [TearDown]
    public void TearDown()
    {
        _something = null;
    }
}
