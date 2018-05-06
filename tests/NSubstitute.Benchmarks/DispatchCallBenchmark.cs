using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using NSubstitute.Benchmarks.TestTypes;

namespace NSubstitute.Benchmarks
{
    [ClrJob, CoreJob]
    public class DispatchCallBenchmark
    {
        private readonly IInterfaceWithSingleMethod _interfaceProxy;
        private readonly AbstractClassWithSingleMethod _abstractClassProxy;
        private readonly ClassWithSingleMethod _classPartialProxy;
        private readonly CustomDelegate _delegateProxy;

        public DispatchCallBenchmark()
        {
            _interfaceProxy = Substitute.For<IInterfaceWithSingleMethod>();
            _abstractClassProxy = Substitute.For<AbstractClassWithSingleMethod>();
            _classPartialProxy = Substitute.For<ClassWithSingleMethod>();
            _delegateProxy = Substitute.For<CustomDelegate>();
        }

        [Benchmark]
        public int DispatchInterfaceProxyCall()
        {
            return _interfaceProxy.Method(null);
        }

        [Benchmark]
        public int DispatchAbstractClassProxyCall()
        {
            return _abstractClassProxy.Method(null);
        }

        [Benchmark]
        public int DispatchClassPartialProxyCall()
        {
            return _classPartialProxy.Method(null);
        }

        [Benchmark]
        public int DispatchDelegateCall()
        {
            return _delegateProxy.Invoke(null);
        }
    }
}