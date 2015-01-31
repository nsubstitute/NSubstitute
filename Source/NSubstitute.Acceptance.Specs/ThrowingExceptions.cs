using System;
using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Exceptions;
using NUnit.Framework;

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

        [Test]
        public void Throws_With_Exception_generetion_Exception_is_thrown()
        {
            _something.Anything(null).Throws(ci => new ArgumentException("Args:" + ci.Args()[0]));

            Assert.Catch<ArgumentException>(() => _something.Anything(null));
        }

        [Test]
        public void ThrowsForAnyArgs_Exception_is_thrown()
        {
            _something.Anything(12).ThrowsForAnyArgs(new Exception());

            Assert.Catch<Exception>(() => _something.Anything(null));
        }

        [Test]
        public void ThrowsForAnyArgs_the_same_parameter_Exception_is_thrown()
        {
            _something.Anything(12).ThrowsForAnyArgs(new Exception());

            Assert.Catch<Exception>(() => _something.Anything(12));
        }

        [Test]
        public void ThrowsForAnyArgs_InvalidOperationException_is_thrown()
        {
            _something.Anything(12).ThrowsForAnyArgs(new InvalidOperationException());

            Assert.Catch<InvalidOperationException>(() => _something.Anything(new object()));
        }

        [Test]
        public void ThrowsForAnyArgs_CustomException_is_thrown()
        {
            _something.Anything(12).ThrowsForAnyArgs(new AmbiguousArgumentsException());

            Assert.Catch<AmbiguousArgumentsException>(() => _something.Anything(new object()));
        }

        [Test]
        public void ThrowsForAnyArgs_Exception_with_message_is_thrown()
        {
            const string exceptionMessage = "This is exception's message";

            _something.Anything(12).ThrowsForAnyArgs(new Exception(exceptionMessage));

            Exception exceptionThrown = Assert.Catch<Exception>(() => _something.Anything(null));
            Assert.AreEqual(exceptionMessage, exceptionThrown.Message);
        }

        [Test]
        public void ThrowsForAnyArgs_Exception_with_inner_exception_is_thrown()
        {
            ArgumentException innerException = new ArgumentException();
            _something.Anything(12).ThrowsForAnyArgs(new Exception("Exception message", innerException));

            Exception exceptionThrown = Assert.Catch<Exception>(() => _something.Anything(1));

            Assert.IsNotNull(exceptionThrown.InnerException);
            Assert.IsInstanceOf<ArgumentException>(exceptionThrown.InnerException);
        }

        [Test]
        public void ThrowsForAnyArgs_With_Exception_generetion_Exception_is_thrown()
        {
            _something.Anything(null).ThrowsForAnyArgs(ci => new ArgumentException("Args:" + ci.Args()[0]));

            Assert.Catch<ArgumentException>(() => _something.Anything(new object()));
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
