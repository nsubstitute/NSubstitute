using NSubstitute.Routes;
using NSubstitute.Routes.Handlers;
using NUnit.Framework;

namespace NSubstitute.Specs.Routes
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