using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class EventHandlerRegistrySpec

    {
        public class Concern : ConcernFor<EventHandlerRegistry>
        {
            protected object _handler;
            protected const string SampleEventName = "Sample";

            public override void Context()
            {
                _handler = new object();
            }

            public override EventHandlerRegistry CreateSubjectUnderTest()
            {
                return new EventHandlerRegistry();
            }
        }
        
        public class When_adding_an_event_subscription : Concern
        {
            public override void Because()
            {
                sut.Add(SampleEventName, _handler);
            }

            [Test]
            public void Should_return_handler_for_event()
            {
                Assert.That(sut.GetHandlers(SampleEventName), Has.Member(_handler));
            }            
        }

        public class When_removing_an_event_subscription : Concern
        {
            public override void Because()
            {
                sut.Add(SampleEventName, _handler);
                sut.Remove(SampleEventName, _handler);
            }

            [Test]
            public void Should_not_return_handler_for_event()
            {
                Assert.That(sut.GetHandlers(SampleEventName), Has.No.Member(_handler));
            }                
        }
    }
}