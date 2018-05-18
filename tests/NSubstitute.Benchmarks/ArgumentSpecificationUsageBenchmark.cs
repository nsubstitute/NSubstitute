using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using NSubstitute.Benchmarks.TestTypes;

namespace NSubstitute.Benchmarks
{
    [ClrJob, CoreJob]
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
            var refValue = Arg.Any<int>();
            _substitute.MethodWithRefArg(ref refValue);
        }

        [Benchmark]
        public void ConfigureOutArgumentWithAnyValue()
        {
            var outValue = Arg.Any<int>();
            _substitute.MethodWithOutArg(out outValue);
        }
    }
}