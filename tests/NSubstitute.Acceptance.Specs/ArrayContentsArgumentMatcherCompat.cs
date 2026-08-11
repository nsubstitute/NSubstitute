using NSubstitute.Core.Arguments;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class ArrayContentsArgumentMatcherCompat
{
    [Test]
    [Obsolete]
    public void Should_still_work_now_it_is_deprecated()
    {
        var spec = new ArgumentSpecification(typeof(int), new EqualsArgumentMatcher(1));
        var matcher = new ArrayContentsArgumentMatcher([spec]);

        Assert.That(matcher.IsSatisfiedBy(new[] { 1 }), Is.True);
        Assert.That(matcher.IsSatisfiedBy(new[] { 2 }), Is.False);
    }
}
