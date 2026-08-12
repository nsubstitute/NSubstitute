using System.Collections;
using System.Text;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Core.SequenceChecking;
using NSubstitute.Exceptions;

namespace NSubstitute;

public sealed class WillReceiveExpectation : IQuery
{
    private const string _indent = "    ";
    private readonly List<CallSpecAndTarget> _expectedCallSpecAndTargets = [];
    private readonly List<UnexpectedCallData?> _receivedCalls = [];
    private readonly ICallSpecificationFactory _callSpecificationFactory;
    private readonly InstanceTracker _instanceTracker = new();
    private readonly Action _buildExpectationsAction;
    private bool _buildingExpectations;

    public WillReceiveExpectation(ICallSpecificationFactory callSpecificationFactory, Action buildExpectationsAction)
    {
        _callSpecificationFactory = callSpecificationFactory;
        _buildExpectationsAction = buildExpectationsAction;
    }

    public void WhileExecuting(Action action)
    {
        _buildingExpectations = true;

        SubstitutionContext.Current.ThreadContext.RunInQueryContext(_buildExpectationsAction, this);

        _buildingExpectations = false;

#if NET6_0_OR_GREATER
        _receivedCalls.EnsureCapacity(_expectedCallSpecAndTargets.Count);
#endif

        SubstitutionContext.Current.ThreadContext.RunInQueryContext(action, this);

        AssertReceivedCalls();
    }

    void IQuery.RegisterCall(ICall call)
    {
        if (call.GetMethodInfo().GetPropertyFromGetterCallOrNull() != null)
            return;

        if (_buildingExpectations)
            AddCallExpectation(call);
        else
            AddReceivedAssertionCall(call);
    }
    private void AddCallExpectation(ICall call)
    {
        var callSpecification = _callSpecificationFactory.CreateFrom(call, MatchArgs.AsSpecifiedInCall);

        _expectedCallSpecAndTargets.Add(new CallSpecAndTarget(callSpecification, call.Target()));
    }
    private void AddReceivedAssertionCall(ICall call)
    {
        var instanceNumber = _instanceTracker.InstanceNumber(call.Target());
        var expectedCallIndex = _receivedCalls.Count;

        if (expectedCallIndex >= _expectedCallSpecAndTargets.Count)
        {
            _receivedCalls.Add(new UnexpectedCallData(specification: null, call, instanceNumber));
            return;
        }

        var specAndTarget = _expectedCallSpecAndTargets[expectedCallIndex];

        var callData = !specAndTarget.CallSpecification.IsSatisfiedBy(call)
            ? new UnexpectedCallData(specAndTarget.CallSpecification, call, instanceNumber)
            : null;

        _receivedCalls.Add(callData);
    }

    private void AssertReceivedCalls()
    {
        if (_receivedCalls.Any(x => x != null) || _receivedCalls.Count < _expectedCallSpecAndTargets.Count)
            throw new CallSequenceNotFoundException(CreateExceptionMessage());
    }

    private string CreateExceptionMessage()
    {
        var builder = new StringBuilder();
        var includeInstanceNumber = HasMultipleCallsOnSameType();
        var multipleInstances = _instanceTracker.NumberOfInstances() > 1;

        builder.AppendLine();

        var i = 0;

        for (; i < _receivedCalls.Count; i++)
        {
            var callData = _receivedCalls[i];

            builder.Append("Call ");
            builder.Append(i + 1);
            builder.Append(": ");

            if (callData == null)
            {
                builder.AppendLine("Accepted!");
            }
            else
            {
                var expectedCall = i < _expectedCallSpecAndTargets.Count
                    ? _expectedCallSpecAndTargets[i]
                    : null;

                AppendUnexpectedCallToExceptionMessage(builder, expectedCall, callData, multipleInstances, includeInstanceNumber);
            }
        }

        AppendNotReceivedCallsToExceptionMessage(builder, nextExpectedCallIndex: i);

        return builder.ToString();
    }

