using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    // todo: rename this to reflect the fact it's not just equals
    public class Issue77_EqualsBehaviourOnClassSubs
    {
        [Test]
        public void Equals_should_work_as_expected_for_class_substitutes_that_overwrites_it()
        {
            var firstSub = Substitute.For<AClassThatOverwritesEquals>();
            var secondSub = Substitute.For<AClassThatOverwritesEquals>();

            Assert.That(firstSub.Equals(firstSub), Is.True);
            Assert.That(firstSub.Equals(secondSub), Is.Not.True);
        }

        public class AClassThatOverwritesEquals
        {
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        // todo: check class with more complicated equals
        // todo: check gethashcode and tostring work
        // todo: check that a substitute within a collection can be discovered

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

        public interface IService
        {
            int AMethod(AClassThatOverwritesEquals aClassThatOverwritesEquals);
        }
    }
}
