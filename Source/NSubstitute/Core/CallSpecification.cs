using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class CallSpecification : ICallSpecification
    {
        static readonly IMethodInfoFormatter DefaultFormatter = new CallFormatter();
        public IMethodInfoFormatter CallFormatter = DefaultFormatter;

        private readonly MethodInfo _methodInfo;
        readonly IArgumentSpecification[] _argumentSpecifications;

        public CallSpecification(MethodInfo methodInfo, IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            _methodInfo = methodInfo;
            _argumentSpecifications = argumentSpecifications.ToArray();
        }

        public MethodInfo GetMethodInfo() { return _methodInfo; }

        public Type ReturnType() { return _methodInfo.ReturnType; }

        public bool IsSatisfiedBy(ICall call)
        {
            if (!AreComparable(GetMethodInfo(), call.GetMethodInfo())) return false;
            if (HasDifferentNumberOfArguments(call)) return false;
            if (NonMatchingArguments(call).Any()) return false;
            return true;
        }

	    bool AreComparable(MethodInfo a, MethodInfo b)
	    {
            if (a == b) return true;
	        if (a.IsGenericMethod && b.IsGenericMethod) return CanCompareGenericMethods(a, b);
	        return false;
		}

        bool CanCompareGenericMethods(MethodInfo a, MethodInfo b)
        {
			return
				   AreEquivalentDefinitions(a, b)
				&& TypesAreAllEquivalent(ParameterTypes(a), ParameterTypes(b))
				&& TypesAreAllEquivalent(a.GetGenericArguments(), b.GetGenericArguments());
        }

        Type[] ParameterTypes(MethodInfo info)
		{
			return info.GetParameters().Select(p=>p.ParameterType).ToArray();
		}

	    static bool TypesAreAllEquivalent(Type[] aArgs, Type[] bArgs)
	    {
	        if (aArgs.Length != bArgs.Length) return false;
	        for (var i = 0; i < aArgs.Length; i++)
	        {
	            var first = aArgs[i];
	            var second = bArgs[i];

	            var areEquivalent = first.IsAssignableFrom(second) || second.IsAssignableFrom(first);
	            if (!areEquivalent) return false;
	        }
	        return true;
	    }

	    bool AreEquivalentDefinitions(MethodInfo a, MethodInfo b)
	    {
		    return a.IsGenericMethod == b.IsGenericMethod
                   && a.ReturnType == b.ReturnType
                   && a.Name.Equals(b.Name, StringComparison.Ordinal);
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
            var argSpecsAsStrings = _argumentSpecifications.Select(x => x.ToString()).ToArray();
            return CallFormatter.Format(GetMethodInfo(), argSpecsAsStrings);
        }

        public string Format(ICall call)
        {
            return CallFormatter.Format(call.GetMethodInfo(), FormatArguments(call.GetOriginalArguments()));
        }

        private IEnumerable<string> FormatArguments(IEnumerable<object> arguments)
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
}