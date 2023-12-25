using BenchmarkDotNet.Attributes;
using NSubstitute.Benchmarks.TestTypes;

namespace NSubstitute.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
public class ArgumentSpecificationUsageBenchmark
{
    private IInterfaceWithMultipleMethods _substitute = Substitute.For<IInterfaceWithMultipleMethods>();

    [Benchmark]
    public void ConfigureArgumentWithAnyValue()
    {
        _substitute.MethodWithArg(Arg.Any<int>());
    }

    [Benchmark]
    public void ConfigureRefArgumentWithAnyValue()
    {
        _substitute.MethodWithRefArg(ref Arg.Any<int>());
    }

    [Benchmark]
    public void ConfigureOutArgumentWithAnyValue()
    {
        _substitute.MethodWithOutArg(out Arg.Any<int>());
    }
}