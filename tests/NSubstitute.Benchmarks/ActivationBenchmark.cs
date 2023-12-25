using BenchmarkDotNet.Attributes;
using NSubstitute.Benchmarks.TestTypes;

namespace NSubstitute.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
public class ActivationBenchmark
{
    [Benchmark]
    public IInterfaceWithSingleMethod CreateInterfaceSubstitute()
    {
        return Substitute.For<IInterfaceWithSingleMethod>();
    }

    [Benchmark]
    public AbstractClassWithSingleMethod CreateAbstractClassSubstitute()
    {
        return Substitute.For<AbstractClassWithSingleMethod>();
    }

    [Benchmark]
    public ClassWithSingleMethod CreateNonAbstractClassSubstitute()
    {
        return Substitute.For<ClassWithSingleMethod>();
    }

    [Benchmark]
    public IntDelegate CreateDelegateSubstitute()
    {
        return Substitute.For<IntDelegate>();
    }
}