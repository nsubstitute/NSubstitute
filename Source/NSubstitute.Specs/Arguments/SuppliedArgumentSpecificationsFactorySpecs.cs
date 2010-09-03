using System;
using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class SuppliedArgumentSpecificationsFactorySpecs
    {
        public class When_creating : ConcernFor<SuppliedArgumentSpecificationsFactory>
        {
            private ISuppliedArgumentSpecifications _result;
            private IArgumentSpecification[] _argumentSpecifications;

            [Test]
            public void Should_create_suppliedAgumentSpecifications_with_correct_specifications()
            {
                Assert.That(_result.DequeueAll(), Is.EquivalentTo(_argumentSpecifications));
            }

            public override void Context()
            {
                _argumentSpecifications = new[] { mock<IArgumentSpecification>(), mock<IArgumentSpecification>() };
            }

            public override void Because()
            {
                _result = sut.Create(_argumentSpecifications);
            }

            public override SuppliedArgumentSpecificationsFactory CreateSubjectUnderTest()
            {
                return new SuppliedArgumentSpecificationsFactory();
            }
        }
    }
}