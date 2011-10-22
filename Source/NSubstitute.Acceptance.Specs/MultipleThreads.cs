using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class MultipleThreads
    {
        [Test]
        [IgnoreTestForSilverlight]
        public void Configure_substitutes_on_different_threads()
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

        [Test]
        [Ignore("Long running, non-deterministic test. Reproduced prob with using CallResults from multiple threads.")]
        public void Call_substitute_method_that_needs_to_return_a_value_from_different_threads() {
            for (var i = 0; i < 1000; i++)
            {
                var sub = Substitute.For<IFoo>();
                var tasks = Enumerable.Range(0, 20).Select(x => new Task(() => sub.Bar())).ToArray();
                Task.StartAll(tasks);
                Task.AwaitAll(tasks);
            }
        }

        [Test]
        [Ignore("Long running, non-deterministic test. Reproduced prob with using CallStack from multiple threads.")]
        public void Call_substitute_method_that_does_not_return_and_just_needs_to_be_recorded_from_different_threads() {
            for (var i = 0; i < 100000; i++)
            {
                var sub = Substitute.For<IFoo>();
                var tasks = Enumerable.Range(0, 20).Select(x => new Task(() => sub.VoidMethod())).ToArray();
                Task.StartAll(tasks);
                Task.AwaitAll(tasks);
            }
        }

        private class Task {
            readonly Action _start;
            readonly Action _await;
            private Exception _exception;
            public Task(Action action) {
                var thread = new Thread(() =>
                {
                    try { action(); }
                    catch (Exception ex) { _exception = ex; }
                });
                _await = () => { thread.Join(); ThrowIfError(); };
                _start = () => thread.Start();
            }
            void ThrowIfError() { if (_exception != null) throw new Exception("Thread threw", _exception); }
            public static void StartAll(Task[] tasks) { Array.ForEach(tasks, x => x._start()); }
            public static void AwaitAll(Task[] tasks) { Array.ForEach(tasks, x => x._await()); }
        }

        public interface IFoo
        {
            int Number();
            IBar Bar();
            void VoidMethod();
        }

        public interface IBar { }
    }
}