using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    /// <summary>
    /// Defines an interface that
    /// provides undo action functionality.
    /// </summary>
    public interface IUndoable
    {
        /// <summary>
        /// Undoes action.
        /// </summary>
        void Undo();
    }

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
        public Type TypeOrNull() { return typeof(T); }
        public bool CanBeAssignedTo(Type t) { return typeof(T).IsAssignableFrom(t); }

        private static Func<CallInfo, T> ReturnNull()
        {
            if (typeof(T).IsValueType) throw new CannotReturnNullForValueType(typeof(T));
            return x => default(T);
        }
    }

    public class ReturnMultipleValues<T> : IReturn, IUndoable
    {
        private int _index = -1;
        private readonly T[] _valuesToReturn;

        public ReturnMultipleValues(IEnumerable<T> values)
        {
            _valuesToReturn = values.ToArray();
        }

        public object ReturnFor(CallInfo info) { return GetNext(); }
        public Type TypeOrNull() { return typeof(T); }
        public bool CanBeAssignedTo(Type t) { return typeof(T).IsAssignableFrom(t); }
        public void Undo() { if (_index > -1) _index--; }
        
        private T GetNext()
        {
            if (_index < _valuesToReturn.Length - 1) _index++;
            return _valuesToReturn[_index];
        }
    }

    public class ReturnMultipleFuncsValues<T> : IReturn, IUndoable
    {
        private int _index = -1;
        private readonly Func<CallInfo, T>[] _valuesToReturn;

        public ReturnMultipleFuncsValues(IEnumerable<Func<CallInfo, T>> funcs)
        {
            _valuesToReturn = funcs.ToArray();
        }

        public object ReturnFor(CallInfo info) { return GetNext(info); }
        public Type TypeOrNull() { return typeof(T); }
        public bool CanBeAssignedTo(Type t) { return typeof(T).IsAssignableFrom(t); }
        public void Undo() { if (_index > -1) _index--; }

        private T GetNext(CallInfo info)
        {
            if (_index < _valuesToReturn.Length - 1) _index++;
            return _valuesToReturn[_index](info);
        }
    }
}