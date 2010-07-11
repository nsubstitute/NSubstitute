using NSubstitute.Routes;
using NSubstitute.Routes.Handlers;
using NUnit.Framework;

namespace NSubstitute.Specs.Routes
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