using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class SuppliedArgumentSpecificationsSpecs
    {
        public abstract class BaseConcern : ConcernFor<SuppliedArgumentSpecifications>
        {
            protected readonly Type Type1 = typeof(string);
            protected readonly Type Type2 = typeof(int);
            protected readonly Type Type3 = typeof(float);

            protected List<IArgumentSpecification> ArgumentSpecifications;
            protected IDefaultChecker DefaultChecker;

            public override void Context()
            {
                ArgumentSpecifications = new List<IArgumentSpecification>();
                DefaultChecker = mock<IDefaultChecker>();
            }

            public override SuppliedArgumentSpecifications CreateSubjectUnderTest()
            {
                return new SuppliedArgumentSpecifications(DefaultChecker, ArgumentSpecifications);
            }

            protected object GetDefaultArgumentFor(Type type)
            {
                return CreateArgumentForType(type, true);
            }

            protected object GetNonDefaultArgumentFor(Type type)
            {
                return CreateArgumentForType(type, false);
            }

            private object CreateArgumentForType(Type type, bool isDefaultForType)
            {
                var argument = new object();
                DefaultChecker.stub(x => x.IsDefault(argument, type)).Return(isDefaultForType);
                return argument;
            }
        }

        public class When_has_argument_specifications : BaseConcern
        {
            [Test]
            public void Should_find_supplied_spec_for_compatible_type_with_default_argument()
            {
                Assert.That(sut.AnyFor(GetDefaultArgumentFor(Type3), Type3), Is.True);
            }

            [Test]
            public void Should_not_find_supplied_spec_for_incompatible_type()
            {
                Assert.False(sut.AnyFor(GetDefaultArgumentFor(Type3), typeof(Guid)));
            }

            [Test]
            public void Should_continue_to_find_supplied_specs_after_using_all_specs()
            {
                sut.DequeueRemaining();
                Assert.That(sut.AnyFor(GetDefaultArgumentFor(Type3), Type3), Is.True);
            }

            [Test]
            public void Should_not_find_supplied_spec_for_incompatible_types()
            {
                Assert.That(sut.AnyFor(GetDefaultArgumentFor(typeof(double)), typeof(double)), Is.False);
            }

            [Test]
            public void Should_not_find_supplied_spec_for_compatible_type_with_non_default_argument_for_that_type()
            {
                Assert.That(sut.AnyFor(GetNonDefaultArgumentFor(Type3), Type3), Is.False);
            }

            [Test]
            public void Should_have_next_spec_for_compatible_type_with_default_argument_for_that_type()
            {
                Assert.That(sut.IsNextFor(GetDefaultArgumentFor(Type1), Type1), Is.True);
            }

            [Test]
            public void Should_not_have_next_spec_when_next_spec_is_an_incompatible_type()
            {
                Assert.That(sut.IsNextFor(GetDefaultArgumentFor(Type3), Type3), Is.False);
            }

            [Test]
            public void Should_not_have_next_spec_when_next_spec_is_a_compatible_type_but_a_non_default_argument_is_given()
            {
                Assert.That(sut.IsNextFor(GetNonDefaultArgumentFor(Type1), Type1), Is.False);
            }

            [Test]
            public void Dequeue_should_return_items_singly_in_order_added()
            {
                Assert.That(sut.Dequeue(), Is.EqualTo(ArgumentSpecifications[0]));
                Assert.That(sut.Dequeue(), Is.EqualTo(ArgumentSpecifications[1]));
                Assert.That(sut.Dequeue(), Is.EqualTo(ArgumentSpecifications[2]));
            }

            [Test]
            public void Dequeuing_remaining_should_return_remaining_items_in_order_added()
            {
                Assert.That(sut.DequeueRemaining(), Is.EquivalentTo(ArgumentSpecifications));
            }

            [Test]
            public void Dequeuing_remaing_should_remove_all_items()
            {
                sut.DequeueRemaining();

                Assert.That(sut.DequeueRemaining().Count(), Is.EqualTo(0));
            }

            public override void Context()
            {
                base.Context();
                ArgumentSpecifications.Add(mock<IArgumentSpecification>());
                ArgumentSpecifications.Add(mock<IArgumentSpecification>());
                ArgumentSpecifications.Add(mock<IArgumentSpecification>());
                ArgumentSpecifications[0].stub(x => x.ForType).Return(Type1);
                ArgumentSpecifications[1].stub(x => x.ForType).Return(Type2);
                ArgumentSpecifications[2].stub(x => x.ForType).Return(Type3);
            }
        }

        public class When_has_no_argument_specifications : BaseConcern
        {
            [Test]
            public void Should_not_have_any_specs_for_type()
            {
                Assert.That(sut.AnyFor(GetDefaultArgumentFor(Type1), Type1), Is.False);
            }

            [Test]
            public void Should_not_have_a_next_spec_for_type()
            {
                Assert.That(sut.IsNextFor(GetDefaultArgumentFor(Type1), Type1), Is.False);
            }

            [Test]
            public void Dequeue_should_throw_exception()
            {
                Assert.Throws<InvalidOperationException>(() => sut.Dequeue());
            }

            [Test]
            public void Dequeuing_remaining_should_not_return_any_specs()
            {
                sut.DequeueRemaining();
                Assert.That(sut.DequeueRemaining(), Is.Empty);
            }
        }
    }
}