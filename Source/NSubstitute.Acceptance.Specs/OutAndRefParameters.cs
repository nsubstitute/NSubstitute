using System;
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
            string GetWithRef(ref string key);
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

        [Test]
        public void Modified_ref_arg_should_not_affect_specification_match()
        {
            //arrange
            var lookup = Substitute.For<ILookupStrings>();

            int magicInt = 42;
            lookup.TryRef(ref magicInt).Returns(true);

            int anyInt = Arg.Any<int>();
            lookup.When(l => l.TryRef(ref anyInt)).Do(c => { c[0] = 100; });

            //act
            magicInt = 42;
            var result = lookup.TryRef(ref magicInt);

            //assert
            Assert.That(result, Is.True);
            Assert.That(magicInt, Is.EqualTo(100));
        }

        [Test]
        public void Modified_out_arg_should_not_affect_specification_match()
        {
            //arrange
            var lookup = Substitute.For<ILookupStrings>();

            string key = "key";

            //Configure to have result for out parameter
            string retValue;
            lookup.TryGet(key, out retValue).Returns(true);

            //Set parameter in When/Do to always initialize out parameter
            string retValueTmp;
            lookup.When(l => l.TryGet(key, out retValueTmp)).Do(c => { c[1] = "42"; });

            //act
            //Check whether our configuration is still actual
            string actualRet;
            var result = lookup.TryGet(key, out actualRet);

            //assert
            Assert.That(result, Is.True);
            Assert.That(actualRet, Is.EqualTo("42"));
        }

        [Test]
        public void Configuration_for_already_configured_call_works()
        {
            //arrange
            var lookup = Substitute.For<ILookupStrings>();

            string anyRef = Arg.Any<string>();
            lookup.GetWithRef(ref anyRef).Returns(c => { c[0] = "42"; return "result"; });

            //act
            string otherKey = "xxx";
            lookup.GetWithRef(ref otherKey).Returns("100");

            otherKey = "xxx";
            var actualResult = lookup.GetWithRef(ref otherKey);

            //assert
            Assert.That(actualResult, Is.EqualTo("100"));
            Assert.That(otherKey, Is.EqualTo("xxx"));
        }

        [Test]
        public void Configuration_for_modified_args_works()
        {
            //arrange
            var lookup = Substitute.For<ILookupStrings>();

            string anyRef = Arg.Any<string>();
            lookup.When(l => l.GetWithRef(ref anyRef)).Do(c => { c[0] = "42"; });

            //act
            string specRef = "xxx";
            lookup.GetWithRef(ref specRef).Returns("100");

            specRef = "xxx";
            var result = lookup.GetWithRef(ref specRef);

            //assert
            Assert.That(result, Is.EqualTo("100"));
            Assert.That(specRef, Is.EqualTo("42"));
        }

        [Test]
        public void Exception_message_displays_original_values()
        {
            //arrange
            var lookup = Substitute.For<ILookupStrings>();

            string anyRef = Arg.Any<string>();
            lookup.When(l => l.GetWithRef(ref anyRef)).Do(c => { c[0] = "42"; });

            //act
            string key = "12345";
            lookup.GetWithRef(ref key);

            //assert
            //Assert that message is like "Expected '98765', but received '12345'".
            key = "98765";
            var exception = Assert.Throws<ReceivedCallsException>(() => lookup.Received().GetWithRef(ref key));
            StringAssert.Contains("98765", exception.Message);
            StringAssert.Contains("12345", exception.Message);
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
