using NSubstitute.Routes;
using NSubstitute.Routes.Handlers;
using NUnit.Framework;

namespace NSubstitute.Specs.Routes
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
            ExpectReturnValueToComeFromPart<ReturnDefaultResultHandler>();
        }
    }
}