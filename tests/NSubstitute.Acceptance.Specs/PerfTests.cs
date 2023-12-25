using System.Diagnostics;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

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

    [Test]
    [Ignore("It's a stress test. It might take a lot of time and it not optimized for frequent execution.")]
    public void Multiple_return_configuration_dont_leak_memory_for_any_args()
    {
        const int bufferSize = 100000000; //100 MB

        var subs = Substitute.For<IByteArrayConsumer>();

        //1000 chunks each 100 MB will require 100GB. If leak is present - OOM should be thrown.
        for (var i = 0; i < 1000; i++)
        {
            subs.ConsumeArray(new byte[bufferSize]).ReturnsForAnyArgs(true);
        }
    }

    [Test]
    [Ignore("FAILS because of CallResults leak")]
    public void Muiltiple_return_configurations_dont_lead_to_memory_leak()
    {
        const int bufferSize = 100000000; //100 MB

        var subs = Substitute.For<IByteArraySource>();

        //1000 chunks each 100 MB will require 100GB. If leak is present - OOM should be thrown.
        for (int i = 0; i < 1000; i++)
        {
            subs.GetArray().Returns(new byte[bufferSize]);
        }
    }

    [Test]
    public void Delegate_proxy_container_is_cached()
    {
        var subs1 = Substitute.For<Func<int, byte>>();
        var subs2 = Substitute.For<Func<int, byte>>();

        var containerType1 = subs1.Target.GetType();
        var containerType2 = subs2.Target.GetType();

        Assert.That(containerType1, Is.EqualTo(containerType2));
    }

    [Test]
    public void ProxyTypeIsCached()
    {
        var proxy1 = Substitute.For<IFoo>();
        var proxy2 = Substitute.For<IFoo>();

        var type1 = proxy1.GetType();
        var type2 = proxy2.GetType();

        Assert.That(type1, Is.EqualTo(type2));
    }

    public interface IFoo { int GetInt(string s); }
    public interface IBar { int GetInt<T>(T t); }
    public interface IByteArraySource { byte[] GetArray(); }
    public interface IByteArrayConsumer { bool ConsumeArray(byte[] array); }
}