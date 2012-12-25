using NUnit.Framework;
using System.Linq;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class Issue77_BaseObjectBehaviourOnConcreteSubstitutesThatOverrideThoseMethods
    {
        [Test]
        public void Equals_should_return_true_for_same_reference_for_class_substitutes_that_overwrite_equals()
        {
            var firstSub = Substitute.For<AClassThatOverwritesEquals>();

            Assert.That(firstSub.Equals(firstSub), Is.True);
        }

        [Test]
        public void Equals_should_return_true_for_same_object_for_class_substitutes_that_overwrite_equals()
        {
            var firstSub = Substitute.For<AClassThatOverwritesEquals>();
            firstSub.Id = 1;
            var secondSub = Substitute.For<AClassThatOverwritesEquals>();
            secondSub.Id = 1;

            Assert.That(firstSub.Equals(secondSub), Is.True);
        }

        [Test]
        public void Equals_should_return_false_for_different_objects_for_class_substitutes_that_overwrite_equals()
        {
            var firstSub = Substitute.For<AClassThatOverwritesEquals>();
            firstSub.Id = 1;
            var secondSub = Substitute.For<AClassThatOverwritesEquals>();
            secondSub.Id = 2;

            Assert.That(firstSub.Equals(secondSub), Is.False);
        }

        [Test]
        public void Should_be_able_to_setup_return_for_call_taking_class_substitutes_that_overwrite_equals()
        {
            var service = Substitute.For<IService>();
            var substitute = Substitute.For<AClassThatOverwritesEquals>();

            service.AMethod(substitute).Returns(1);

            Assert.That(service.AMethod(substitute), Is.EqualTo(1));
        }

        [Test]
        public void Should_be_able_to_check_received_call_when_taking_class_substitutes_that_overwrite_equals()
        {
            var service = Substitute.For<IService>();
            var substitute = Substitute.For<AClassThatOverwritesEquals>();

            service.AMethod(substitute);

            service.Received().AMethod(substitute);
        }

        [Test]
        public void Should_be_able_to_find_a_substitute_that_overrides_equals_in_a_collection()
        {
            var substitute = Substitute.For<AClassThatOverwritesEquals>();
            substitute.Id = 2;
            var classes = new [] {new AClassThatOverwritesEquals { Id = 1 }, substitute, new AClassThatOverwritesEquals { Id = 3 }};

            Assert.That(classes.Contains(substitute), Is.True);
        }

        [Test]
        public void Should_be_able_to_call_tostring_on_substitute_that_overrides_tostring()
        {
            var substitute = Substitute.For<AClassThatOverwritesEquals>();
            substitute.Id = 2;

            Assert.That(substitute.ToString(), Is.EqualTo("{Id=2}"));
        }

        [Test]
        public void Should_be_able_to_call_gethashcode_on_substitute_that_overrides_gethashcode()
        {
            var substitute = Substitute.For<AClassThatOverwritesEquals>();
            substitute.Id = 2;

            Assert.That(substitute.GetHashCode(), Is.EqualTo(2.GetHashCode()));
        }

        public class AClassThatOverwritesEquals
        {
            public int Id { get; set; }

            public override bool Equals(object obj)
            {
                var aClassThatOverwritesEquals = obj as AClassThatOverwritesEquals;
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
            int AMethod(AClassThatOverwritesEquals aClassThatOverwritesEquals);
        }

        [Test]
        [Pending]
        public void ArgumentMatchers_should_work_with_substituted_equals()
        {
            var firstSub = Substitute.For<AClassThatOverwritesEquals>();
            firstSub.Equals(Arg.Is<object>(x => ReferenceEquals(x, firstSub))).Returns(true);
            var secondSub = Substitute.For<AClassThatOverwritesEquals>();
            secondSub.Equals(Arg.Is<object>(x => ReferenceEquals(x, secondSub))).Returns(true);

            var service = Substitute.For<IService>();
            service.AMethod(firstSub).Returns(1);
            service.AMethod(secondSub).Returns(2);

            Assert.That(service.AMethod(firstSub), Is.EqualTo(1));
            Assert.That(service.AMethod(secondSub), Is.EqualTo(2));
        }
    }
}
