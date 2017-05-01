using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public interface IReturn
    {
        object ReturnFor(CallInfo info);
        Type TypeOrNull();
        bool CanBeAssignedTo(Type t);
    }

    public class ReturnValue : IReturn
    {        
        private readonly object _value;
        public ReturnValue(object value) { _value = value; }
        public object ReturnFor(CallInfo info) { return _value; }
        public Type TypeOrNull() { return _value == null ? null : _value.GetType(); }
        public bool CanBeAssignedTo(Type t) { return _value.IsCompatibleWith(t); }
    }

    public class ReturnValueFromFunc<T> : IReturn
    {
        private readonly Func<CallInfo, T> _funcToReturnValue;
        public ReturnValueFromFunc(Func<CallInfo, T> funcToReturnValue) { _funcToReturnValue = funcToReturnValue ?? ReturnNull(); }
        public object ReturnFor(CallInfo info) { return _funcToReturnValue(info); }
        public Type TypeOrNull() { return typeof (T); }
        public bool CanBeAssignedTo(Type t) { return typeof (T).IsAssignableFrom(t); }

        private static Func<CallInfo, T> ReturnNull()
        {
            if (typeof(T).IsValueType()) throw new CannotReturnNullForValueType(typeof(T));
            return x => default(T);
        }
    }

    public class ReturnMultipleValues<T> : IReturn
    {
        private readonly ConcurrentQueue<T> _valuesToReturn;
        private readonly T _lastValue;

        public ReturnMultipleValues(T[] values)
        {
            _valuesToReturn = new ConcurrentQueue<T>(values);
            _lastValue = values.LastOrDefault();
        }
        public object ReturnFor(CallInfo info) { return GetNext(); }
        public Type TypeOrNull() { return typeof (T); }
        public bool CanBeAssignedTo(Type t) { return typeof (T).IsAssignableFrom(t); }

        private T GetNext()
        {
            T nextResult;
            if (_valuesToReturn.TryDequeue(out nextResult))
            {
                return nextResult;
            }

            return _lastValue;
        }
    }

    public class ReturnMultipleFuncsValues<T> : IReturn
    {
        private readonly ConcurrentQueue<Func<CallInfo, T>> _funcsToReturn;
        private readonly Func<CallInfo, T> _lastFunc;

        public ReturnMultipleFuncsValues(Func<CallInfo, T>[] funcs)
        {
            _funcsToReturn = new ConcurrentQueue<Func<CallInfo, T>>(funcs);
            _lastFunc = funcs.LastOrDefault();
        }
        public object ReturnFor(CallInfo info) { return GetNext(info); }
        public Type TypeOrNull() { return typeof (T); }
        public bool CanBeAssignedTo(Type t) { return typeof (T).IsAssignableFrom(t); }

        private T GetNext(CallInfo info)
        {
            Func<CallInfo, T> nextFunc;
            if (_funcsToReturn.TryDequeue(out nextFunc))
            {
                return nextFunc(info);
            }

            return _lastFunc(info);
        }
    }
}