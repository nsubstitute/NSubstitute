using BenchmarkDotNet.Attributes;
using NSubstitute.Benchmarks.TestTypes;

namespace NSubstitute.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
public class VerifyReceivedCallBenchmark
{
    private readonly IInterfaceWithSingleMethod _interfaceProxy;
    private readonly AbstractClassWithSingleMethod _abstractClassProxy;
    private readonly ClassWithSingleMethod _classPartialProxy;
    private readonly IntDelegate _intDelegateProxy;
    private readonly VoidDelegate _voidDelegateProxy;

    public VerifyReceivedCallBenchmark()
    {
        _interfaceProxy = Substitute.For<IInterfaceWithSingleMethod>();
        _interfaceProxy.IntMethod("42");
        _interfaceProxy.VoidMethod("42");

        _abstractClassProxy = Substitute.For<AbstractClassWithSingleMethod>();
        _abstractClassProxy.IntMethod("42");
        _abstractClassProxy.VoidMethod("42");

        _classPartialProxy = Substitute.For<ClassWithSingleMethod>();
        _classPartialProxy.IntMethod("42");
        _classPartialProxy.VoidMethod("42");

        _intDelegateProxy = Substitute.For<IntDelegate>();
        _intDelegateProxy("42");

        _voidDelegateProxy = Substitute.For<VoidDelegate>();
        _voidDelegateProxy("42");
    }

    [Benchmark]
    public int VerifyInterfaceProxyCall_Int() => _interfaceProxy.Received().IntMethod("42");

    [Benchmark]
    public int VerifyAbstractClassProxyCall_Int() => _abstractClassProxy.Received().IntMethod("42");

    [Benchmark]
    public int VerifyClassPartialProxyCall_Int() => _classPartialProxy.Received().IntMethod("42");

    [Benchmark]
    public int VerifyDelegateCall_Int() => _intDelegateProxy.Received().Invoke("42");

    [Benchmark]
    public void VerifyInterfaceProxyCall_Void() => _interfaceProxy.Received().VoidMethod("42");

    [Benchmark]
    public void VerifyAbstractClassProxyCall_Void() => _abstractClassProxy.Received().VoidMethod("42");

    [Benchmark]
    public void VerifyClassPartialProxyCall_Void() => _classPartialProxy.Received().VoidMethod("42");

    [Benchmark]
    public void VerifyDelegateCall_Void() => _voidDelegateProxy.Received().Invoke("42");
}