using System;
using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ReceivedCallsExtensionSpec 
    {
        public class When_getting_received_calls_for_a_substitute : StaticConcern
        {
            private object _substitute;
            private IEnumerable<ICall> _allCalls;
            private IEnumerable<ICall> _result;

            public override void Context()
            {
                _substitute = new object();
                _allCalls = new ICall[0];
                var callRouter = mock<ICallRouter>();
                callRouter.stub(x => x.ReceivedCalls()).Return(_allCalls);
                var context = mock<ISubstitutionContext>();
                context.stub(x => x.GetCallRouterFor(_substitute)).Return(callRouter);
                temporarilyChange(() => SubstitutionContext.Current).to(context);
            }

            public override void Because()
            {
                _result = _substitute.ReceivedCalls();
            }

            [Test]
            public void Should_return_all_received_calls_from_substitute()
            {
                Assert.That(_result, Is.SameAs(_allCalls));
            }
        }
    }
}