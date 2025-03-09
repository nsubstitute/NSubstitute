using NSubstitute.Exceptions;
using System.Collections.Concurrent;
using System.Reflection;

namespace NSubstitute.Core;

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

internal sealed class ReturnValue(object? value) : IReturn, ICallIndependentReturn
{
    public object? GetReturnValue() => value;
    public object? ReturnFor(CallInfo info) => GetReturnValue();
    public Type? TypeOrNull() => value?.GetType();
    public bool CanBeAssignedTo(Type t) => value.IsCompatibleWith(t);
}

internal sealed class ReturnValueFromFunc<T>(Func<CallInfo, T?>? funcToReturnValue) : IReturn
{
    private readonly Func<CallInfo, T?> _funcToReturnValue = funcToReturnValue ?? ReturnNull();

    public object? ReturnFor(CallInfo info) => _funcToReturnValue(info);
    public Type TypeOrNull() => typeof(T);
    public bool CanBeAssignedTo(Type t) => typeof(T).IsAssignableFrom(t);

    private static Func<CallInfo, T?> ReturnNull()
    {
        if (typeof(T).GetTypeInfo().IsValueType) throw new CannotReturnNullForValueType(typeof(T));
        return x => default(T);
    }
}

internal sealed class ReturnMultipleValues<T>(T?[] values) : IReturn, ICallIndependentReturn
{
    private readonly ConcurrentQueue<T?> _valuesToReturn = new ConcurrentQueue<T?>(values);
    private readonly T? _lastValue = values.Last();

    public object? GetReturnValue() => GetNext();
    public object? ReturnFor(CallInfo info) => GetReturnValue();
    public Type TypeOrNull() => typeof(T);
    public bool CanBeAssignedTo(Type t) => typeof(T).IsAssignableFrom(t);

    private T? GetNext() => _valuesToReturn.TryDequeue(out var nextResult) ? nextResult : _lastValue;
}

internal sealed class ReturnMultipleFuncsValues<T>(Func<CallInfo, T?>[] funcs) : IReturn
{
    private readonly ConcurrentQueue<Func<CallInfo, T?>> _funcsToReturn = new ConcurrentQueue<Func<CallInfo, T?>>(funcs);
    private readonly Func<CallInfo, T?> _lastFunc = funcs.Last();

    public object? ReturnFor(CallInfo info) => GetNext(info);
    public Type TypeOrNull() => typeof(T);
    public bool CanBeAssignedTo(Type t) => typeof(T).IsAssignableFrom(t);

    private T? GetNext(CallInfo info) => _funcsToReturn.TryDequeue(out var nextFunc) ? nextFunc(info) : _lastFunc(info);
}