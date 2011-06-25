using NSubstitute.Exceptions;
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
        public void Received_with_any_ref_args()
        {
            var sub = Substitute.For<ILookupStrings>();

            var value = 1;
            var otherValue = 1;
            sub.TryRef(ref value);

            sub.ReceivedWithAnyArgs().TryRef(ref otherValue);
        }

        [Test]
        public void When_with_any_ref_args()
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
        public void Set_ref_argument_via_returns()
        {
            var anyInt = 0;
            var sub = Substitute.For<ILookupStrings>();

            sub.TryRef(ref anyInt).ReturnsForAnyArgs(x => { x[0] = 5; return true; });

            var result = sub.TryRef(ref anyInt);

            Assert.That(result, Is.True);
            Assert.That(anyInt, Is.EqualTo(5));
        }

        [Test]
        public void Set_ref_argument_via_when_do()
        {
            var anyInt = 0;
            var sub = Substitute.For<ILookupStrings>();

            sub.WhenForAnyArgs(x => x.TryRef(ref anyInt)).Do(x => x[0] = 5);

            sub.TryRef(ref anyInt);

            Assert.That(anyInt, Is.EqualTo(5));
        }

        [Test]
        public void Set_out_argument_via_returns()
        {
            var sub = Substitute.For<ILookupStrings>();
            string output;
            sub.TryGet("key", out output).Returns(x => { x[1] = "howdy"; return true; });

            var result = sub.TryGet("key", out output);

            Assert.That(result, Is.True);
            Assert.That(output, Is.EqualTo("howdy"));
        }

        [Test]
        public void Throw_when_setting_non_out_arg()
        {
            var sub = Substitute.For<ILookupStrings>();
            string output;
            sub.TryGet("key", out output).Returns(x => { x[0] = "arg 0 is not settable"; return true; });

            Assert.Throws<ArgumentIsNotOutOrRefException>(() => sub.TryGet("key", out output));
        }

        [Test]
        public void Throw_when_setting_arg_with_incompatible_value()
        {
            var sub = Substitute.For<ILookupStrings>();
            string output;
            sub.TryGet("key", out output).Returns(x => { x[1] = 2; return true; });

            Assert.Throws<ArgumentSetWithIncompatibleValueException>(() => sub.TryGet("key", out output));
        }

        [Test]
        public void Match_int_ref_argument()
        {
            var sub = Substitute.For<ILookupStrings>();
            var value = 1;

            sub.TryRef(ref value);

            sub.Received().TryRef(ref value);
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

        [Test]
        public void Sample_test_for_a_subject_that_uses_a_substitute_with_out_arguments()
        {
            var lookup = Substitute.For<ILookupStrings>();
            var something = new Something(lookup);

            var key = "key";
            string value;
            lookup.TryGet(key, out value).Returns(x => { x[1] = "test"; return true; });

            Assert.That(something.GetValue(key), Is.EqualTo("test"));
            Assert.That(something.GetValue("diff key"), Is.EqualTo("none"));
        }

        private class Something
        {
            private readonly ILookupStrings _lookup;
            public Something(ILookupStrings lookup) { _lookup = lookup; }
            public string GetValue(string key)
            {
                string value;
                return _lookup.TryGet(key, out value) ? value : "none";
            }
        }
    }
}
