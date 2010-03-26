using System;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class RaiseExtensionSpec
    {
        public class When_raising_an_event_on_a_substitute : StaticConcern
        {
            readonly object[] _arguments = new[] {new object(), new object()};
            private IFoo _substitute;
            private ICallHandler _callHandler;
            private Action<IFoo> _eventReference;

            public override void Context()
            {
                _eventReference = mock<Action<IFoo>>();
                var context = mock<ISubstitutionContext>();
                _substitute = mock<IFoo>();
                _callHandler = mock<ICallHandler>();
                context.stub(x => x.GetCallHandlerFor(_substitute)).Return(_callHandler);
                temporarilyChange(() => SubstitutionContext.Current).to(context);
            }

            public override void Because()
            {
                _substitute.Raise(_eventReference, _arguments);
            }

            [Test]
            public void Should_tell_call_handler_for_substitute_to_raise_event()
            {
                _callHandler.received(x => x.RaiseEventFromNextCall(_arguments));
            }

            [Test]
            public void Should_invoke_event_reference_so_call_handler_can_raise_the_event()
            {
                _eventReference.received(x => x(_substitute));
            }
        }
    }
}