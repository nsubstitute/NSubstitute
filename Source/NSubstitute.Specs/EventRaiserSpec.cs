using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSubstitute.Specs
{
    public class EventRaiserSpec
    {
        public class Concern : ConcernFor<EventRaiser>
        {
            protected IEventHandlerRegistry _eventHandlerRegistry;

            public override void Context()
            {
                _eventHandlerRegistry = mock<IEventHandlerRegistry>();
            }

            public override EventRaiser CreateSubjectUnderTest()
            {
                return new EventRaiser(_eventHandlerRegistry);
            }
        }

        public class When_raising_an_event : Concern
        {
            private const string SampleEventName = "Sample";
            EventHandler<EventArgs> _eventHandler;
            ICall _call;
            object[] _eventArguments;
            IEnumerable<object> _handlers;

            public override void Context()
            {
                base.Context();
                var subscribeMethodInfo = typeof(SampleClassWithEvent).GetEvent(SampleEventName).GetAddMethod();
                _call = CreateCall(subscribeMethodInfo, new object[0]);

                _eventArguments = new[] { new object(), EventArgs.Empty };

                _eventHandler = mock<EventHandler<EventArgs>>();
                _handlers = new object[] { _eventHandler };
                _eventHandlerRegistry.Stub(x => x.GetHandlers(SampleEventName)).Return(_handlers);
            }

            public override void Because()
            {
                sut.Raise(_call, _eventArguments);
            }

            [Test]
            public void Should_call_event_with_arguments()
            {
                _eventHandler.AssertWasCalled(x => x.Invoke(_eventArguments[0], (EventArgs) _eventArguments[1]));
            }

            protected ICall CreateCall(MethodInfo subscribeMethodInfo, object[] arguments)
            {
                var call = mock<ICall>();
                call.stub(x => x.GetMethodInfo()).Return(subscribeMethodInfo);
                call.stub(x => x.GetArguments()).Return(arguments);
                return call;
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