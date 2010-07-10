using System;
using NSubstitute.Core;
using NSubstitute.Routes;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class WhenCalledSpecs
    {
        public class When_a_callback_is_given_to_run_for_a_call : ConcernFor<WhenCalled<IFoo>> {
            private Action<CallInfo> _callbackWithArguments;
            private ISubstitutionContext _context;
            private IFoo _substitute;
            private Action<IFoo> _call;
            private ICallRouter _callRouter;
            private MatchArgs _matchArgs;

            [Test]
            public void Should_tell_substitute_to_add_callback_for_next_call_then_invoke_call()
            {
                _callRouter.received(x => x.SetRoute<DoWhenCalledRoute>(_callbackWithArguments, _matchArgs));
                _call.received(x => x(_substitute));
            }

            public override void Because()
            {
                sut.Do(_callbackWithArguments);
            }

            public override void Context()
            {
                _call = mock<Action<IFoo>>();
                _callbackWithArguments = args => { };
                _matchArgs = MatchArgs.AsSpecifiedInCall; 

                _context = mock<ISubstitutionContext>();
                _substitute = mock<IFoo>();
                _callRouter = mock<ICallRouter>();

                _context.stub(x => x.GetCallRouterFor(_substitute)).Return(_callRouter);
            }

            public override WhenCalled<IFoo> CreateSubjectUnderTest()
            {
                return new WhenCalled<IFoo>(_context, _substitute, _call, _matchArgs);
            }
        }
    }
}