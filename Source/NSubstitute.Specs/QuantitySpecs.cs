using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class QuantitySpecs
    {
        public static IEnumerable<int> Items(int count) { return Enumerable.Range(0, count); }

        public class At_least_one : ConcernFor<Quantity>
        {
            [Test] public void Should_match_one_item() { Assert.That(sut.Matches(Items(1))); }
            [Test] public void Should_match_more_than_one_item() { Assert.That(sut.Matches(Items(2))); }
            [Test] public void Should_not_match_less_than_one_item() { Assert.False(sut.Matches(Items(0))); }
            [Test] public void Should_require_more_than_zero_items() { Assert.That(sut.RequiresMoreThan(Items(0))); }
            [Test] public void Should_not_require_more_than_one_item() { Assert.False(sut.RequiresMoreThan(Items(1))); }
            [Test] public void Describe() { Assert.That(sut.Describe("call", "calls"), Is.EqualTo("a call")); }

            public override Quantity CreateSubjectUnderTest() { return Quantity.AtLeastOne(); }
        }

        public class None : ConcernFor<Quantity>
        {
            [Test] public void Should_not_match_one_item() { Assert.False(sut.Matches(Items(1))); }
            [Test] public void Should_not_match_more_than_one_item() { Assert.False(sut.Matches(Items(2))); }
            [Test] public void Should_match_less_than_one_item() { Assert.That(sut.Matches(Items(0))); }
            [Test] public void Should_not_require_more_than_zero_items() { Assert.False(sut.RequiresMoreThan(Items(0))); }
            [Test] public void Describe() { Assert.That(sut.Describe("call", "calls"), Is.EqualTo("no calls")); }
            public override Quantity CreateSubjectUnderTest() { return Quantity.None(); }
        }

        public class Exact_quantities : ConcernFor<Quantity>
        {
            public override Quantity CreateSubjectUnderTest() { return Quantity.Exactly(5); }
            [Test] public void Should_match_required_number_of_items() { Assert.That(sut.Matches(Items(5))); }
            [Test] public void Should_not_match_more_than_required_number_of_items() { Assert.False(sut.Matches(Items(6))); }
            [Test] public void Should_not_match_less_than_required_number_of_items() { Assert.False(sut.Matches(Items(4))); }
            [Test] public void Should_require_more_than_four_items() { Assert.That(sut.RequiresMoreThan(Items(4))); }
            [Test] public void Should_not_require_more_than_five_items() { Assert.False(sut.RequiresMoreThan(Items(5))); }
            [Test] public void Should_not_require_more_than_six_items() { Assert.False(sut.RequiresMoreThan(Items(6))); }

            [Test] public void Describe_many() { Assert.That(Quantity.Exactly(3).Describe("call", "calls"), Is.EqualTo("exactly 3 calls")); }
            [Test] public void Describe_one() { Assert.That(Quantity.Exactly(1).Describe("call", "calls"), Is.EqualTo("exactly 1 call")); }
            [Test] public void Describe_none() { Assert.That(Quantity.Exactly(0).Describe("call", "calls"), Is.EqualTo("no calls")); }
        }
    }
}