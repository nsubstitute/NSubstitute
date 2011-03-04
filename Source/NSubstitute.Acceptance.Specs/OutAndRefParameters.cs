using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class OutAndRefParameters
    {
        public interface ILookupStrings
        {
            bool TryGet(string key, out string value);
        }

        [Test]
        [Pending]
        public void SetOutputArgument()
        {
            var sub = Substitute.For<ILookupStrings>();
            string value;
            //sub.TryGet("hi", out value).Returns(x => { x[1] = "hello"; return true; });

            var result = sub.TryGet("hi", out value);

            Assert.That(value, Is.EqualTo("hello"));
            Assert.That(result, Is.True);
        }
    }
}