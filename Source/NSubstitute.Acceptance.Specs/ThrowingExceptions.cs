using NSubstitute.Acceptance.Specs.Infrastructure;
using NUnit.Framework;
using System;
using NSubstitute.Exceptions;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class ThrowingExceptions
    {
        private ISomething _something;

        [Test]
        public void Throws_Exception_is_thrown()
        {
            _something.Count().Throws(new Exception());            

            Assert.Catch<Exception>(() => _something.Count());
        }

        [Test]
        public void Throws_InvalidOperationException_is_thrown()
        {
            _something.Count().Throws(new InvalidOperationException());

            Assert.Catch<InvalidOperationException>(() => _something.Count());
        }

        [Test]
        public void Throws_CustomException_is_thrown()
        {
            _something.Count().Throws(new AmbiguousArgumentsException());

            Assert.Catch<AmbiguousArgumentsException>(() => _something.Count());
        }

        [Test]
        public void Throws_Exception_with_message_is_thrown()
        {
            const string exceptionMessage = "This is exception's message";

            _something.Count().Throws(new Exception(exceptionMessage));

            Exception exceptionThrown = Assert.Catch<Exception>(() => _something.Count());
            Assert.AreEqual(exceptionMessage, exceptionThrown.Message);
        }

        [Test]
        public void Throws_Exception_with_inner_exception_is_thrown()
        {
            ArgumentException innerException = new ArgumentException();
            _something.Count().Throws(new Exception("Exception message", innerException));

            Exception exceptionThrown = Assert.Catch<Exception>(() => _something.Count());
            
            Assert.IsNotNull(exceptionThrown.InnerException);
            Assert.IsInstanceOf<ArgumentException>(exceptionThrown.InnerException);
        }

        [SetUp]
        public void SetUp()
        {
            _something = Substitute.For<ISomething>();
        }

        [TearDown]
        public void TearDown()
        {
            _something = null;
        }
    }
}
