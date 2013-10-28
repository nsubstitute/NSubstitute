using System;

namespace NSubstitute.Core
{
    /// <summary>
    /// Particularly poor implementation of Maybe/Option type.
    /// This is just filling an immediate need; use FSharpOption or XSharpx or similar for a 
    /// real implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Maybe<T>
    {
        private readonly bool hasValue;
        private readonly T value;

        public Maybe(T value) : this()
        {
            this.value = value;
            hasValue = true;
        }

        public bool HasValue() { return hasValue; }

        public T ValueOr(Func<T> other) { return Fold(other, x => x); }
        public T ValueOr(T other) { return ValueOr(() => other); }
        public T ValueOrDefault() { return ValueOr(default(T)); }

        public TResult Fold<TResult>(Func<TResult> handleNoValue, Func<T, TResult> handleValue)
        {
            return HasValue() ? handleValue(value) : handleNoValue();
        }

        public static Maybe<T> Nothing() { return new Maybe<T>(); }
    }
}