using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class OutAndRefParameters
    {
        public interface ILookupStrings
        {
            bool TryRef(ref int number);
            bool TryGet(string key, out string value);
            string Get(string key);
            string LookupByObject(ref object obj);
        }

        [Test]
        [Pending]
        public void SetNonOutputArgument()
        {
            var sub = Substitute.For<ILookupStrings>();
            var key = "hi";
            sub.Get(key).Returns("hello");
            sub.When(x => x.Get(key)).Do(x => { x[0] = "snarf!"; });

            var result = sub.Get("hi");

            Assert.That(key, Is.EqualTo("hi"));
            Assert.That(result, Is.EqualTo("hello"));
        }

        [Test]
        [Pending]
        public void SetOutputArgument()
        {
            var sub = Substitute.For<ILookupStrings>();
            string value;
            sub.TryGet("hi", out value).Returns(x => { x[1] = "hello"; return true; });

            var result = sub.TryGet("hi", out value);

            Assert.That(value, Is.EqualTo("hello"));
            Assert.That(result, Is.True);
        }

        [Test]
        public void MatchIntRefArgument()
        {
            var sub = Substitute.For<ILookupStrings>();
            var value = 1;
            var otherValue = 1;
            sub.TryRef(ref value);
            sub.DidNotReceive().TryRef(ref otherValue);
        }

        [Test]
        public void MatchObjectRefArgument()
        {
            var sub = Substitute.For<ILookupStrings>();
            var value = new object();
            var otherValue = new object();
            sub.LookupByObject(ref value);
            sub.DidNotReceive().LookupByObject(ref otherValue);
        }
    }
}