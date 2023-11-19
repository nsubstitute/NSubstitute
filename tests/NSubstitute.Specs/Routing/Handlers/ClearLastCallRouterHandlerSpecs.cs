using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class ClearLastCallRouterHandlerSpecs
    {
        public class When_handling_a_call : ConcernFor<ClearLastCallRouterHandler>
        {
            private ISubstitutionContext _context;
            private RouteAction _result;

            public override void Because()
            {
                _result = sut.Handle(mock<ICall>());
            }

            [Test]
            public void Clear_last_call_router()
            {
                _context.received(x => x.ClearLastCallRouter());
            }

            [Test]
            public void Continues_route()
            {
                Assert.That(_result, Is.EqualTo(RouteAction.Continue()));
            }

            public override void Context() { _context = mock<ISubstitutionContext>(); }

            public override ClearLastCallRouterHandler CreateSubjectUnderTest()
            {
                return new ClearLastCallRouterHandler(_context);
            }
        }
    }
}