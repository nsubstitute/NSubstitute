using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class SubstituteTimingAndInteractions
{
    public class When_updating_a_property_on_a_sub_from_an_event_handler
    {
        public interface IICapn
        {
            event Action HoistTheMainSail;
            bool IsMainSailHoisted { get; set; }
        }

        [Test]
        public void Should_update_the_property_when_the_event_is_raised()
        {
            var firstMate = Substitute.For<IICapn>();
            firstMate.HoistTheMainSail += () => firstMate.IsMainSailHoisted = true;

            firstMate.HoistTheMainSail += Raise.Event<Action>();
            Assert.That(firstMate.IsMainSailHoisted, Is.True);
        }
    }

    public class When_unsubscribing_from_an_event_from_within_an_event_handler
    {
        public interface IHaveEvents { event EventHandler Event; }
        IHaveEvents sub;
        int removeHandlerCallCount;

        [SetUp]
        public void SetUp() { sub = Substitute.For<IHaveEvents>(); }

        [Test]
        public void Should_remove_handler_after_first_event_is_raised()
        {
            sub.Event += RemoveThisEventSubscriptionWhenRaised;
            sub.Event += Raise.Event();
            sub.Event += Raise.Event();
            Assert.That(removeHandlerCallCount, Is.EqualTo(1));
        }

        void RemoveThisEventSubscriptionWhenRaised(object sender, EventArgs e)
        {
            removeHandlerCallCount++;
            sub.Event -= RemoveThisEventSubscriptionWhenRaised;
        }
    }

    public class When_setting_the_return_value_of_one_sub_within_the_call_to_set_a_return_on_another
    {

        public interface IWidget { string GetName(); }
        public interface IWidgetFactory { IWidget CreateWidget(); }

        [Test]
        [Pending, Explicit]
        public void Should_set_both_calls_to_return_the_specified_values()
        {
            const string widgetName = "widget x";
            var factory = Substitute.For<IWidgetFactory>();
            factory.CreateWidget().Returns(CreateSubstituteForWidget(widgetName));

            Assert.That(factory.CreateWidget().GetName(), Is.EqualTo(widgetName));
        }

        IWidget CreateSubstituteForWidget(string widgetName)
        {
            var widget = Substitute.For<IWidget>();
            widget.GetName().Returns(widgetName);
            return widget;
        }
    }
}