using System;
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
        private readonly Func<T> _initalValueFactory;

        public RobustThreadLocal()
        {
            _threadLocal = new ThreadLocal<T>();
        }

        public RobustThreadLocal(Func<T> initialValueFactory)
        {
            _initalValueFactory = initialValueFactory ?? throw new ArgumentNullException(nameof(initialValueFactory));
            _threadLocal = new ThreadLocal<T>(initialValueFactory);
        }

        public T Value
        {
            get
            {
                try
                {
                    return _threadLocal.Value;
                }
                catch (ObjectDisposedException)
                {
                    return _initalValueFactory != null ? _initalValueFactory.Invoke() : default(T);
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
