using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface IReturn
    {
        object ReturnFor(CallInfo info);
    }

    public interface IReturn<T> : IReturn {}
    
    public class ReturnValue<T> : IReturn<T>
    {        
        private readonly T _value;
        public ReturnValue(T value) { _value = value; }
        public object ReturnFor(CallInfo info) { return _value; }
    }

    public class ReturnValueFromFunc<T> : IReturn<T>
    {
        private readonly Func<CallInfo, T> _funcToReturnValue;
        public ReturnValueFromFunc(Func<CallInfo, T> funcToReturnValue) { _funcToReturnValue = funcToReturnValue; }
        public object ReturnFor(CallInfo info) { return _funcToReturnValue(info); }
    }

    //public class ReturnMultipleValues<T> : IReturn
    //{
    //    private IEnumerable<T> _values;
    //    public ReturnMultipleValues(IEnumerable<T> values) { _values = values; }
    //    public object ReturnFor(CallInfo info) { return _value; }
    //}
}