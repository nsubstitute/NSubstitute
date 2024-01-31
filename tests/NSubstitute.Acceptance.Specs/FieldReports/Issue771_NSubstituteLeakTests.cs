using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;


public class Issue771_NSubstituteLeakTests
{
    public class Foo;

    public interface IInterface
    {
        public void Method(Foo foo);
    }
    private readonly IInterface _interfaceMock = Substitute.For<IInterface>();

    [Test]
    public async Task NSubstituteDoesNotLeak()
    {
        WeakReference<Foo> weakFoo;
        {
            var foo = new Foo();
            weakFoo = new WeakReference<Foo>(foo);
            _interfaceMock.Method(foo);
            _interfaceMock.ClearReceivedCalls();
        }

        await GcCollect();

        Assert.False(weakFoo.TryGetTarget(out _));
    }

    private static async Task GcCollect()
    {
        for (var i = 0; i < 3; i++)
        {
            await Task.Yield();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}


