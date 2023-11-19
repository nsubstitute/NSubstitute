using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class EqualsBehaviourOnClassSubs
    {
        [Test]
        public void Equals_should_work_as_expected_for_class_substitutes()
        {
            var firstSub = Substitute.For<AClass>();
            var secondSub = Substitute.For<AClass>();

            Assert.AreEqual(firstSub, firstSub);
            Assert.AreNotEqual(firstSub, secondSub);
        }

        public class AClass { }
    }
}