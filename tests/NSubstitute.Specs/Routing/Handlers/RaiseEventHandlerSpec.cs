using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class RaiseEventHandlerSpec
    {
        public abstract class Concern : ConcernFor<RaiseEventHandler>
        {
            private const string SampleEventName = "Sample";
            private IEventHandlerRegistry _eventHandlerRegistry;
            private Func<ICall, object[]> _getEventArguments;
            protected IList<object> _handlers;

            protected EventHandler<EventArgs> EventHandler;
            protected object[] EventArguments;
            protected ICall Call;

            public override void Context()
            {
                var subscribeMethodInfo = typeof(SampleClassWithEvent).GetEvent(SampleEventName).GetAddMethod();
                Call = CreateCall(subscribeMethodInfo, new object[0]);

                EventArguments = new[] { new object(), EventArgs.Empty };
                _getEventArguments = mock<Func<ICall, object[]>>();
                _getEventArguments.stub(x => x(Call)).Return(EventArguments);

                EventHandler = mock<EventHandler<EventArgs>>();
                _handlers = new object[] { EventHandler };
                _eventHandlerRegistry = mock<IEventHandlerRegistry>();
                _eventHandlerRegistry.Stub(x => x.GetHandlers(SampleEventName)).Return(_handlers);
            }

            public override RaiseEventHandler CreateSubjectUnderTest()
            {
                return new RaiseEventHandler(_eventHandlerRegistry, _getEventArguments);
            }

            protected ICall CreateCall(MethodInfo subscribeMethodInfo, object[] arguments)
            {
                var call = mock<ICall>();
                call.stub(x => x.GetMethodInfo()).Return(subscribeMethodInfo);
                call.stub(x => x.GetArguments()).Return(arguments);
                return call;
            }
        }

        public class When_raising_an_event_from_a_call : Concern
        {
            private RouteAction _result;

            [Test]
            public void Should_raise_event_with_arguments()
            {
                EventHandler.AssertWasCalled(x => x.Invoke(EventArguments[0], (EventArgs) EventArguments[1]));
            }

            [Test]
            public void Should_continue_route()
            {
                Assert.That(_result, Is.SameAs(RouteAction.Continue()));
            }

            public override void Because()
            {
                _result = sut.Handle(Call);
            }
        }

        public class When_raising_an_event_and_event_handler_throws_an_exception : Concern
        {
            public override void Context()
            {
                base.Context();
                EventHandler handler = (sender, eventArgs) => { throw new Exception(); };
                _handlers[0] = handler;
            }

            [Test]
            public void Should_throw_original_exception()
            {
                Assert.Throws<Exception>(() => sut.Handle(Call));
            }
        }

        class SampleClassWithEvent
        {
            public event EventHandler Sample;
            public void OnSample(EventArgs e) { var handler = Sample; if (handler != null) handler(this, e); }
            public void NonEventMethod() { }
        }
    }
}
