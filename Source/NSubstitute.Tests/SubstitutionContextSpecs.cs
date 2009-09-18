using System;
using NSubstitute.Tests.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Tests
{
    public class SubstitutionContextSpecs
    {
        public abstract class Concern : ConcernFor<SubstitutionContext>
        {
            public override SubstitutionContext CreateSubjectUnderTest()
            {
                return new SubstitutionContext();
            }            
        }
        
        public class When_setting_the_return_value_of_the_last_call : Concern
        {
            private ISubstitute substitute;
            private int valueToReturn;

            [Test]
            public void Should_tell_the_last_called_substitue_to_set_the_return_value_of_its_last_call()
            {
                substitute.received(x => x.LastCallShouldReturn(valueToReturn));
            }
            
            public override void Because()
            {
                sut.LastSubstituteCalled(substitute);
                sut.LastCallShouldReturn(valueToReturn);
            }

            public override void Context()
            {
                substitute = mock<ISubstitute>();
                valueToReturn = 42;
            }

        }

        public class When_trying_to_set_a_return_value_when_no_previous_call_has_been_made : Concern
        {
            [Test]
            public void Should_throw_a_substitution_exception()
            {
                Assert.Throws<SubstitutionException>(() => sut.LastCallShouldReturn(5));
            }
        }
    }
}