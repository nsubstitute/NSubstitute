using System;
using System.Threading;
using NSubstitute.Acceptance.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class ConcurrencyTests
    {
        [Test]
        public void Call_between_invocation_and_received_doesnt_cause_issue()
        {
            //arrange
            var subs = Substitute.For<ISomething>();

            var backgroundReady = new AutoResetEvent(false);

            //act
            var dummy = subs.Say("ping");

            RunInOtherThread(() =>
            {
                subs.Echo(42);
                backgroundReady.Set();
            });

            backgroundReady.WaitOne();

            dummy.Returns("pong");

            //assert
            var actualResult = subs.Say("ping");

            Assert.That(actualResult, Is.EqualTo("pong"));
        }

        [Test]
        public void Background_invocation_doesnt_delete_specification()
        {
            //arrange
            var subs = Substitute.For<ISomething>();

            var backgroundReady = new AutoResetEvent(false);

            //act
            var dummy = subs.Say(Arg.Any<string>());

            RunInOtherThread(() =>
            {
                subs.Say("hello");
                backgroundReady.Set();
            });

            backgroundReady.WaitOne();
            dummy.Returns("42");

            //assert
            Assert.That(subs.Say("Alex"), Is.EqualTo("42"));
        }

        [Test]
        public void Both_threads_can_configure_returns_concurrently()
        {
            //arrange
            var subs = Substitute.For<ISomething>();

            var foregroundReady = new AutoResetEvent(false);
            var backgroundReady = new AutoResetEvent(false);

            //act
            //1
            var dummy = subs.Say("ping");

            RunInOtherThread(() =>
            {
                //2
                var d = subs.Echo(42);
                SignalAndWait(backgroundReady, foregroundReady);

                //4
                d.Returns("42");
                backgroundReady.Set();
            });

            backgroundReady.WaitOne();

            //3
            dummy.Returns("pong");
            SignalAndWait(foregroundReady, backgroundReady);

            //assert
            Assert.That(subs.Say("ping"), Is.EqualTo("pong"));
            Assert.That(subs.Echo(42), Is.EqualTo("42"));
        }

#if (NET45 || NET4 || NETSTANDARD1_3)
        [Test]
        public void Configuration_works_fine_for_async_methods()
        {
            //arrange
            var subs = Substitute.For<ISomething>();

            //act
            subs.EchoAsync(42).Returns("42");

            //assert
            var result = subs.EchoAsync(42).Result;
            Assert.That(result, Is.EqualTo("42"));
        }
#endif

        private static void RunInOtherThread(Action action)
        {
            new Thread(action.Invoke) {IsBackground = true}.Start();
        }

        private static void SignalAndWait(EventWaitHandle toSignal, EventWaitHandle toWait)
        {
            toSignal.Set();
            toWait.WaitOne();
        }
    }
}