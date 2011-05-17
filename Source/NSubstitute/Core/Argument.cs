using System;

namespace NSubstitute.Core
{
    public class Argument
    {
        private readonly Type _declaredType;
        private readonly object _value;

        public Argument(Type declaredType, object value)
        {
            _declaredType = declaredType;
            _value = value;
        }

        public object Value
        {
            get { return _value; }
        }

        public Type DeclaredType
        {
            get { return _declaredType; }
        }

        public Type ActualType
        {
            get { return (_value == null) ? _declaredType : _value.GetType(); }
        }
    }
}