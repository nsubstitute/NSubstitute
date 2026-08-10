using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NSubstitute.Exceptions;

namespace NSubstitute.Core.Arguments;

internal static class ParamsSupport
{
    public static bool IsParams(ParameterInfo parameterInfo)
    {
        const string paramCollectionAttributeFullName = "System.Runtime.CompilerServices.ParamCollectionAttribute";

        return parameterInfo.IsDefined(typeof(ParamArrayAttribute), inherit: false)
               // Needed because attribute is available in .NET 9+ only
               || parameterInfo.GetCustomAttributesData().Any(x => x.AttributeType.FullName == paramCollectionAttributeFullName);
    }

    public static Type GetElementType(Type paramsParameterType)
    {
        if (paramsParameterType.IsArray)
        {
            return paramsParameterType.GetElementType()!;
        }

        if (TryGetEnumerableElementType(paramsParameterType, out var elementType))
        {
            return elementType;
        }

        // The parameter type implements only the non-generic IEnumerable (e.g. ArrayList, or a custom
        // collection-initializer-pattern type without IEnumerable<T>). There's no "T" to read off an
        // interface in that case - the compiler itself falls back to whatever single-argument Add the
        // type exposes, so mirror that here.
        if (TryGetAddMethodParameterType(paramsParameterType, out elementType))
        {
            return elementType;
        }

        throw new SubstituteInternalException($"Could not determine params element type for parameter of type '{paramsParameterType.FullName}'.");

        static bool TryGetEnumerableElementType(Type type, [NotNullWhen(true)] out Type? elementType)
        {
            var enumerableOfT = type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                ? type
                : type.GetInterfaces().FirstOrDefault(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            elementType = enumerableOfT?.GetGenericArguments()[0];
            return elementType != null;
        }

        static bool TryGetAddMethodParameterType(Type type, [NotNullWhen(true)] out Type? elementType)
        {
            var addMethod = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "Add" && m.GetParameters().Length == 1);

            elementType = addMethod?.GetParameters()[0].ParameterType;
            return elementType != null;
        }
    }

    public static IEnumerable<object?> UnwrapArgument(object argument)
    {
        if (TryUnwrapArgument(argument, out var result))
        {
            return result;
        }

        throw new SubstituteInternalException($"Expected to get collection argument, but got argument of '{argument.GetType().FullName}' type.");
    }

    public static bool TryUnwrapArgument(object? argument, out IEnumerable<object?> result)
    {
        if (argument is IEnumerable enumerable)
        {
            result = enumerable.Cast<object?>();
            return true;
        }

        result = [];
        return false;
    }
}
