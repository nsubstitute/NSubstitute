using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class EqualsBehaviourOnClassSubs
{
    [Test]
    public void Equals_should_work_as_expected_for_class_substitutes()
    {
        var firstSub = Substitute.For<AClass>();
        var secondSub = Substitute.For<AClass>();

        ClassicAssert.AreEqual(firstSub, firstSub);
        ClassicAssert.AreNotEqual(firstSub, secondSub);
    }

    public class AClass { }
}