using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core;

public class CallSpecification(MethodInfo methodInfo, IEnumerable<IArgumentSpecification> argumentSpecifications) : ICallSpecification
{
    private readonly IArgumentSpecification[] _argumentSpecifications = argumentSpecifications.ToArray();

    public MethodInfo GetMethodInfo() => methodInfo;

    public Type ReturnType() => methodInfo.ReturnType;

    public bool IsSatisfiedBy(ICall call)
    {
        if (!AreComparable(GetMethodInfo(), call.GetMethodInfo()))
        {
            return false;
        }

        if (HasDifferentNumberOfArguments(call))
        {
            return false;
        }

        if (!IsMatchingArgumentSpecifications(call))
        {
            return false;
        }

        return true;
    }

    private static bool AreComparable(MethodInfo a, MethodInfo b)
    {
        if (a == b)
        {
            return true;
        }

        if (a.IsGenericMethod && b.IsGenericMethod)
        {
            return CanCompareGenericMethods(a, b);
        }

        return false;
    }

    private static bool CanCompareGenericMethods(MethodInfo a, MethodInfo b)
    {
        return
               AreEquivalentDefinitions(a, b)
            && TypesAreAllEquivalent(ParameterTypes(a), ParameterTypes(b))
            && TypesAreAllEquivalent(a.GetGenericArguments(), b.GetGenericArguments());
    }

    private static Type[] ParameterTypes(MethodInfo info)
    {
        return info.GetParameters().Select(p => p.ParameterType).ToArray();
    }

    internal static bool TypesAreAllEquivalent(Type[] aArgs, Type[] bArgs)
    {
        if (aArgs.Length != bArgs.Length) return false;
        for (var i = 0; i < aArgs.Length; i++)
        {
            var first = aArgs[i];
            var second = bArgs[i];

            if (first.IsGenericType && second.IsGenericType
                && first.GetGenericTypeDefinition() == second.GetGenericTypeDefinition())
            {
                // both are the same generic type. If their GenericTypeArguments match then they are equivalent
                if (!TypesAreAllEquivalent(first.GenericTypeArguments, second.GenericTypeArguments))
                {
                    return false;
                }
                continue;
            }

            var areAssignable = first.IsAssignableFrom(second) || second.IsAssignableFrom(first);
            var areAnyTypeAssignable = typeof(Arg.AnyType).IsAssignableFrom(first) ||
                                       typeof(Arg.AnyType).IsAssignableFrom(second);
            var areByRefAnyTypeAssignable = first.IsByRef && second.IsByRef &&
                                            (typeof(Arg.AnyType).IsAssignableFrom(first.GetElementType()) ||
                                             typeof(Arg.AnyType).IsAssignableFrom(second.GetElementType()));
            var areEquivalent = areAssignable || areAnyTypeAssignable || areByRefAnyTypeAssignable;
            if (!areEquivalent) return false;
        }
        return true;
    }

    private static bool AreEquivalentDefinitions(MethodInfo a, MethodInfo b)
    {
        return a.IsGenericMethod == b.IsGenericMethod
               && a.ReturnType == b.ReturnType
               && a.Name.Equals(b.Name, StringComparison.Ordinal);
    }

    private bool IsMatchingArgumentSpecifications(ICall call)
    {
        object?[] arguments = call.GetOriginalArguments();
        for (int i = 0; i < arguments.Length; i++)
        {
            if (!_argumentSpecifications[i].IsSatisfiedBy(arguments[i]))
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerable<ArgumentMatchInfo> NonMatchingArguments(ICall call)
    {
        var arguments = call.GetOriginalArguments();
        return arguments
                .Select((arg, index) => new ArgumentMatchInfo(index, arg, _argumentSpecifications[index]))
                .Where(x => !x.IsMatch);
    }

    public override string ToString()
    {
        var argSpecsAsStrings = _argumentSpecifications.Select(x => x.ToString() ?? string.Empty).ToArray();
        return CallFormatter.Default.Format(GetMethodInfo(), argSpecsAsStrings);
    }

    public string Format(ICall call)
    {
        return CallFormatter.Default.Format(call.GetMethodInfo(), FormatArguments(call.GetOriginalArguments()));
    }

    private IEnumerable<string> FormatArguments(IEnumerable<object?> arguments)
    {
        return arguments.Zip(_argumentSpecifications, (arg, spec) => spec.FormatArgument(arg)).ToArray();
    }

    public ICallSpecification CreateCopyThatMatchesAnyArguments()
    {
        var anyArgs = GetMethodInfo()
            .GetParameters()
            .Zip(
                _argumentSpecifications,
                (p, spec) => spec.CreateCopyMatchingAnyArgOfType(p.ParameterType)
            )
            .ToArray();
        return new CallSpecification(GetMethodInfo(), anyArgs);
    }

    public void InvokePerArgumentActions(CallInfo callInfo)
    {
        var arguments = callInfo.Args();
        var argSpecs = _argumentSpecifications;

        for (var i = 0; i < arguments.Length; i++)
        {
            argSpecs[i].RunAction(arguments[i]);
        }
    }

    private bool HasDifferentNumberOfArguments(ICall call)
    {
        return _argumentSpecifications.Length != call.GetOriginalArguments().Length;
    }
}