using BenchmarkDotNet.Attributes;
using NSubstitute.Benchmarks.TestTypes;

namespace NSubstitute.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
public class DispatchNonConfiguredCallBenchmark
{
    private readonly IInterfaceWithSingleMethod _interfaceProxy;
    private readonly AbstractClassWithSingleMethod _abstractClassProxy;
    private readonly ClassWithSingleMethod _classPartialProxy;
    private readonly IntDelegate _intDelegateProxy;
    private readonly VoidDelegate _voidDelegateProxy;

    public DispatchNonConfiguredCallBenchmark()
    {
        _interfaceProxy = Substitute.For<IInterfaceWithSingleMethod>();
        _abstractClassProxy = Substitute.For<AbstractClassWithSingleMethod>();
        _classPartialProxy = Substitute.For<ClassWithSingleMethod>();
        _intDelegateProxy = Substitute.For<IntDelegate>();
        _voidDelegateProxy = Substitute.For<VoidDelegate>();
    }

    [Benchmark]
    public int DispatchInterfaceProxyCall_Int() => _interfaceProxy.IntMethod(null);

    [Benchmark]
    public int DispatchAbstractClassProxyCall_Int() => _abstractClassProxy.IntMethod(null);

    [Benchmark]
    public int DispatchClassPartialProxyCall_Int() => _classPartialProxy.IntMethod(null);

    [Benchmark]
    public int DispatchDelegateCall_Int() => _intDelegateProxy.Invoke(null);

    [Benchmark]
    public void DispatchInterfaceProxyCall_Void() => _interfaceProxy.VoidMethod(null);

    [Benchmark]
    public void DispatchAbstractClassProxyCall_Void() => _abstractClassProxy.VoidMethod(null);

    [Benchmark]
    public void DispatchClassPartialProxyCall_Void() => _classPartialProxy.VoidMethod(null);

    [Benchmark]
    public void DispatchDelegateCall_Void() => _voidDelegateProxy.Invoke(null);
}