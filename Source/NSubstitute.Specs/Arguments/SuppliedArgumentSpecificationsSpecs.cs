using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
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

            protected List<IArgumentSpecification> _argumentSpecifications;

            public override void Context()
            {
                _argumentSpecifications = new List<IArgumentSpecification>();
            }

            public override SuppliedArgumentSpecifications CreateSubjectUnderTest()
            {
                return new SuppliedArgumentSpecifications(_argumentSpecifications);
            }
        }

        public class When_has_argument_specifications : BaseConcern
        {
            [Test]
            public void AnyFor_should_return_true()
            {
                Assert.That(sut.AnyFor(Type3), Is.True);
            }

            [Test]
            public void AnyFor_should_return_false()
            {
                Assert.That(sut.AnyFor(typeof(double)), Is.False);
            }

            [Test]
            public void NextFor_should_return_true()
            {
                Assert.That(sut.NextFor(Type1), Is.True);
            }

            [Test]
            public void NextFor_should_return_false()
            {
                Assert.That(sut.NextFor(Type3), Is.False);
            }

            [Test]
            public void Dequeue_should_return_items_singly_in_order_added()
            {
                Assert.That(sut.Dequeue(), Is.EqualTo(_argumentSpecifications[0]));
                Assert.That(sut.Dequeue(), Is.EqualTo(_argumentSpecifications[1]));
                Assert.That(sut.Dequeue(), Is.EqualTo(_argumentSpecifications[2]));
            }

            [Test]
            public void DequeueAll_should_return_all_items_in_order_added()
            {
                Assert.That(sut.DequeueAll(), Is.EquivalentTo(_argumentSpecifications));
            }

            [Test]
            public void DequeueAll_should_remove_all_items()
            {
                sut.DequeueAll();

                Assert.That(sut.DequeueAll().Count(), Is.EqualTo(0));
            }

            public override void Context()
            {
                base.Context();
                _argumentSpecifications.Add(mock<IArgumentSpecification>());
                _argumentSpecifications.Add(mock<IArgumentSpecification>());
                _argumentSpecifications.Add(mock<IArgumentSpecification>());
                _argumentSpecifications[0].stub(x => x.ForType).Return(Type1);
                _argumentSpecifications[1].stub(x => x.ForType).Return(Type2);
                _argumentSpecifications[2].stub(x => x.ForType).Return(Type3);
            }
        }
        public class When_has_no_argument_specifications : BaseConcern
        {
            [Test]
            public void AnyFor_should_return_false()
            {
                Assert.That(sut.AnyFor(Type1), Is.False);
            }

            [Test]
            public void NextFor_should_return_false()
            {
                Assert.That(sut.NextFor(Type1), Is.False);
            }

            [Test]
            public void Dequeue_should_throw_exception()
            {
                Assert.Throws<InvalidOperationException>(() => sut.Dequeue());
            }

            [Test]
            public void DequeueAll_should_return_nothing()
            {
                sut.DequeueAll();
                Assert.That(sut.DequeueAll(), Is.Empty);
            }
        }
    }
}