using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class SubstituteTimingAndInteractions
    {
        [Test]
        public void Update_a_property_on_a_substitute_while_it_is_raising_an_event()
        {
            var firstMate = Substitute.For<IICapn>();
            firstMate.HoistTheMainSail += () => firstMate.IsMainSailHoisted = true;

            firstMate.HoistTheMainSail += Raise.Action();
            Assert.That(firstMate.IsMainSailHoisted, Is.True);
        }   

        public interface IICapn
        {
            event Action HoistTheMainSail;
            bool IsMainSailHoisted { get; set; }
        }
    }
}