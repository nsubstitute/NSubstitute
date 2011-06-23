using System;
using System.Threading;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class MultipleThreads
    {
        [Test]
        [IgnoreTestForSilverlight]
        public void CallFromMultipleThreads()
        {
            var firstSub = Substitute.For<IFoo>();
            var secondSub = Substitute.For<IFoo>();

            using (var interrupted = new AutoResetEvent(false))
            {

                var initialThread = new Thread(() =>
                                                   {
                                                       firstSub.Number();
                                                       if (!interrupted.WaitOne(TimeSpan.FromSeconds(1))) Assert.Fail("Timed out waiting for interrupt");
                                                       1.Returns(1);
                                                   });
                var interruptingThread = new Thread(() =>
                                                        {
                                                            secondSub.Number().Returns(2);
                                                            interrupted.Set();
                                                        });

                initialThread.Start();
                interruptingThread.Start();

                initialThread.Join();
                interruptingThread.Join();
            }

            Assert.That(firstSub.Number(), Is.EqualTo(1));
            Assert.That(secondSub.Number(), Is.EqualTo(2));
        }

        public interface IFoo
        {
            int Number();
        }
    }
}