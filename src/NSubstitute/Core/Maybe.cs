using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NSubstitute.Core
{
    /// <summary>
    /// Particularly poor implementation of Maybe/Option type.
    /// This is just filling an immediate need; use FSharpOption or XSharpx or similar for a
    /// real implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Maybe<T> : IEnumerable<T>
    {
        private readonly bool hasValue;
        private readonly T value;

        public Maybe(T value) : this()
        {
            this.value = value;
            hasValue = true;
        }

        public bool HasValue() { return hasValue; }

        public Maybe<T> OrElse(Func<Maybe<T>> other) { var current = this; return Fold(other, _ => current); }
        public Maybe<T> OrElse(Maybe<T> other) { return OrElse(() => other); }
        public T ValueOr(Func<T> other) { return Fold(other, x => x); }
        public T ValueOr(T other) { return ValueOr(() => other); }
        public T? ValueOrDefault() { return ValueOr(default(T)!); }

        public TResult Fold<TResult>(Func<TResult> handleNoValue, Func<T, TResult> handleValue)
        {
            return HasValue() ? handleValue(value) : handleNoValue();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (hasValue)
            {
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>) this).GetEnumerator();
        }
    }

    public static class Maybe
    {
        public static Maybe<T> Just<T>(T value) { return new Maybe<T>(value); }
        public static Maybe<T> Nothing<T>() { return new Maybe<T>(); }
    }
}