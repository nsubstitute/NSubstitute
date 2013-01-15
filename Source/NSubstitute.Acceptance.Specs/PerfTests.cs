using System;
using System.Diagnostics;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class PerfTests
    {
        [Test]
        [Ignore("Long running, non-deterministic test. Used for basic profiling and checking impact of individual changes.")]
        public void TimeBasicOperations()
        {
            const int Result = 42;
            const string ArgForCall = "foo";

            var watch = Stopwatch.StartNew();
            for (int batch = 0; batch < 100; batch++)
            {
                var sub = Substitute.For<IFoo>();
                for (long i = 0; i < 1000; i++)
                {
                    sub.GetInt(ArgForCall).Returns(Result);
                    sub.GetInt(ArgForCall);
                    sub.Received().GetInt(ArgForCall);
                }

            }
            watch.Stop();

            Console.WriteLine("{0}", watch.ElapsedMilliseconds);
        }

        [Test]
        [Ignore("Long running, non-deterministic test. Used for basic profiling and checking impact of individual changes.")]
        public void TimeBasicOperationsWithGenerics()
        {
            const int Result = 42;
            const string ArgForCall = "foo";

            var watch = Stopwatch.StartNew();
            for (int batch = 0; batch < 100; batch++)
            {
                var sub = Substitute.For<IBar>();
                for (long i = 0; i < 1000; i++)
                {
                    sub.GetInt(ArgForCall).Returns(Result);
                    sub.GetInt(ArgForCall);
                    sub.Received().GetInt(ArgForCall);
                }

            }
            watch.Stop();

            Console.WriteLine("{0}", watch.ElapsedMilliseconds);
        }

        public interface IFoo { int GetInt(string s); }
        public interface IBar { int GetInt<T>(T t); }
    }
}