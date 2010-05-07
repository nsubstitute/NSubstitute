using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Routes.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSubstitute.Specs.Routes.Handlers
{
    public class RaiseEventHandlerSpec
    {
        public class When_raising_an_event_from_a_call : ConcernFor<RaiseEventHandler>
        {
            private const string SampleEventName = "Sample";
            IEventHandlerRegistry _eventHandlerRegistry;
            EventHandler<EventArgs> _eventHandler;
            ICall _call;
            object[] _eventArguments;
            Func<ICall, object[]> _getEventArguments;
            IEnumerable<object> _handlers;

            [Test]
            public void Should_raise_event_with_arguments()
            {
                _eventHandler.AssertWasCalled(x => x.Invoke(_eventArguments[0], (EventArgs) _eventArguments[1]));
            }

            public override void Because()
            {
                sut.Handle(_call);
            }

            public override void Context()
            {
                var subscribeMethodInfo = typeof(SampleClassWithEvent).GetEvent(SampleEventName).GetAddMethod();
                _call = CreateCall(subscribeMethodInfo, new object[0]);

                _eventArguments = new[] { new object(), EventArgs.Empty };
                _getEventArguments = mock <Func<ICall, object[]>>();
                _getEventArguments.stub(x => x(_call)).Return(_eventArguments);

                _eventHandler = mock<EventHandler<EventArgs>>();
                _handlers = new object[] { _eventHandler };
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

        class SampleClassWithEvent
        {
            public event EventHandler Sample;
            public void OnSample(EventArgs e) { var handler = Sample; if (handler != null) handler(this, e); }
            public void NonEventMethod() { }
        }
    }
}