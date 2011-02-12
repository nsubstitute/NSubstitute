using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class DisposeWithThreadLocal
    {
        [Test]
        public void DisposeSubstituteAndPerformGC()
        {
            using (var s = Substitute.For<DisposableClass>())
            { }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public class DisposableClass : IDisposable
        {
            bool disposed = false;

            ~DisposableClass() { Dispose(false); }

            public virtual void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (disposing)
                    { // Dispose something }
                        disposed = true;
                    }
                }
            }
        }
    }
}