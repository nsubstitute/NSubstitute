using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;
using NSubstitute.Proxies.DelegateProxy;

namespace NSubstitute.Core
{
    public class SequenceInOrderAssertion
    {
        private readonly CallFormatter _callFormatter;
        private readonly ArgumentFormatter _argumentFormatter;

        public SequenceInOrderAssertion()
        {
            _callFormatter = new CallFormatter();
            _argumentFormatter = new ArgumentFormatter();
        }

        public void Assert(IQueryResults queryResult)
        {
            var matchingCallsInOrder = queryResult
                .MatchingCallsInOrder()
                .Where(x => IsNotPropertyGetterCall(x.GetMethodInfo()))
                .ToArray();
            var querySpec = queryResult
                .QuerySpecification()
                .Where(x => IsNotPropertyGetterCall(x.CallSpecification.GetMethodInfo()))
                .ToArray();

            if (matchingCallsInOrder.Length != querySpec.Length)
            {
                throw new CallSequenceNotFoundException(GetExceptionMessage(querySpec, matchingCallsInOrder));
            }

            var callsAndSpecs = matchingCallsInOrder
                .Zip(querySpec, (call, specAndTarget) =>
                                new
                                    {
                                        Call = call,
                                        Spec = specAndTarget.CallSpecification,
                                        IsMatch = Matches(call, specAndTarget)
                                    }
                );

            if (callsAndSpecs.Any(x => !x.IsMatch))
            {
                throw new CallSequenceNotFoundException(GetExceptionMessage(querySpec, matchingCallsInOrder));
            }
        }

        private string GetExceptionMessage(CallSpecAndTarget[] querySpec, ICall[] matchingCallsInOrder)
        {
            const string callDelimiter = "\n    ";
            var instanceLookup = new TypeInstanceNumberLookup();
            var isAcrossMultipleTargets = IsAcrossMultipleTargets(querySpec);
            var formattedQuery = string.Join(callDelimiter,
                                             querySpec.Select(x => FormatCallSpec(x, isAcrossMultipleTargets, instanceLookup)).ToArray());
            var formattedCalls = string.Join(callDelimiter,
                                             matchingCallsInOrder.Select(x => FormatCall(x, isAcrossMultipleTargets, instanceLookup)).ToArray());
            return string.Format("\nExpected to receive these calls in order:{0}{1}\n" +
                                 "Actually received matching calls in this order:{0}{2}\n\n{3}",
                                 callDelimiter,
                                 formattedQuery,
                                 formattedCalls,
                                 "*** Note: calls to property getters are not considered part of the query. ***");
        }

        private string FormatCall(ICall call, bool isAcrossMultipleTargets, TypeInstanceNumberLookup instanceLookup)
        {
            var s = _callFormatter.Format(call.GetMethodInfo(), FormatArgs(call.GetArguments()));
            if (!isAcrossMultipleTargets) return s;

            var target = call.Target();
            var methodInfo = call.GetMethodInfo();
            return FormatCallForInstance(instanceLookup, target, methodInfo, s);
        }

        private string FormatCallSpec(CallSpecAndTarget callSpecAndTarget, bool isAcrossMultipleTargets, TypeInstanceNumberLookup instanceLookup)
        {
            var s = callSpecAndTarget.CallSpecification.ToString();
            if (!isAcrossMultipleTargets) return s;

            var target = callSpecAndTarget.Target;
            var methodInfo = callSpecAndTarget.CallSpecification.GetMethodInfo();
            return FormatCallForInstance(instanceLookup, target, methodInfo, s);
        }

        private static string FormatCallForInstance(TypeInstanceNumberLookup instanceLookup, object target, MethodInfo methodInfo, string s)
        {
            var instanceNumber = instanceLookup.GetInstanceNumberFor(target);
            var declaringType = methodInfo.DeclaringType;
            var declaringTypeName = declaringType == typeof (DelegateCall) ? target.ToString() : declaringType.Name;
            return string.Format("{0}#{1}.{2}", declaringTypeName, instanceNumber, s);
        }

        private bool IsAcrossMultipleTargets(CallSpecAndTarget[] querySpec)
        {
            if (!querySpec.Any()) return false;
            var firstTarget = querySpec.First().Target;
            return querySpec.Any(x => !ReferenceEquals(firstTarget, x.Target));
        }

        private IEnumerable<string> FormatArgs(object[] arguments)
        {
            return arguments.Select(x => _argumentFormatter.Format(x, false));
        }

        private bool Matches(ICall call, CallSpecAndTarget specAndTarget)
        {
            return ReferenceEquals(call.Target(), specAndTarget.Target)
                   && specAndTarget.CallSpecification.IsSatisfiedBy(call);
        }

        private bool IsNotPropertyGetterCall(MethodInfo methodInfo)
        {
            return methodInfo.GetPropertyFromGetterCallOrNull() == null;
        }
    }
}