    private bool HasMultipleCallsOnSameType()
    {
        var lookup = new Dictionary<Type, int>();

        foreach (var call in _receivedCalls)
        {
            if (call == null)
                continue;

            if (lookup.TryGetValue(call.DeclaringType, out var instanceNumber))
            {
                if (instanceNumber != call.InstanceNumber)
                    return true;
            }
            else
            {
                lookup.Add(call.DeclaringType, call.InstanceNumber);
            }
        }

        return false;
    }

    private static void AppendUnexpectedCallToExceptionMessage(StringBuilder builder,
        CallSpecAndTarget? expectedCall,
        UnexpectedCallData unexpectedCallData,
        bool multipleInstances,
        bool includeInstanceNumber)
    {
        // Not matched or unexpected
        if (expectedCall != null)
        {
            builder.AppendLine("Not matched!");
            builder.Append($"{_indent}Expected: ");
            builder.AppendLine(expectedCall.CallSpecification.ToString());
        }
        else
        {
            builder.AppendLine("Unexpected!");
        }

        builder.Append($"{_indent}But was: ");

        // Prepend instance number and type if multiple instances
        if (multipleInstances)
        {
            if (includeInstanceNumber)
            {
                builder.Append(unexpectedCallData.InstanceNumber);
                builder.Append('@');
            }

            builder.Append(unexpectedCallData.DeclaringType.GetNonMangledTypeName());
            builder.Append('.');
        }

        builder.AppendLine(unexpectedCallData.CallFormat);

        // Append non-matching arguments
        foreach (var argumentFormat in unexpectedCallData.NonMatchingArgumentFormats)
        {
            builder.Append(_indent);
            builder.Append(_indent);
            builder.AppendLine(argumentFormat);
        }
    }

    private void AppendNotReceivedCallsToExceptionMessage(StringBuilder builder, int nextExpectedCallIndex)
    {
        for (; nextExpectedCallIndex < _expectedCallSpecAndTargets.Count; nextExpectedCallIndex++)
        {
            builder.AppendLine($"Call {nextExpectedCallIndex + 1}: Not received!");
            builder.Append($"{_indent}Expected: ");
            builder.AppendLine(_expectedCallSpecAndTargets[nextExpectedCallIndex].CallSpecification.ToString());
        }
    }

    private sealed class UnexpectedCallData
    {
        public Type DeclaringType { get; }
        public string CallFormat { get; }
        public IReadOnlyList<string> NonMatchingArgumentFormats { get; }
        public int InstanceNumber { get; }

        public UnexpectedCallData(ICallSpecification? specification, ICall call, int instanceNumber)
        {
            DeclaringType = call.GetMethodInfo().DeclaringType!;

            CallFormat = FormatCall(call);

            NonMatchingArgumentFormats = specification != null
                ? FormatNonMatchingArguments(specification, call)
                : [];

            InstanceNumber = instanceNumber;
        }

        private static string FormatCall(ICall call)
        {
            // Based on SequenceFormatter, maybe we can refactor this?

            var methodInfo = call.GetMethodInfo();

            var args = methodInfo.GetParameters()
                .Zip(call.GetOriginalArguments(), (info, value) => (info, value))
                .SelectMany(x =>
                {
                    var (info, value) = x;

                    return info.IsParams()
                        ? ((IEnumerable)value!).Cast<object>()
                        : ToEnumerable(value);

                    static IEnumerable<T> ToEnumerable<T>(T value)
                    {
                        yield return value;
                    }
                })
                .Select(x => ArgumentFormatter.Default.Format(x, false))
                .ToArray();

            return CallFormatter.Default.Format(methodInfo, args);
        }
        private static string[] FormatNonMatchingArguments(ICallSpecification specification, ICall call)
        {
            var nonMatchingArguments = specification.NonMatchingArguments(call).ToArray();
            var result = new string[nonMatchingArguments.Length];

            for (var i = 0; i < nonMatchingArguments.Length; i++)
            {
                var nonMatchingArgument = nonMatchingArguments[i];
                var description = nonMatchingArgument.DescribeNonMatch();

                if (string.IsNullOrWhiteSpace(description))
                    description = $"arg[{nonMatchingArgument.Index}] not matched: {nonMatchingArgument.Specification}";

                result[i] = description;
            }

            return result;
        }
    }
}