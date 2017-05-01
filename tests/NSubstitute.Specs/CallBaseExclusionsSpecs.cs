using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallBaseExclusionsSpecs : ConcernFor<CallBaseExclusions>
    {
        public override CallBaseExclusions CreateSubjectUnderTest() { return new CallBaseExclusions(); }

        [Test]
        public void Exclude_call()
        {
            var call = mock<ICall>();
            var spec = mock<ICallSpecification>();
            spec.stub(x => x.IsSatisfiedBy(call)).Return(true);
            sut.Exclude(spec);

            Assert.That(sut.IsExcluded(call), Is.True);
        }

        [Test]
        public void Non_excluded_call()
        {
            var call = mock<ICall>();

            Assert.That(sut.IsExcluded(call), Is.False);
        }
    }
}