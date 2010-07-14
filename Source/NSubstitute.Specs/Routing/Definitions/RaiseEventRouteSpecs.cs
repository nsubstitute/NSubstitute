using NSubstitute.Routing.Definitions;
using NSubstitute.Routing.Handlers;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Definitions
{
    public class RaiseEventRouteSpecs : ConcernForRoute<RaiseEventRoute>
    {
        [Test]
        public void Should_raise_event()
        {
            AssertPartHandledCall<RaiseEventHandler>();
        }

        public override void Context()
        {
            base.Context();
            ExpectReturnValueToComeFromPart<ReturnDefaultForReturnTypeHandler>();
        }
    }
}