using System;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class EventSubscriptionHandlerSpec
    {
        public class Concern : ConcernFor<EventSubscriptionHandler>
        {
            protected IEventHandlerRegistry _eventHandlerRegistry;

            public override void Context()
            {
                _eventHandlerRegistry = mock<IEventHandlerRegistry>();   
               
            }

            public override EventSubscriptionHandler CreateSubjectUnderTest()
            {
                return new EventSubscriptionHandler(_eventHandlerRegistry);
            }

            protected ICall CreateCall(MethodInfo subscribeMethodInfo, object[] arguments)
            {
                var call = mock<ICall>();
                call.stub(x => x.GetMethodInfo()).Return(subscribeMethodInfo);
                call.stub(x => x.GetArguments()).Return(arguments);
                return call;
            }
        }
        
        public class When_handling_an_event_subscription : Concern
        {
            private const string SampleEventName = "Sample";
            private ICall _eventSubscription;
            private object _handlerArgument;
            private RouteAction _result;

            public override void Context()
            {
                base.Context();
                var subscribeMethodInfo = typeof (SampleClassWithEvent).GetEvent(SampleEventName).GetAddMethod();
                _handlerArgument = new object();
                _eventSubscription = CreateCall(subscribeMethodInfo, new[] {_handlerArgument});
            }

            public override void Because()
            {
                _result = sut.Handle(_eventSubscription);
            }

            [Test]
            public void Should_add_handler_for_event()
            {
                _eventHandlerRegistry.received(x => x.Add(SampleEventName, _handlerArgument));
            }

            [Test]
            public void Should_continue_route()
            {
                Assert.That(_result, Is.SameAs(RouteAction.Continue()));
            }

        }

        public class When_handling_an_event_unsubscription : Concern
        {
            private const string SampleEventName = "Sample";
            private ICall _eventUnsubscription;
            private object _handlerArgument;
            private RouteAction _result;

            public override void Context()
            {
                base.Context();
                var subscribeMethodInfo = typeof(SampleClassWithEvent).GetEvent(SampleEventName).GetRemoveMethod();
                _handlerArgument = new object();
                _eventUnsubscription = CreateCall(subscribeMethodInfo, new[] { _handlerArgument });
            }

            public override void Because()
            {
                _result = sut.Handle(_eventUnsubscription);
            }

            [Test]
            public void Should_remove_handler_for_event()
            {
                _eventHandlerRegistry.received(x => x.Remove(SampleEventName, _handlerArgument));
            }                

            [Test]
            public void Should_continue_route()
            {
                Assert.That(_result, Is.SameAs(RouteAction.Continue()));
            }
        }

        public class When_call_is_not_an_event_subscription_or_removal :Concern
        {
            private ICall _nonEventCall;
            private RouteAction _result;

            public override void Context()
            {
                base.Context();
                var nonEventMethodInfo = typeof (SampleClassWithEvent).GetMethod("NonEventMethod");
                _nonEventCall = CreateCall(nonEventMethodInfo, new object[0]);
            }

            public override void Because()
            {
                _result = sut.Handle(_nonEventCall);
            }

            [Test]
            public void Should_not_add_or_remove_a_handler_for_event()
            {
                _eventHandlerRegistry.did_not_receive(x => x.Add(Arg<string>.Is.Anything, Arg<object>.Is.Anything));
            }

            [Test]
            public void Should_continue_route()
            {
                Assert.That(_result, Is.SameAs(RouteAction.Continue()));
            }
        }

        class SampleClassWithEvent
        {
            public event EventHandler Sample;
            public void OnSample(EventArgs e) { var handler = Sample; if (handler != null) handler(this, e); }
            public void NonEventMethod() {}
        }
    }
}