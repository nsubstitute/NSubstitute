using System;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallCollectionSpecs : ConcernFor<CallCollection>
    {
        public override CallCollection CreateSubjectUnderTest() => new CallCollection();

        [Test]
        public void Should_add_call()
        {
            //arrange
            var call = mock<ICall>();

            //act
            sut.Add(call);

            //assert
            CollectionAssert.Contains(sut.AllCalls(), call);
        }

        [Test]
        public void Should_delete_call_when_deleted()
        {
            //arrange
            var call = mock<ICall>();

            //act
            sut.Add(call);
            sut.Delete(call);

            //assert
            CollectionAssert.DoesNotContain(sut.AllCalls(), call);
        }

        [Test]
        public void Should_fail_when_delete_nonexisting_call()
        {
            //arrange
            var call = mock<ICall>();

            //act/assert
            var exception = Assert.Throws<SubstituteInternalException>(() => sut.Delete(call));
            Assert.That(exception.Message, Is.StringContaining("CallCollection.Delete - collection doesn't contain the call"));
        }

        [Test]
        public void Should_delete_all_calls_on_clear()
        {
            //arrange
            var call1 = mock<ICall>();
            var call2 = mock<ICall>();

            //act
            sut.Add(call1);
            sut.Add(call2);

            sut.Clear();

            //assert
            CollectionAssert.IsEmpty(sut.AllCalls());
        }

        [Test]
        public void Should_return_all_calls_in_the_order_they_were_received()
        {
            //arrange
            var firstCall = mock<ICall>();
            var secondCall = mock<ICall>();

            //act
            sut.Add(firstCall);
            sut.Add(secondCall);

            //assert
            CollectionAssert.AreEqual(sut.AllCalls(), new[] { firstCall, secondCall });
        }
    }
}