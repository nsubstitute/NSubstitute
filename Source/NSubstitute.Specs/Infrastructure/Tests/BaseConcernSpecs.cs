using NUnit.Framework;

namespace NSubstitute.Specs.TestInfrastructure.Tests
{
    public class BaseConcernSpecs
    {
        [TestFixture]
        public class When_a_persistent_property_is_changed
        {
            [Test]
            public void Should_revert_property_on_teardown()
            {
                var concern = new TestConcern();
                Assert.That(StaticClassWithPersistentProperty.APersistentProperty, Is.EqualTo(StaticClassWithPersistentProperty.InitialPropertyValue));
                concern.SetUp();
                Assert.That(StaticClassWithPersistentProperty.APersistentProperty, Is.EqualTo(TestConcern.NewPropertyValue));
                concern.TearDown();
                Assert.That(StaticClassWithPersistentProperty.APersistentProperty, Is.EqualTo(StaticClassWithPersistentProperty.InitialPropertyValue));
            }
            
            private class TestConcern : StaticConcern
            {
                public const int NewPropertyValue = 7;

                public override void Context()
                {
                    temporarilyChange(StaticClassWithPersistentProperty.APersistentProperty)
                        .to(NewPropertyValue)
                        .via(x => StaticClassWithPersistentProperty.APersistentProperty = x);
                }
            }

            private class StaticClassWithPersistentProperty
            {
                public const int InitialPropertyValue = 10;
                public static int APersistentProperty = InitialPropertyValue;
            }
        }
    }
}