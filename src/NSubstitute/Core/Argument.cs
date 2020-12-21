using System;
using System.Reflection;

namespace NSubstitute.Core
{
    public class Argument
    {
        private readonly ICall? _call;
        private readonly int _argIndex;

        private readonly Type? _declaredType;
        private readonly Func<object?>? _getValue;
        private readonly Action<object?>? _setValue;

        [Obsolete("This constructor overload is deprecated and will be removed in the next version.")]
        public Argument(Type declaredType, Func<object?> getValue, Action<object?> setValue)
        {
            _declaredType = declaredType;
            _getValue = getValue;
            _setValue = setValue;
        }

        public Argument(ICall call, int argIndex)
        {
            _call = call;
            _argIndex = argIndex;
        }

        public object? Value
        {
            get => _getValue != null ? _getValue() : _call!.GetArguments()[_argIndex];
            set
            {
                if (_setValue != null)
                {
                    _setValue(value);
                }
                else
                {
                    _call!.GetArguments()[_argIndex] = value;
                }
            }
        }

        public bool IsByRef => DeclaredType.IsByRef;

        public Type DeclaredType => _declaredType ?? _call!.GetParameterInfos()[_argIndex].ParameterType;

        public Type ActualType => Value == null ? DeclaredType : Value.GetType();

        public bool IsDeclaredTypeEqualToOrByRefVersionOf(Type type) =>
            AsNonByRefType(DeclaredType) == type;

        public bool IsValueAssignableTo(Type type) =>
            type.IsAssignableFrom(AsNonByRefType(ActualType));

        public bool CanSetValueWithInstanceOf(Type type) =>
            AsNonByRefType(DeclaredType).IsAssignableFrom(type);

        private static Type AsNonByRefType(Type type) =>
            type.IsByRef ? type.GetElementType()! : type;
    }
}