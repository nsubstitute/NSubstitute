using System;
using NSubstitute.ClearExtensions;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ClearSubstituteExtensionSpec
    {
        public class When_clearing_substitute : StaticConcern
        {
            private const ClearOptions ClearOptions = (ClearOptions) 42;
            private ICallRouter _callRouter;
            private object _substitute;

            [Test]
            public void Should_pass_on_clear_options_to_substitutes_call_router()
            {
                _callRouter.received(x => x.Clear(ClearOptions));
            }

            public override void Because()
            {
                _substitute.ClearSubstitute(ClearOptions);
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