using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NSubstitute.Core
{
    /// <summary>
    /// Delegates to ThreadLocal&lt;T&gt;, but wraps Value property access in try/catch to swallow ObjectDisposedExceptions.
    /// These can occur if the Value property is accessed from the finalizer thread. Because we can't detect this, we'll
    /// just swallow the exception (the finalizer thread won't be using any of the values from thread local storage anyway).
    /// </summary>
    internal class RobustThreadLocal<T>
    {
        private readonly ThreadLocal<T> _threadLocal;
        private readonly Func<T>? _initialValueFactory;

        public RobustThreadLocal()
        {
            _threadLocal = new ThreadLocal<T>();
        }

        public RobustThreadLocal(Func<T> initialValueFactory)
        {
            _initialValueFactory = initialValueFactory;
            _threadLocal = new ThreadLocal<T>(initialValueFactory);
        }

        public T Value
        {
            get
            {
                // Suppress nullability for result, as we trust type by usage.
                // For non-nullable we expect ctor with default to be used.
                try
                {
                    return _threadLocal.Value!;
                }
                catch (ObjectDisposedException)
                {
                    return _initialValueFactory != null ? _initialValueFactory.Invoke() : default!;
                }
            }
            set
            {
                try
                {
                    _threadLocal.Value = value;
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }
    }
}
