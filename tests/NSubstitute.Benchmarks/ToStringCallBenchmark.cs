using BenchmarkDotNet.Attributes;
using NSubstitute.Benchmarks.TestTypes;

namespace NSubstitute.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
public class ToStringCallBenchmark
{
    private readonly IInterfaceWithSingleMethod _interfaceProxy;
    private readonly AbstractClassWithSingleMethod _abstractClassProxy;
    private readonly ClassWithSingleMethod _regularClassWithoutToStringProxy;
    private readonly ClassWithToStringImplementation _regularClassWithToStringProxy;

    public ToStringCallBenchmark()
    {
        _interfaceProxy = Substitute.For<IInterfaceWithSingleMethod>();
        _abstractClassProxy = Substitute.For<AbstractClassWithSingleMethod>();
        _regularClassWithoutToStringProxy = Substitute.For<ClassWithSingleMethod>();
        _regularClassWithToStringProxy = Substitute.For<ClassWithToStringImplementation>();
    }

    [Benchmark]
    public string InterfaceToString()
    {
        return _interfaceProxy.ToString();
    }

    [Benchmark]
    public string AbstractToString()
    {
        return _abstractClassProxy.ToString();
    }

    [Benchmark]
    public string ClassWithoutToStringImplementation()
    {
        return _regularClassWithoutToStringProxy.ToString();
    }

    [Benchmark]
    public string ClassWithToStringImplementation()
    {
        return _regularClassWithToStringProxy.ToString();
    }
}