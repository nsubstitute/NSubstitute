#if !NET45
using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class ThrowingAsyncExceptions
    {
        public class WithVoidReturn
        {
            private ISomething _something;

            [Test]
            public void ThrowAsyncException()
            {
                var exception = new Exception();
                _something.Async().ThrowsAsync(exception);

                AssertFaultedTaskException<Exception>(() => _something.Async());
            }

            [Test]
            public void ThrowAsyncExceptionWithDefaultConstructor()
            {
                _something.Async().ThrowsAsync<ArgumentException>();

                AssertFaultedTaskException<ArgumentException>(() => _something.Async());
            }

            [Test]
            public void ThrowExceptionWithMessage()
            {
                const string exceptionMessage = "This is exception's message";

                _something.Async().ThrowsAsync(new Exception(exceptionMessage));

                Exception exceptionThrown = AssertFaultedTaskException<Exception>(() => _something.Async());
                Assert.AreEqual(exceptionMessage, exceptionThrown.Message);
            }

            [Test]
            public void ThrowExceptionWithInnerException()
            {
                ArgumentException innerException = new ArgumentException();
                _something.Async().ThrowsAsync(new Exception("Exception message", innerException));

                Exception exceptionThrown = AssertFaultedTaskException<Exception>(() => _something.Async());

                Assert.IsNotNull(exceptionThrown.InnerException);
                Assert.IsInstanceOf<ArgumentException>(exceptionThrown.InnerException);
            }

            [Test]
            public void ThrowExceptionUsingFactoryFunc()
            {
                _something.DoAsync("abc").ThrowsAsync(ci => new ArgumentException("Args:" + ci.Args()[0]));

                AssertFaultedTaskException<ArgumentException>(() => _something.DoAsync("abc"));
            }

            [Test]
            public void DoesNotThrowForNonMatchingArgs()
            {
                _something.DoAsync(12).ThrowsAsync(new Exception());

                AssertFaultedTaskException<Exception>(() => _something.DoAsync(12));
                Assert.DoesNotThrowAsync(() => _something.DoAsync(11));
            }

            [Test]
            public void ThrowExceptionForAnyArgs()
            {
                _something.DoAsync(12).ThrowsAsyncForAnyArgs(new Exception());

                AssertFaultedTaskException<Exception>(() => _something.DoAsync(null));
                AssertFaultedTaskException<Exception>(() => _something.DoAsync(12));
            }

            [Test]
            public void ThrowExceptionWithDefaultConstructorForAnyArgs()
            {
                _something.DoAsync(12).ThrowsAsyncForAnyArgs<InvalidOperationException>();

                AssertFaultedTaskException<InvalidOperationException>(() => _something.DoAsync(null));
            }

            [Test]
            public void ThrowExceptionCreatedByFactoryFuncForAnyArgs()
            {
                _something.DoAsync(null).ThrowsAsyncForAnyArgs(ci => new ArgumentException("Args:" + ci.Args()[0]));

                AssertFaultedTaskException<ArgumentException>(() => _something.DoAsync(new object()));
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

        public class WithReturnValue
        {
            private ISomething _something;

            [Test]
            public void ThrowAsyncException()
            {
                var exception = new Exception();
                _something.CountAsync().ThrowsAsync(exception);

                AssertFaultedTaskException<Exception>(() => _something.CountAsync());
            }


            [Test]
            public void ThrowAsyncExceptionWithDefaultConstructor()
            {
                _something.CountAsync().ThrowsAsync<ArgumentException>();

                AssertFaultedTaskException<ArgumentException>(() => _something.CountAsync());
            }

            [Test]
            public void ThrowExceptionWithMessage()
            {
                const string exceptionMessage = "This is exception's message";

                _something.CountAsync().ThrowsAsync(new Exception(exceptionMessage));

                Exception exceptionThrown = AssertFaultedTaskException<Exception>(() => _something.CountAsync());
                Assert.AreEqual(exceptionMessage, exceptionThrown.Message);
            }

            [Test]
            public void ThrowExceptionWithInnerException()
            {
                ArgumentException innerException = new ArgumentException();
                _something.CountAsync().ThrowsAsync(new Exception("Exception message", innerException));

                Exception exceptionThrown = AssertFaultedTaskException<Exception>(() => _something.CountAsync());

                Assert.IsNotNull(exceptionThrown.InnerException);
                Assert.IsInstanceOf<ArgumentException>(exceptionThrown.InnerException);
            }

            [Test]
            public void ThrowExceptionUsingFactoryFunc()
            {
                _something.AnythingAsync("abc").ThrowsAsync(ci => new ArgumentException("Args:" + ci.Args()[0]));

                AssertFaultedTaskException<ArgumentException>(() => _something.AnythingAsync("abc"));
            }

            [Test]
            public void DoesNotThrowForNonMatchingArgs()
            {
                _something.AnythingAsync(12).ThrowsAsync(new Exception());

                AssertFaultedTaskException<Exception>(() => _something.AnythingAsync(12));
                Assert.DoesNotThrowAsync(() => _something.AnythingAsync(11));
            }

            [Test]
            public void ThrowExceptionForAnyArgs()
            {
                _something.AnythingAsync(12).ThrowsAsyncForAnyArgs(new Exception());

                AssertFaultedTaskException<Exception>(() => _something.AnythingAsync(null));
                AssertFaultedTaskException<Exception>(() => _something.AnythingAsync(12));
            }

            [Test]
            public void ThrowExceptionWithDefaultConstructorForAnyArgs()
            {
                _something.AnythingAsync(12).ThrowsAsyncForAnyArgs<InvalidOperationException>();

                AssertFaultedTaskException<InvalidOperationException>(() => _something.AnythingAsync(null));
            }

            [Test]
            public void ThrowExceptionCreatedByFactoryFuncForAnyArgs()
            {
                _something.AnythingAsync(null).ThrowsAsyncForAnyArgs(ci => new ArgumentException("Args:" + ci.Args()[0]));

                AssertFaultedTaskException<ArgumentException>(() => _something.AnythingAsync(new object()));
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

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public class ForValueTask
        {

            private ISomething _something;

            [Test]
            public void ThrowAsyncException()
            {
                var exception = new Exception();
                _something.CountValueTaskAsync().ThrowsAsync(exception);

                AssertFaultedTaskException<int, Exception>(() => _something.CountValueTaskAsync());
            }

            [Test]
            public void ThrowAsyncExceptionWithDefaultConstructor()
            {
                _something.CountValueTaskAsync().ThrowsAsync<int, ArgumentException>();

                AssertFaultedTaskException<int, ArgumentException>(() => _something.CountValueTaskAsync());
            }

            [Test]
            public void ThrowExceptionWithMessage()
            {
                const string exceptionMessage = "This is exception's message";

                _something.CountValueTaskAsync().ThrowsAsync(new Exception(exceptionMessage));

                Exception exceptionThrown = AssertFaultedTaskException<int, Exception>(() => _something.CountValueTaskAsync());
                Assert.AreEqual(exceptionMessage, exceptionThrown.Message);
            }

            [Test]
            public void ThrowExceptionWithInnerException()
            {
                ArgumentException innerException = new ArgumentException();
                _something.CountValueTaskAsync().ThrowsAsync(new Exception("Exception message", innerException));

                Exception exceptionThrown = AssertFaultedTaskException<int, Exception>(() => _something.CountValueTaskAsync());

                Assert.IsNotNull(exceptionThrown.InnerException);
                Assert.IsInstanceOf<ArgumentException>(exceptionThrown.InnerException);
            }

            [Test]
            public void ThrowExceptionUsingFactoryFunc()
            {
                _something.AnythingValueTaskAsync("abc").ThrowsAsync(ci => new ArgumentException("Args:" + ci.Args()[0]));

                AssertFaultedTaskException<int, ArgumentException>(() => _something.AnythingValueTaskAsync("abc"));
            }

            [Test]
            public void DoesNotThrowForNonMatchingArgs()
            {
                _something.AnythingValueTaskAsync(12).ThrowsAsync(new Exception());

                AssertFaultedTaskException<int, Exception>(() => _something.AnythingValueTaskAsync(12));
                AssertDoesNotThrow(() => _something.AnythingValueTaskAsync(11));
            }

            [Test]
            public void ThrowExceptionForAnyArgs()
            {
                _something.AnythingValueTaskAsync(12).ThrowsAsyncForAnyArgs(new Exception());

                AssertFaultedTaskException<int, Exception>(() => _something.AnythingValueTaskAsync(null));
                AssertFaultedTaskException<int, Exception>(() => _something.AnythingValueTaskAsync(12));
            }

            [Test]
            public void ThrowExceptionWithDefaultConstructorForAnyArgs()
            {
                _something.AnythingValueTaskAsync(12).ThrowsAsyncForAnyArgs<int, InvalidOperationException>();

                AssertFaultedTaskException<int, InvalidOperationException>(() => _something.AnythingValueTaskAsync(null));
            }

            [Test]
            public void ThrowExceptionCreatedByFactoryFuncForAnyArgs()
            {
                _something.AnythingValueTaskAsync(null).ThrowsAsyncForAnyArgs(ci => new ArgumentException("Args:" + ci.Args()[0]));

                AssertFaultedTaskException<int, ArgumentException>(() => _something.AnythingValueTaskAsync(new object()));
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

            public static TException AssertFaultedTaskException<T, TException>(Func<ValueTask<T>> act)
                where TException : Exception
            {
                var actual = act();

                Assert.That(actual.IsFaulted, Is.True);
                return Assert.CatchAsync<TException>(async () => await actual);
            }

            public static void AssertDoesNotThrow<T>(Func<ValueTask<T>> act)
            {
                var actual = act();

                Assert.That(actual.IsFaulted, Is.False);
            }
        }
#endif

        public static TException AssertFaultedTaskException<TException>(Func<Task> act)
            where TException : Exception
        {
            var actual = act();

            Assert.That(actual.Status, Is.EqualTo(TaskStatus.Faulted));
            Assert.That(actual.Exception, Is.TypeOf<AggregateException>());
            return actual.Exception!.InnerExceptions.First() as TException;
        }
    }
}
#endif