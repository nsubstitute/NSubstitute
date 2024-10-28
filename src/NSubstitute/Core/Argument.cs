namespace NSubstitute.Core;

public class Argument(ICall call, int argIndex)
{
    private readonly ICall? _call = call;

    public object? Value
    {
        get => _call!.GetArguments()[argIndex];
        set => _call!.GetArguments()[argIndex] = value;
    }

    public bool IsByRef => DeclaredType.IsByRef;

    public Type DeclaredType => _call!.GetParameterInfos()[argIndex].ParameterType;

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