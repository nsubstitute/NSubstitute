using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class NotASubstituteExceptions
    {
        [Test]
        public void Should_describe_how_exception_can_occur()
        {
            var notASub = new object();
            Assert.That(
                () => notASub.Received(),
                Throws
                    .TypeOf<NotASubstituteException>()
                .And
                    .Message.EqualTo(
                        "NSubstitute extension methods like .Received() can only be called on objects created using Substitute.For<T>() and related methods."
                    )
            );
        }
    }
}