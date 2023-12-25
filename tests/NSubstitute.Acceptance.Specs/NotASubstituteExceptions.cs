using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class NotASubstituteExceptions
{
    object _notASub;

    [SetUp]
    public void SetUp()
    {
        _notASub = new object();
    }

    [Test]
    public void Should_describe_how_exception_can_occur()
    {
        Assert.That(
            () => _notASub.Received(),
            Throws
                .TypeOf<NotASubstituteException>()
            .And
                .Message.EqualTo(
                    "NSubstitute extension methods like .Received() can only be called on objects created using Substitute.For<T>() and related methods."
                )
        );
    }

    [Test]
    public void Calling_received_on_a_non_sub()
    {
        AssertThrowsNotASubstituteException(() => _notASub.Received());
    }

    [Test]
    public void Calling_when_on_a_non_sub()
    {
        AssertThrowsNotASubstituteException(() => _notASub.When(x => x.GetHashCode()));
    }


    private static void AssertThrowsNotASubstituteException(Action action)
    {
        Assert.That(() => action(), Throws.TypeOf<NotASubstituteException>());
    }
}