using System;
using System.Threading;

namespace NSubstitute.Core
{
    /// <summary>
    /// Delegates to ThreadLocal&lt;T&gt;, but wraps Value property access in try/catch to swallow ObjectDisposedExceptions.
    /// These can occur if the Value property is accessed from the finalizer thread. Because we can't detect this, we'll
    /// just swallow the exception (the finalizer thread won't be using any of the values from thread local storage anyway).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class RobustThreadLocal<T>
    {
        readonly ThreadLocal<T> _threadLocal;
        public RobustThreadLocal() { _threadLocal = new ThreadLocal<T>(); }
        public RobustThreadLocal(Func<T> initialValue) { _threadLocal = new ThreadLocal<T>(initialValue); }
        public T Value
        {
            get
            {
                try { return _threadLocal.Value; }
                catch (ObjectDisposedException) { return default(T); }
            }
            set
            {
                try { _threadLocal.Value = value; }
                catch (ObjectDisposedException) { }
            }
        }
    }
}
