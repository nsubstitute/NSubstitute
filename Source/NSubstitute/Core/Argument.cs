using System;
using System.Reflection;

namespace NSubstitute.Core
{
    public class Argument
    {
        private readonly Type _declaredType;
        private readonly Func<object> _getValue;
        private readonly Action<object> _setValue;

        public Argument(Type declaredType, Func<object> getValue, Action<object> setValue)
        {
            _declaredType = declaredType;
            _getValue = getValue;
            _setValue = setValue;
        }

        public object Value
        {
            get { return _getValue(); }
            set { _setValue(value); }
        }

        public bool IsByRef { get { return DeclaredType.IsByRef; } }

        public Type DeclaredType
        {
            get { return _declaredType; }
        }

        public virtual Type ActualType
        {
            get { return (Value == null) ? _declaredType : Value.GetType(); }
        }

        public bool IsDeclaredTypeEqualToOrByRefVersionOf(Type type)
        {
            return AsNonByRefType(DeclaredType) == type;
        }

        public bool IsValueAssignableTo(Type type)
        {
            return type.IsAssignableFrom(AsNonByRefType(ActualType));
        }

        public bool CanSetValueWithInstanceOf(Type type)
        {
            return AsNonByRefType(ActualType).IsAssignableFrom(type);
        }

        private static Type AsNonByRefType(Type type)
        {
            return (type.IsByRef) ? type.GetElementType() : type;
        }
    }
}