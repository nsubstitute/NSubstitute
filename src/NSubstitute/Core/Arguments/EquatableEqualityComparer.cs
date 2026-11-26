using System.Collections.Concurrent;
using System.Reflection;

namespace NSubstitute.Core.Arguments;

/// <summary>
/// Compares instances of types that implement <see cref="IEquatable{T}"/> without overriding
/// <see cref="object.Equals(object)"/>. Mirrors the semantics of <see cref="EqualityComparer{T}.Default"/>,
/// which prefers the <see cref="IEquatable{T}"/> implementation when one is available.
/// </summary>
internal static class EquatableEqualityComparer
{
    private static readonly ConcurrentDictionary<Type, MethodInfo?> EqualsMethods = new();

    public static bool AreEqualViaIEquatable(object? x, object? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        Type type = x.GetType();
        if (y.GetType() != type)
        {
            return false;
        }

        MethodInfo? equalsMethod = EqualsMethods.GetOrAdd(type, static t =>
        {
            Type equatableType = typeof(IEquatable<>).MakeGenericType(t);
            return equatableType.IsAssignableFrom(t)
                ? equatableType.GetMethod(nameof(IEquatable<object>.Equals))
                : null;
        });

        return equalsMethod is not null && (bool)equalsMethod.Invoke(x, new object?[] { y })!;
    }
}
