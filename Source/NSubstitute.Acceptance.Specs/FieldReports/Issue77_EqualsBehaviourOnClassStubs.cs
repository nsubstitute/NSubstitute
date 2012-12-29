using NUnit.Framework;
using System.Linq;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class Issue77_BaseObjectBehaviourOnConcreteSubstitutesThatOverrideThoseMethods
    {
        [Test]
        public void Equals_should_return_true_for_same_reference_for_class_substitutes_that_overwrite_equals()
        {
            var substitute = Substitute.For<AClassThatOverwritesBaseObjectMethods>();

            Assert.That(substitute.Equals(substitute), Is.True);
        }

        [Test]
        public void Equals_should_return_true_for_same_object_for_class_substitutes_that_overwrite_equals()
        {
            var substitute = Substitute.For<AClassThatOverwritesBaseObjectMethods>();
            substitute.Id = 1;
            var substitute2 = Substitute.For<AClassThatOverwritesBaseObjectMethods>();
            substitute2.Id = 1;

            Assert.That(substitute.Equals(substitute2), Is.True);
        }

        [Test]
        public void Equals_should_return_false_for_different_objects_for_class_substitutes_that_overwrite_equals()
        {
            var substitute = Substitute.For<AClassThatOverwritesBaseObjectMethods>();
            substitute.Id = 1;
            var substitute2 = Substitute.For<AClassThatOverwritesBaseObjectMethods>();
            substitute2.Id = 2;

            Assert.That(substitute.Equals(substitute2), Is.False);
        }

        [Test]
        public void Should_be_able_to_setup_return_for_call_taking_class_substitutes_that_overwrite_equals()
        {
            var service = Substitute.For<IService>();
            var substitute = Substitute.For<AClassThatOverwritesBaseObjectMethods>();

            service.AMethod(substitute).Returns(1);

            Assert.That(service.AMethod(substitute), Is.EqualTo(1));
        }

        [Test]
        public void Should_be_able_to_check_received_call_when_taking_class_substitutes_that_overwrite_equals()
        {
            var service = Substitute.For<IService>();
            var substitute = Substitute.For<AClassThatOverwritesBaseObjectMethods>();

            service.AMethod(substitute);

            service.Received().AMethod(substitute);
        }

        [Test]
        public void Should_be_able_to_find_a_substitute_that_overrides_equals_in_a_collection()
        {
            var substitute = Substitute.For<AClassThatOverwritesBaseObjectMethods>();
            substitute.Id = 2;
            var classes = new [] {new AClassThatOverwritesBaseObjectMethods { Id = 1 }, substitute, new AClassThatOverwritesBaseObjectMethods { Id = 3 }};

            Assert.That(classes.Contains(substitute), Is.True);
        }

        [Test]
        public void Should_be_able_to_call_tostring_on_substitute_that_overrides_tostring()
        {
            var substitute = Substitute.For<AClassThatOverwritesBaseObjectMethods>();
            substitute.Id = 2;

            Assert.That(substitute.ToString(), Is.EqualTo("{Id=2}"));
        }

        [Test]
        public void Should_be_able_to_call_gethashcode_on_substitute_that_overrides_gethashcode()
        {
            var substitute = Substitute.For<AClassThatOverwritesBaseObjectMethods>();
            substitute.Id = 2;

            Assert.That(substitute.GetHashCode(), Is.EqualTo(2.GetHashCode()));
        }

        [Test]
        public void Should_be_able_to_mock_equals_against_same_object_on_substitute_that_overrides_equals()
        {
            var substitute = Substitute.For<AClassThatOverwritesBaseObjectMethods>();

            substitute.Equals(substitute).Returns(false);

            Assert.That(substitute.Equals(substitute), Is.False);
        }

        [Test]
        public void Should_be_able_to_mock_equals_on_a_sub_that_overrides_equals_against_another_substitute()
        {
            var first = Substitute.For<AClassThatOverwritesBaseObjectMethods>();
            var second = Substitute.For<AClassThatOverwritesBaseObjectMethods>();

            first.Equals(second).Returns(false);

            Assert.That(first.Equals(second), Is.False);
        }

        [Test]
        public void Should_be_able_to_mock_equals_on_substitute_that_overrides_equals()
        {
            var substitute = Substitute.For<AClassThatOverwritesBaseObjectMethods>();
            var x = new object();

            substitute.Equals(x).Returns(true);

            Assert.That(substitute.Equals(x), Is.True);
        }

        [Test]
        public void Should_be_able_to_mock_gethashcode_on_substitute_that_overrides_equals()
        {
            var substitute = Substitute.For<AClassThatOverwritesBaseObjectMethods>();

            substitute.GetHashCode().Returns(5);

            Assert.That(substitute.GetHashCode(), Is.EqualTo(5));
        }

        [Test]
        public void Should_be_able_to_mock_tostring_on_substitute_that_overrides_equals()
        {
            var substitute = Substitute.For<AClassThatOverwritesBaseObjectMethods>();

            substitute.ToString().Returns("aString");

            Assert.That(substitute.ToString(), Is.EqualTo("aString"));
        }

        public class AClassThatOverwritesBaseObjectMethods
        {
            public int Id { get; set; }

            public override bool Equals(object obj)
            {
                var aClassThatOverwritesEquals = obj as AClassThatOverwritesBaseObjectMethods;
                if (aClassThatOverwritesEquals == null)
                    return false;

                return aClassThatOverwritesEquals.Id.Equals(Id);
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }

            public override string ToString()
            {
                return string.Format("{{Id={0}}}", Id);
            }
        }

        public interface IService
        {
            int AMethod(AClassThatOverwritesBaseObjectMethods aClassThatOverwritesBaseObjectMethods);
        }
    }
}
