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
        public void Match_any_args()
        {
            var sub = Substitute.For<ILookupStrings>();

            var value = 1;
            var otherValue = 1;
            sub.TryRef(ref value);

            sub.ReceivedWithAnyArgs().TryRef(ref otherValue);
        }

        [Test]
        public void Match_any_args_with_when()
        {
            var calledWith = 0;
            var sub = Substitute.For<ILookupStrings>();

            var anyIntRef = 123;
            var intRefToUseForCall = 789;
            sub.WhenForAnyArgs(x => x.TryRef(ref anyIntRef)).Do(x => calledWith = x.Arg<int>());

            sub.TryRef(ref intRefToUseForCall);

            Assert.That(calledWith, Is.EqualTo(intRefToUseForCall));
        }

        [Test]
        [Pending]
        public void Match_int_ref_argument()
        {
            var sub = Substitute.For<ILookupStrings>();
            var value = 1;
            var otherValue = 1;

            sub.TryRef(ref value);

            sub.Received().TryRef(ref value);
            sub.DidNotReceive().TryRef(ref otherValue);
        }

        [Test]
        public void Match_object_ref_argument()
        {
            var sub = Substitute.For<ILookupStrings>();
            var value = new object();
            var otherValue = new object();

            sub.LookupByObject(ref value);

            sub.Received().LookupByObject(ref value);
            sub.DidNotReceive().LookupByObject(ref otherValue);
        }
    }
}
