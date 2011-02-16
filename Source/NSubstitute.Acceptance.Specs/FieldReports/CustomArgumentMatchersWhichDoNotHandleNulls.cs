using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class CustomArgumentMatchersWhichDoNotHandleNulls
    {
        public interface IRequest { string Get(string url); }

        [Test]
        [Pending]
        public void Condition_that_requires_object_reference()
        {
            var request = Substitute.For<IRequest>();
            request.Get(Arg.Is<string>(x => x.Contains("greeting"))).Returns("hello world");
            request.Get(Arg.Is<string>(x => string.IsNullOrEmpty(x))).Returns("?");

            Assert.That(request.Get("greeting"), Is.EqualTo("hello world"));
            Assert.That(request.Get(""), Is.EqualTo("?"));
            Assert.That(request.Get(null), Is.EqualTo("?"));
        }

        [Test]
        [Pending]
        public void Negated_condition_that_requires_object_reference()
        {
            var request = Substitute.For<IRequest>();
            request.Get(Arg.Is<string>(x => !x.StartsWith("please"))).Returns("ask nicely");
            request.Get(Arg.Is<string>(x => x.Length > 10)).Returns("request too long");

            Assert.That(request.Get("greeting"), Is.EqualTo("ask nicely"));
            Assert.That(request.Get("please provide me with a greeting"), Is.EqualTo("request too long"));
            Assert.That(request.Get(null), Is.EqualTo(""));
        }
    }
}