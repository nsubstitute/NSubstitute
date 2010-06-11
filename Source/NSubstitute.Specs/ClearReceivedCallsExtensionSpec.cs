using System;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ClearReceivedCallsExtensionSpec
    {
        public class When_clearing_calls_on_a_substitute : StaticConcern
        {
            private ICallRouter _callRouter;
            private object _substitute;

            [Test]
            public void Should_clear_calls_on_substitutes_call_router()
            {
                _callRouter.received(x => x.ClearReceivedCalls()); 
            }

            public override void Because()
            {
                _substitute.ClearReceivedCalls();
            }

            public override void Context()
            {
                _substitute = new object();
                _callRouter = mock<ICallRouter>();
                var substitutionContext = mock<ISubstitutionContext>();
                substitutionContext.stub(x => x.GetCallRouterFor(_substitute)).Return(_callRouter);
                temporarilyChange(() => SubstitutionContext.Current).to(substitutionContext);
            }
        }   
    }
}