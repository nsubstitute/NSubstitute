using System;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class SubstituteStateSpecs
    {
        public class SomeClass { }
        public class InstanceOfInterface : IAmAnInterface { }
        public interface IAmAnInterface { }

        public class When_finding_a_type : ConcernFor<SubstituteState>
        {
            protected object[] _state = new object[0];
            protected object[] _additionalArguments = new object[0];
            protected object _result;

            public override void Because()
            {
                _result = sut.FindInstanceFor(typeof(IAmAnInterface), _additionalArguments);
            }

            public override SubstituteState CreateSubjectUnderTest()
            {
                return new SubstituteState(_state);
            }

        }
        public class When_finding_an_instance_for_a_type_that_exists_in_the_substitute_state : When_finding_a_type
        {
            private InstanceOfInterface _instanceOfInterface;

            [Test]
            public void Should_find_instance_of_state_that_is_assignable_to_the_required_type()
            {
                Assert.That(_result, Is.SameAs(_instanceOfInterface));
            }

            public override void Context()
            {
                _instanceOfInterface = new InstanceOfInterface();
                _state = new object[] { new SomeClass(), _instanceOfInterface };
            }
        }

        public class When_finding_an_instance_of_a_type_that_does_not_exist_in_the_sub_state_but_is_in_additional_arguments : When_finding_a_type
        {
            private object _instanceOfInterface;

            [Test]
            public void Should_find_instance_from_additional_arguments()
            {
                Assert.That(_result, Is.SameAs(_instanceOfInterface));
            }

            public override void Context()
            {
                _instanceOfInterface = new InstanceOfInterface();
                _additionalArguments = new[] { _instanceOfInterface };
            }
        }

        public class When_creating_for_a_substitution_context : StaticConcern
        {
            [Test]
            public void Can_create_an_instance()
            {
                Assert.That(SubstituteState.Create(SubstitutionContext.Current), Is.Not.Null);    
            }
        }
    }
}