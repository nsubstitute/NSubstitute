using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public interface IReturn
    {
        object? ReturnFor(ICallInfo info);
        Type? TypeOrNull();
        bool CanBeAssignedTo(Type t);
    }

    /// <summary>
    /// Performance optimization. Allows to not construct <see cref="ICallInfo"/> if configured result doesn't depend on it.
    /// </summary>
    internal interface ICallIndependentReturn
    {
        object? GetReturnValue();
    }

    public class ReturnValue : IReturn, ICallIndependentReturn
    {
        private readonly object? _value;

        public ReturnValue(object? value)
        {
            _value = value;
        }

        public object? GetReturnValue() => _value;
        public object? ReturnFor(ICallInfo info) => GetReturnValue();
        public Type? TypeOrNull() => _value?.GetType();
        public bool CanBeAssignedTo(Type t) => _value.IsCompatibleWith(t);
    }

    public class ReturnValueFromFunc<T> : IReturn
    {
        private readonly Func<ICallInfo<T>, T?> _funcToReturnValue;

        public ReturnValueFromFunc(Func<ICallInfo<T>, T?>? funcToReturnValue)
        {
            _funcToReturnValue = funcToReturnValue ?? ReturnNull();
        }

        public object? ReturnFor(ICallInfo info) => _funcToReturnValue(info.ForCallReturning<T>());
        public Type TypeOrNull() => typeof(T);
        public bool CanBeAssignedTo(Type t) => typeof(T).IsAssignableFrom(t);

        private static Func<ICallInfo, T?> ReturnNull()
        {
            if (typeof(T).GetTypeInfo().IsValueType) throw new CannotReturnNullForValueType(typeof(T));
            return x => default;
        }
    }

    public class ReturnMultipleValues<T> : IReturn, ICallIndependentReturn
    {
        private readonly ConcurrentQueue<T?> _valuesToReturn;
        private readonly T? _lastValue;

        public ReturnMultipleValues(T?[] values)
        {
            _valuesToReturn = new ConcurrentQueue<T?>(values);
            _lastValue = values.Last();
        }

        public object? GetReturnValue() => GetNext();
        public object? ReturnFor(ICallInfo info) => GetReturnValue();
        public Type TypeOrNull() => typeof(T);
        public bool CanBeAssignedTo(Type t) => typeof(T).IsAssignableFrom(t);

        private T? GetNext() => _valuesToReturn.TryDequeue(out var nextResult) ? nextResult : _lastValue;
    }

    public class ReturnMultipleFuncsValues<T> : IReturn
    {
        private readonly ConcurrentQueue<Func<ICallInfo<T>, T?>> _funcsToReturn;
        private readonly Func<ICallInfo<T>, T?> _lastFunc;

        public ReturnMultipleFuncsValues(Func<ICallInfo<T>, T?>[] funcs)
        {
            _funcsToReturn = new ConcurrentQueue<Func<ICallInfo<T>, T?>>(funcs);
            _lastFunc = funcs.Last();
        }

        public object? ReturnFor(ICallInfo info) => GetNext(info);
        public Type TypeOrNull() => typeof(T);
        public bool CanBeAssignedTo(Type t) => typeof(T).IsAssignableFrom(t);

        private T? GetNext(ICallInfo info) =>
            _funcsToReturn.TryDequeue(out var nextFunc)
            ? nextFunc(info.ForCallReturning<T>())
            : _lastFunc(info.ForCallReturning<T>());
    }
}