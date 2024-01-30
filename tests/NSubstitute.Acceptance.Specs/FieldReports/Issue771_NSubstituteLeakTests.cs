using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;


public class Issue771_NSubstituteLeakTests
{
    public class Foo;

    [Test]
    public void NSubstituteDoesNotLeak()
    {
        WeakReference<Foo> weakFoo;
        {
            var foo = new Foo();
            weakFoo = new WeakReference<Foo>(foo);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Assert.False(weakFoo.TryGetTarget(out _));
    }
}


