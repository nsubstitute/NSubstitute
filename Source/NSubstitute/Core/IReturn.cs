using System;
using System.Collections.Generic;
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
        private readonly IEnumerator<T> _valuesToReturn;
        public ReturnMultipleValues(IEnumerable<T> values)
        {
            _valuesToReturn = ValuesToReturn(values).GetEnumerator();
        }
        public object ReturnFor(CallInfo info) { return GetNext(); }
        public Type TypeOrNull() { return typeof (T); }
        public bool CanBeAssignedTo(Type t) { return typeof (T).IsAssignableFrom(t); }

        private T GetNext()
        {
            _valuesToReturn.MoveNext();
            return _valuesToReturn.Current;
        }

        private IEnumerable<T> ValuesToReturn(IEnumerable<T> specifiedValues)
        {
            T lastValue = default(T);
            foreach (var value in specifiedValues)
            {
                lastValue = value;
                yield return value;
            }
            yield return lastValue;
        }
    }

    public class ReturnMultipleFuncsValues<T> : IReturn
    {
        private readonly IEnumerator<Func<CallInfo, T>> _valuesToReturn;
        public ReturnMultipleFuncsValues(IEnumerable<Func<CallInfo, T>> funcs)
        {
            _valuesToReturn = ValuesToReturn(funcs).GetEnumerator();
        }
        public object ReturnFor(CallInfo info) { return GetNext(info); }
        public Type TypeOrNull() { return typeof (T); }
        public bool CanBeAssignedTo(Type t) { return typeof (T).IsAssignableFrom(t); }

        private T GetNext(CallInfo info)
        {
            _valuesToReturn.MoveNext();
            return _valuesToReturn.Current(info);
        }

        private IEnumerable<Func<CallInfo, T>> ValuesToReturn(IEnumerable<Func<CallInfo, T>> specifiedFuncs)
        {
            Func<CallInfo, T> lastValue = default(Func<CallInfo, T>);
            foreach (var value in specifiedFuncs)
            {
                lastValue = value;
                yield return value;
            }
            yield return lastValue;
        }
    }
}