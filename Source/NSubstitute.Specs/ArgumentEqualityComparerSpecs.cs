using System.Collections;
using System.Collections.Generic;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ArgumentEqualityComparerSpecs : ConcernFor<IEqualityComparer>
    {

        [Test]
        public void Should_match_when_arguments_have_same_reference()
        {
            var list = new List<int>();
            var list2 = (IList)list;
            Assert.That(sut.Equals(list, list2));
        }

        [Test]
        public void Should_match_when_value_type_arguments_have_same_value()
        {            
            Assert.That(sut.Equals(1, 1));
        }

        [Test]
        public void Should_not_match_when_reference_type_arguments_have_different_references()
        {
            Assert.False(sut.Equals(new object(), new object()));
        }

        [Test]
        public void Should_match_when_both_arguments_are_null()
        {
            string x = null;
            List<int> y = null;
            Assert.That(sut.Equals(x, y));
        }

        public override IEqualityComparer CreateSubjectUnderTest()
        {
            return new ArgumentEqualityComparer();
        }
    }
}