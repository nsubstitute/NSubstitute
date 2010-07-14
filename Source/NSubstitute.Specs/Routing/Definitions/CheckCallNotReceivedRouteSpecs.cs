using NSubstitute.Routing.Definitions;
using NSubstitute.Routing.Handlers;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Definitions
{
    public class CheckCallNotReceivedRouteSpecs : ConcernForRoute<CheckCallNotReceivedRoute>
    {
        [Test]
        public void Should_check_did_not_receive_call()
        {
            AssertPartHandledCall<CheckDidNotReceiveCallHandler>();
        }

        public override void Context()
        {
            base.Context();
            ExpectReturnValueToComeFromPart<ReturnDefaultForReturnTypeHandler>();
        }
    }
}