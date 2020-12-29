using System;

namespace NSubstitute.Core
{
    internal class ReturnObservable<T> : IObservable<T?>
    {
        private readonly T? _value;

        public ReturnObservable() : this(default) { }

        public ReturnObservable(T? value)
        {
            _value = value;
        }

        public IDisposable Subscribe(IObserver<T?> observer)
        {
            observer.OnNext(_value);
            observer.OnCompleted();

            return EmptyDisposable.Instance;
        }
    }

    internal class EmptyDisposable : IDisposable
    {
        public static IDisposable Instance { get; } = new EmptyDisposable();

        public void Dispose() { }
    }
}