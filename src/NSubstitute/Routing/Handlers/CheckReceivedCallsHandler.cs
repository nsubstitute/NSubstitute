using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NSubstitute.Core;
using NSubstitute.ReceivedExtensions;

namespace NSubstitute.Routing.Handlers
{
    public class CheckReceivedCallsHandler : ICallHandler
    {
        private readonly ICallCollection _receivedCalls;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly IReceivedCallsExceptionThrower _exceptionThrower;
        private readonly MatchArgs _matchArgs;
        private readonly Quantity _requiredQuantity;
        private readonly TimeSpan _requiredTimeSpan;

        public CheckReceivedCallsHandler(ICallCollection receivedCalls, ICallSpecificationFactory callSpecificationFactory, IReceivedCallsExceptionThrower exceptionThrower, MatchArgs matchArgs, Quantity requiredQuantity, TimeSpan requiredTimeSpan)
        {
            _receivedCalls = receivedCalls;
            _callSpecificationFactory = callSpecificationFactory;
            _exceptionThrower = exceptionThrower;
            _matchArgs = matchArgs;
            _requiredQuantity = requiredQuantity;
            _requiredTimeSpan = requiredTimeSpan;
        }

        public RouteAction Handle(ICall call)
        {
            ICallSpecification callSpecification = _callSpecificationFactory.CreateFrom(call, _matchArgs);
            ICallSpecification allCallsToMethodSpec = _callSpecificationFactory.CreateFrom(call, MatchArgs.Any);

            List<ICall> allCalls = _receivedCalls.AllCalls().ToList();
            List<ICall> matchingCalls = allCalls.Where(callSpecification.IsSatisfiedBy).ToList();

            var foundMatchingCalls = SpinWait.SpinUntil(() =>
            {
                callSpecification = _callSpecificationFactory.CreateFrom(call, _matchArgs);
                allCallsToMethodSpec = _callSpecificationFactory.CreateFrom(call, MatchArgs.Any);

                allCalls = _receivedCalls.AllCalls().ToList();
                matchingCalls = allCalls.Where(callSpecification.IsSatisfiedBy).ToList();
                return _requiredQuantity.Matches(matchingCalls);
            }, _requiredTimeSpan);

            if (!foundMatchingCalls)
            {
                var relatedCalls = allCalls.Where(allCallsToMethodSpec.IsSatisfiedBy).Except(matchingCalls);
                _exceptionThrower.Throw(callSpecification, matchingCalls, relatedCalls, _requiredQuantity);
            }
            return RouteAction.Continue();
        }
    }
}