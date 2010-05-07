using NUnit.Framework;

namespace NSubstitute.Specs.Infrastructure.Tests
{
    public class BaseConcernSpecs
    {
        private class StaticClassWithPersistentMembers
        {
            public const int InitialValue = 10;
            public static int APersistentField = InitialValue;
            public static int APersistentProperty { get; private set; }   
            static StaticClassWithPersistentMembers()
            {
                APersistentProperty = InitialValue;
            }
        }

        [TestFixture]
        public class When_a_temporary_change_to_a_persistence_value_is_requested
        {
            [Test]
            public void Should_change_property_after_setup_and_revert_property_on_teardown()
            {
                var concern = new TestConcern();
                Assert.That(StaticClassWithPersistentMembers.APersistentProperty, Is.EqualTo(StaticClassWithPersistentMembers.InitialValue));
                concern.SetUp();
                Assert.That(StaticClassWithPersistentMembers.APersistentProperty, Is.EqualTo(TestConcern.NewValue));
                concern.TearDown();
                Assert.That(StaticClassWithPersistentMembers.APersistentProperty, Is.EqualTo(StaticClassWithPersistentMembers.InitialValue));
            }

            [Test]
            public void Should_change_field_after_setup_and_revert_field_on_teardown()
            {
                var concern = new TestConcern();
                Assert.That(StaticClassWithPersistentMembers.APersistentField, Is.EqualTo(StaticClassWithPersistentMembers.InitialValue));
                concern.SetUp();
                Assert.That(StaticClassWithPersistentMembers.APersistentField, Is.EqualTo(TestConcern.NewValue));
                concern.TearDown();
                Assert.That(StaticClassWithPersistentMembers.APersistentField, Is.EqualTo(StaticClassWithPersistentMembers.InitialValue));                
            }
            
            private class TestConcern : StaticConcern
            {
                public const int NewValue = 7;

                public override void Context()
                {
                    temporarilyChange(() => StaticClassWithPersistentMembers.APersistentField).to(NewValue);
                    temporarilyChange(() => StaticClassWithPersistentMembers.APersistentProperty).to(NewValue);
                }
            }
        }

        [TestFixture]
        public class When_a_temporary_change_is_not_configured_properly
        {
            [Test]
            public void Should_throw_an_exception()
            {
                var concern = new TestConcern();
                Assert.Throws<TemporaryChangeNotConfiguredProperlyException>(() => concern.SetUp());                
            }

            private class TestConcern : StaticConcern
            {
                public override void Context()
                {
                    temporarilyChange(() => StaticClassWithPersistentMembers.APersistentField);
                }
            }
        }
    }
}