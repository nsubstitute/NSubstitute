using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class OutAndRefParameters
    {
        public interface ILookupStrings
        {
            bool TryGet(string key, out string value);
            string Get(string key);
            string LookupByObject(ref object obj);
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

        [Test]
        [Pending]
        public void Match_and_set_ref_argument()
        {
            var sub = Substitute.For<ILookupStrings>();
            var key = new object();
            sub.LookupByObject(ref key).Returns(x => { x[0] = new object(); return "1"; });

            Assert.That(sub.LookupByObject(ref key), Is.EqualTo("1"));
            Assert.That(sub.LookupByObject(ref key), Is.EqualTo("1"));
        }

        [Test]
        public void Match_int_ref_argument()
        {
            var sub = Substitute.For<ILookupStrings>();
            var value = 1;
            var otherValue = 1;
            sub.TryRef(ref value);
            sub.DidNotReceive().TryRef(ref otherValue);
        }

        [Test]
        public void Match_object_ref_argument()
        {
            var sub = Substitute.For<ILookupStrings>();
            var value = new object();
            var otherValue = new object();
            sub.LookupByObject(ref value);
            sub.DidNotReceive().LookupByObject(ref otherValue);
        }
    }
}
