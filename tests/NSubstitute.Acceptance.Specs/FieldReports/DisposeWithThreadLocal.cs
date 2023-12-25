using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class DisposeWithThreadLocal
{
    [Test]
    public void DisposeSubstituteAndPerformGC()
    {
        using (var s = Substitute.For<DisposableClass>()) { }
        GC.Collect();
        GC.WaitForPendingFinalizers();

        //Exception thrown on background thread. Can view this from output of test runner.
    }

    public class DisposableClass : IDisposable
    {
        bool disposed = false;
        ~DisposableClass() { Dispose(false); }
        public virtual void Dispose() { Dispose(true); }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed) { if (disposing) { disposed = true; } }
        }
    }
}