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
        object? ReturnFor(CallInfo info);
        Type? TypeOrNull();
        bool CanBeAssignedTo(Type t);
    }

    /// <summary>
    /// Performance optimization. Allows to not construct <see cref="CallInfo"/> if configured result doesn't depend on it.
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
        public object? ReturnFor(CallInfo info) => GetReturnValue();
        public Type? TypeOrNull() => _value?.GetType();
        public bool CanBeAssignedTo(Type t) => _value.IsCompatibleWith(t);
    }

    public class ReturnValueFromFunc<T> : IReturn
    {
        private readonly Func<CallInfo, T?> _funcToReturnValue;

        public ReturnValueFromFunc(Func<CallInfo, T?>? funcToReturnValue)
        {
            _funcToReturnValue = funcToReturnValue ?? ReturnNull();
        }

        public object? ReturnFor(CallInfo info) => _funcToReturnValue(info);
        public Type TypeOrNull() => typeof (T);
        public bool CanBeAssignedTo(Type t) => typeof (T).IsAssignableFrom(t);

        private static Func<CallInfo, T?> ReturnNull()
        {
            if (typeof(T).GetTypeInfo().IsValueType) throw new CannotReturnNullForValueType(typeof(T));
            return x => default(T);
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
        public object? ReturnFor(CallInfo info) => GetReturnValue();
        public Type TypeOrNull() => typeof (T);
        public bool CanBeAssignedTo(Type t) => typeof (T).IsAssignableFrom(t);

        private T? GetNext() => _valuesToReturn.TryDequeue(out var nextResult) ? nextResult : _lastValue;
    }

    public class ReturnMultipleFuncsValues<T> : IReturn
    {
        private readonly ConcurrentQueue<Func<CallInfo, T?>> _funcsToReturn;
        private readonly Func<CallInfo, T?> _lastFunc;

        public ReturnMultipleFuncsValues(Func<CallInfo, T?>[] funcs)
        {
            _funcsToReturn = new ConcurrentQueue<Func<CallInfo, T?>>(funcs);
            _lastFunc = funcs.Last();
        }

        public object? ReturnFor(CallInfo info) => GetNext(info);
        public Type TypeOrNull() => typeof (T);
        public bool CanBeAssignedTo(Type t) => typeof (T).IsAssignableFrom(t);

        private T? GetNext(CallInfo info) => _funcsToReturn.TryDequeue(out var nextFunc) ? nextFunc(info) : _lastFunc(info);
    }
}