using System.Linq;
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

        public class When_adding_a_handler_to_registry_while_iterating_over_handlers : Concern
        {
            int handlersIteratedOver;

            public override void Because()
            {
                sut.Add(SampleEventName, _handler);
                handlersIteratedOver = 0;
                foreach (var handler in sut.GetHandlers(SampleEventName))
                {
                    handlersIteratedOver++;
                    sut.Add(SampleEventName, _handler);
                }
            }

            [Test]
            public void Should_only_iterate_over_initial_handler()
            {
                Assert.That(handlersIteratedOver, Is.EqualTo(1));
            }

            [Test]
            public void Should_contain_both_handlers_after_iterating()
            {
                Assert.That(sut.GetHandlers(SampleEventName).Count(), Is.EqualTo(2));
            }
        }
    }
}