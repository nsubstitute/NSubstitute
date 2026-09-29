using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class ThrowingAsyncExceptions
{
    public class WithVoidReturn
    {
        private ISomething _something;

        [Test]
        public async Task ThrowAsyncException()
        {
            var exception = new Exception();
            _something.Async().ThrowsAsync(exception);

            await AssertFaultedTaskException<Exception>(() => _something.Async());
        }

        [Test]
        public async Task ThrowAsyncExceptionWithDefaultConstructor()
        {
            _something.Async().ThrowsAsync<ArgumentException>();

            await AssertFaultedTaskException<ArgumentException>(() => _something.Async());
        }

        [Test]
        public async Task ThrowExceptionWithMessage()
        {
            const string exceptionMessage = "This is exception's message";

            _something.Async().ThrowsAsync(new Exception(exceptionMessage));

            Exception exceptionThrown = await AssertFaultedTaskException<Exception>(() => _something.Async());
            Assert.That(exceptionThrown.Message, Is.EqualTo(exceptionMessage));
        }

        [Test]
        public async Task ThrowExceptionWithInnerException()
        {
            ArgumentException innerException = new ArgumentException();
            _something.Async().ThrowsAsync(new Exception("Exception message", innerException));

            Exception exceptionThrown = await AssertFaultedTaskException<Exception>(() => _something.Async());

            Assert.That(exceptionThrown.InnerException, Is.Not.Null);
            Assert.That(exceptionThrown.InnerException, Is.InstanceOf<ArgumentException>());
        }

        [Test]
        public async Task ThrowExceptionUsingFactoryFunc()
        {
            _something.DoAsync("abc").ThrowsAsync(ci => new ArgumentException("Args:" + ci.Args()[0]));

            await AssertFaultedTaskException<ArgumentException>(() => _something.DoAsync("abc"));
        }

        [Test]
        public async Task DoesNotThrowForNonMatchingArgs()
        {
            _something.DoAsync(12).ThrowsAsync(new Exception());

            await AssertFaultedTaskException<Exception>(() => _something.DoAsync(12));
            Assert.DoesNotThrowAsync(() => _something.DoAsync(11));
        }

        [Test]
        public async Task ThrowExceptionForAnyArgs()
        {
            _something.DoAsync(12).ThrowsAsyncForAnyArgs(new Exception());

            await AssertFaultedTaskException<Exception>(() => _something.DoAsync(null));
            await AssertFaultedTaskException<Exception>(() => _something.DoAsync(12));
        }

        [Test]
        public async Task ThrowExceptionWithDefaultConstructorForAnyArgs()
        {
            _something.DoAsync(12).ThrowsAsyncForAnyArgs<InvalidOperationException>();

            await AssertFaultedTaskException<InvalidOperationException>(() => _something.DoAsync(null));
        }

        [Test]
        public async Task ThrowExceptionCreatedByFactoryFuncForAnyArgs()
        {
            _something.DoAsync(null).ThrowsAsyncForAnyArgs(ci => new ArgumentException("Args:" + ci.Args()[0]));

            await AssertFaultedTaskException<ArgumentException>(() => _something.DoAsync(new object()));
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
        public async Task ThrowAsyncException()
        {
            var exception = new Exception();
            _something.CountAsync().ThrowsAsync(exception);

            await AssertFaultedTaskException<Exception>(() => _something.CountAsync());
        }


        [Test]
        public async Task ThrowAsyncExceptionWithDefaultConstructor()
        {
            _something.CountAsync().ThrowsAsync<ArgumentException>();

            await AssertFaultedTaskException<ArgumentException>(() => _something.CountAsync());
        }

        [Test]
        public async Task ThrowExceptionWithMessage()
        {
            const string exceptionMessage = "This is exception's message";

            _something.CountAsync().ThrowsAsync(new Exception(exceptionMessage));

            Exception exceptionThrown = await AssertFaultedTaskException<Exception>(() => _something.CountAsync());
            Assert.That(exceptionThrown.Message, Is.EqualTo(exceptionMessage));
        }

        [Test]
        public async Task ThrowExceptionWithInnerException()
        {
            ArgumentException innerException = new ArgumentException();
            _something.CountAsync().ThrowsAsync(new Exception("Exception message", innerException));

            Exception exceptionThrown = await AssertFaultedTaskException<Exception>(() => _something.CountAsync());

            Assert.That(exceptionThrown.InnerException, Is.Not.Null);
            Assert.That(exceptionThrown.InnerException, Is.InstanceOf<ArgumentException>());
        }

        [Test]
        public async Task ThrowExceptionUsingFactoryFunc()
        {
            _something.AnythingAsync("abc").ThrowsAsync(ci => new ArgumentException("Args:" + ci.Args()[0]));

            await AssertFaultedTaskException<ArgumentException>(() => _something.AnythingAsync("abc"));
        }

        [Test]
        public async Task DoesNotThrowForNonMatchingArgs()
        {
            _something.AnythingAsync(12).ThrowsAsync(new Exception());

            await AssertFaultedTaskException<Exception>(() => _something.AnythingAsync(12));
            Assert.DoesNotThrowAsync(() => _something.AnythingAsync(11));
        }

        [Test]
        public async Task ThrowExceptionForAnyArgs()
        {
            _something.AnythingAsync(12).ThrowsAsyncForAnyArgs(new Exception());

            await AssertFaultedTaskException<Exception>(() => _something.AnythingAsync(null));
            await AssertFaultedTaskException<Exception>(() => _something.AnythingAsync(12));
        }

        [Test]
        public async Task ThrowExceptionWithDefaultConstructorForAnyArgs()
        {
            _something.AnythingAsync(12).ThrowsAsyncForAnyArgs<InvalidOperationException>();

            await AssertFaultedTaskException<InvalidOperationException>(() => _something.AnythingAsync(null));
        }

        [Test]
        public async Task ThrowExceptionCreatedByFactoryFuncForAnyArgs()
        {
            _something.AnythingAsync(null).ThrowsAsyncForAnyArgs(ci => new ArgumentException("Args:" + ci.Args()[0]));

            await AssertFaultedTaskException<ArgumentException>(() => _something.AnythingAsync(new object()));
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

    public class ForValueTask
    {

        private ISomething _something;

        [Test]
        public async Task ThrowAsyncException()
        {
            var exception = new Exception();
            _something.CountValueTaskAsync().ThrowsAsync(exception);

            await AssertFaultedTaskException<int, Exception>(() => _something.CountValueTaskAsync());
        }

        [Test]
        public async Task ThrowAsyncExceptionWithDefaultConstructor()
        {
            _something.CountValueTaskAsync().ThrowsAsync<int, ArgumentException>();

            await AssertFaultedTaskException<int, ArgumentException>(() => _something.CountValueTaskAsync());
        }

        [Test]
        public async Task ThrowExceptionWithMessage()
        {
            const string exceptionMessage = "This is exception's message";

            _something.CountValueTaskAsync().ThrowsAsync(new Exception(exceptionMessage));

            Exception exceptionThrown = await AssertFaultedTaskException<int, Exception>(() => _something.CountValueTaskAsync());
            Assert.That(exceptionThrown.Message, Is.EqualTo(exceptionMessage));
        }

        [Test]
        public async Task ThrowExceptionWithInnerException()
        {
            ArgumentException innerException = new ArgumentException();
            _something.CountValueTaskAsync().ThrowsAsync(new Exception("Exception message", innerException));

            Exception exceptionThrown = await AssertFaultedTaskException<int, Exception>(() => _something.CountValueTaskAsync());

            Assert.That(exceptionThrown.InnerException, Is.Not.Null);
            Assert.That(exceptionThrown.InnerException, Is.InstanceOf<ArgumentException>());
        }

        [Test]
        public async Task ThrowExceptionUsingFactoryFunc()
        {
            _something.AnythingValueTaskAsync("abc").ThrowsAsync(ci => new ArgumentException("Args:" + ci.Args()[0]));

            await AssertFaultedTaskException<int, ArgumentException>(() => _something.AnythingValueTaskAsync("abc"));
        }

        [Test]
        public async Task DoesNotThrowForNonMatchingArgs()
        {
            _something.AnythingValueTaskAsync(12).ThrowsAsync(new Exception());

            await AssertFaultedTaskException<int, Exception>(() => _something.AnythingValueTaskAsync(12));
            AssertDoesNotThrow(() => _something.AnythingValueTaskAsync(11));
        }

        [Test]
        public async Task ThrowExceptionForAnyArgs()
        {
            _something.AnythingValueTaskAsync(12).ThrowsAsyncForAnyArgs(new Exception());

            await AssertFaultedTaskException<int, Exception>(() => _something.AnythingValueTaskAsync(null));
            await AssertFaultedTaskException<int, Exception>(() => _something.AnythingValueTaskAsync(12));
        }

        [Test]
        public async Task ThrowExceptionWithDefaultConstructorForAnyArgs()
        {
            _something.AnythingValueTaskAsync(12).ThrowsAsyncForAnyArgs<int, InvalidOperationException>();

            await AssertFaultedTaskException<int, InvalidOperationException>(() => _something.AnythingValueTaskAsync(null));
        }

        [Test]
        public async Task ThrowExceptionCreatedByFactoryFuncForAnyArgs()
        {
            _something.AnythingValueTaskAsync(null).ThrowsAsyncForAnyArgs(ci => new ArgumentException("Args:" + ci.Args()[0]));

            await AssertFaultedTaskException<int, ArgumentException>(() => _something.AnythingValueTaskAsync(new object()));
        }

        [Test]
        public async Task ThrowVoidAsyncException()
        {
            var exception = new Exception();
            _something.VoidValueTaskAsync().ThrowsAsync(exception);

            await AssertFaultedTaskException<Exception>(() => _something.VoidValueTaskAsync());
        }

        [Test]
        public async Task ThrowVoidAsyncExceptionWithDefaultConstructor()
        {
            _something.VoidValueTaskAsync().ThrowsAsync<ArgumentException>();

            await AssertFaultedTaskException<ArgumentException>(() => _something.VoidValueTaskAsync());
        }

        [Test]
        public async Task ThrowVoidExceptionWithMessage()
        {
            const string exceptionMessage = "This is exception's message";

            _something.VoidValueTaskAsync().ThrowsAsync(new Exception(exceptionMessage));

            Exception exceptionThrown = await AssertFaultedTaskException<Exception>(() => _something.VoidValueTaskAsync());
            Assert.That(exceptionThrown.Message, Is.EqualTo(exceptionMessage));
        }

        [Test]
        public async Task ThrowVoidExceptionWithInnerException()
        {
            ArgumentException innerException = new ArgumentException();
            _something.VoidValueTaskAsync().ThrowsAsync(new Exception("Exception message", innerException));

            Exception exceptionThrown = await AssertFaultedTaskException<Exception>(() => _something.VoidValueTaskAsync());

            Assert.That(exceptionThrown.InnerException, Is.Not.Null);
            Assert.That(exceptionThrown.InnerException, Is.InstanceOf<ArgumentException>());
        }

        [Test]
        public async Task ThrowVoidExceptionUsingFactoryFunc()
        {
            _something.AnythingVoidValueTaskAsync("abc").ThrowsAsync(ci => new ArgumentException("Args:" + ci.Args()[0]));

            await AssertFaultedTaskException<ArgumentException>(() => _something.AnythingVoidValueTaskAsync("abc"));
        }

        [Test]
        public async Task DoesNotThrowVoidForNonMatchingArgs()
        {
            _something.AnythingVoidValueTaskAsync(12).ThrowsAsync(new Exception());

            await AssertFaultedTaskException<Exception>(() => _something.AnythingVoidValueTaskAsync(12));
            AssertDoesNotThrow(() => _something.AnythingVoidValueTaskAsync(11));
        }

        [Test]
        public async Task ThrowVoidExceptionForAnyArgs()
        {
            _something.AnythingVoidValueTaskAsync(12).ThrowsAsyncForAnyArgs(new Exception());

            await AssertFaultedTaskException<Exception>(() => _something.AnythingVoidValueTaskAsync(null));
            await AssertFaultedTaskException<Exception>(() => _something.AnythingVoidValueTaskAsync(12));
        }

        [Test]
        public async Task ThrowVoidExceptionWithDefaultConstructorForAnyArgs()
        {
            _something.AnythingVoidValueTaskAsync(12).ThrowsAsyncForAnyArgs<InvalidOperationException>();

            await AssertFaultedTaskException<InvalidOperationException>(() => _something.AnythingVoidValueTaskAsync(null));
        }

        [Test]
        public async Task ThrowVoidExceptionCreatedByFactoryFuncForAnyArgs()
        {
            _something.AnythingVoidValueTaskAsync(null).ThrowsAsyncForAnyArgs(ci => new ArgumentException("Args:" + ci.Args()[0]));

            await AssertFaultedTaskException<ArgumentException>(() => _something.AnythingVoidValueTaskAsync(new object()));
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

        public static async Task<TException> AssertFaultedTaskException<T, TException>(Func<ValueTask<T>> act)
            where TException : Exception
        {
            var actual = act();

            Assert.That(actual.IsFaulted, Is.True);
            return (await Assert.CatchAsync<TException>(async () => await actual))!;
        }

        public static void AssertDoesNotThrow<T>(Func<ValueTask<T>> act)
        {
            var actual = act();

            Assert.That(actual.IsFaulted, Is.False);
        }

        public static async Task<TException> AssertFaultedTaskException<TException>(Func<ValueTask> act)
            where TException : Exception
        {
            var actual = act();

            Assert.That(actual.IsFaulted, Is.True);
            return (await Assert.CatchAsync<TException>(async () => await actual))!;
        }

        public static void AssertDoesNotThrow(Func<ValueTask> act)
        {
            var actual = act();

            Assert.That(actual.IsFaulted, Is.False);
        }
    }

    public static async Task<TException> AssertFaultedTaskException<TException>(Func<Task> act)
        where TException : Exception
    {
        var actual = act();

        Assert.That(actual.Status, Is.EqualTo(TaskStatus.Faulted));
        Assert.That(actual.Exception, Is.TypeOf<AggregateException>());
        return actual.Exception!.InnerExceptions.First() as TException;
    }
}