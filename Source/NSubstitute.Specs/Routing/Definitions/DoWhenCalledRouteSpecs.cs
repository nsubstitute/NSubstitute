using NSubstitute.Routing.Definitions;
using NSubstitute.Routing.Handlers;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Definitions
{
    public class DoWhenCalledRouteSpecs : ConcernForRoute<DoWhenCalledRoute>
    {
        [Test]
        public void Should_set_action_for_call_specification()
        {
            AssertPartHandledCall<SetActionForCallHandler>();
        }

        public override void Context()
        {
            base.Context();
            ExpectReturnValueToComeFromPart<ReturnDefaultForReturnTypeHandler>();
        }
    }
}