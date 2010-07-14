using NSubstitute.Routing.Definitions;
using NSubstitute.Routing.Handlers;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Definitions
{
    public class CheckCallReceivedRouteSpecs : ConcernForRoute<CheckCallReceivedRoute>
    {
        [Test]
        public void Should_check_call_received()
        {
            AssertPartHandledCall<CheckReceivedCallHandler>();
        }

        public override void Context()
        {
            base.Context();
            ExpectReturnValueToComeFromPart<ReturnDefaultForReturnTypeHandler>();
        }
    }
}