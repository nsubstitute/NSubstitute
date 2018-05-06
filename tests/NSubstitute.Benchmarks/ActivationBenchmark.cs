using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using NSubstitute.Benchmarks.TestTypes;

namespace NSubstitute.Benchmarks
{
    [ClrJob, CoreJob]
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
        public CustomDelegate CreateDelegateSubstitute()
        {
            return Substitute.For<CustomDelegate>();
        }
    }
}