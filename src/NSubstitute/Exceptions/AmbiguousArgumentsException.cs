using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using static System.Environment;

namespace NSubstitute.Exceptions
{
    public class AmbiguousArgumentsException : SubstituteException
    {
        internal const string NonReportedResolvedSpecificationsKey = "NON_REPORTED_RESOLVED_SPECIFICATIONS";
        private const string DefaultErrorMessage =
            "Cannot determine argument specifications to use. Please use specifications for all arguments of the same type.";

        private const string TabPadding = "    ";

        internal bool ContainsDefaultMessage { get; }

        public AmbiguousArgumentsException() : base(DefaultErrorMessage)
        {
            ContainsDefaultMessage = true;
        }

        public AmbiguousArgumentsException(string message) : base(message)
        {
        }

        public AmbiguousArgumentsException(MethodInfo method,
            IEnumerable<object?> invocationArguments,
            IEnumerable<IArgumentSpecification> matchedSpecifications,
            IEnumerable<IArgumentSpecification> allSpecifications)
            : this(BuildExceptionMessage(method, invocationArguments, matchedSpecifications, allSpecifications))
        {
        }

        private static string BuildExceptionMessage(MethodInfo method,
            IEnumerable<object?> invocationArguments,
            IEnumerable<IArgumentSpecification> matchedSpecifications,
            IEnumerable<IArgumentSpecification> allSpecifications)
        {
            string? methodSignature = null;
            string? methodArgsWithHighlightedPossibleArgSpecs = null;
            string? matchedSpecificationsInfo = null;
            if (CallFormatter.Default.CanFormat(method))
            {
                var argsWithInlinedParamsArray = invocationArguments.ToArray();
                // If last argument is `params`, we inline the value.
                if (method.GetParameters().Last().IsParams()
                    && argsWithInlinedParamsArray.Last() is IEnumerable paramsArray)
                {
                    argsWithInlinedParamsArray = argsWithInlinedParamsArray
                        .Take(argsWithInlinedParamsArray.Length - 1)
                        .Concat(paramsArray.Cast<object>())
                        .ToArray();
                }

                methodSignature = CallFormatter.Default.Format(
                    method,
                    FormatMethodParameterTypes(method.GetParameters()));

                methodArgsWithHighlightedPossibleArgSpecs = CallFormatter.Default.Format(
                    method,
                    FormatMethodArguments(argsWithInlinedParamsArray));

                matchedSpecificationsInfo = CallFormatter.Default.Format(
                    method,
                    PadNonMatchedSpecifications(matchedSpecifications, argsWithInlinedParamsArray));
            }

            var message = new StringBuilder();
            message.AppendLine(DefaultErrorMessage);

            if (methodSignature != null)
            {
                message.AppendLine("Method signature:");
                message.Append(TabPadding);
                message.AppendLine(methodSignature);
            }

            if (methodArgsWithHighlightedPossibleArgSpecs != null)
            {
                message.AppendLine("Method arguments (possible arg matchers are indicated with '*'):");
                message.Append(TabPadding);
                message.AppendLine(methodArgsWithHighlightedPossibleArgSpecs);
            }

            message.AppendLine("All queued specifications:");
            message.AppendLine(FormatSpecifications(allSpecifications));

            if (matchedSpecificationsInfo != null)
            {
                message.AppendLine("Matched argument specifications:");
                message.Append(TabPadding);
                message.AppendLine(matchedSpecificationsInfo);
            }

            return message.ToString();
        }

        private static IEnumerable<string> FormatMethodParameterTypes(IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(p =>
            {
                var type = p.ParameterType;

                if (p.IsOut)
                    return "out " + type.GetElementType()!.GetNonMangledTypeName();

                if (type.IsByRef)
                    return "ref " + type.GetElementType()!.GetNonMangledTypeName();

                if (p.IsParams())
                    return "params " + type.GetNonMangledTypeName();

                return type.GetNonMangledTypeName();
            });
        }

        private static IEnumerable<string> FormatMethodArguments(IEnumerable<object?> arguments)
        {
            var defaultChecker = new DefaultChecker(new DefaultForType());

            return arguments.Select(arg =>
            {
                var isPotentialArgSpec = arg == null || defaultChecker.IsDefault(arg, arg.GetType());
                return ArgumentFormatter.Default.Format(arg, highlight: isPotentialArgSpec);
            });
        }

        private static IEnumerable<string> PadNonMatchedSpecifications(IEnumerable<IArgumentSpecification> matchedSpecifications, IEnumerable<object?> allArguments)
        {
            var allMatchedSpecs = matchedSpecifications.Select(x => x.ToString() ?? string.Empty).ToArray();

            int nonResolvedArgumentsCount = allArguments.Count() - allMatchedSpecs.Length;
            var nonResolvedArgsPlaceholders = Enumerable.Repeat("???", nonResolvedArgumentsCount);

            return allMatchedSpecs.Concat(nonResolvedArgsPlaceholders);
        }

        private static string FormatSpecifications(IEnumerable<IArgumentSpecification> specifications)
        {
            return string.Join(NewLine, specifications.Select(spec => TabPadding + spec.ToString()));
        }
    }
}
