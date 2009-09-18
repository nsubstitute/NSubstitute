using System;
using NSubstitute.Tests.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Tests
{
    public class SubstitutionContextSpecs
    {
        public class When_setting_the_return_value_of_the_last_call : ConcernFor<SubstitutionContext>
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
                sut.LastSubstitute(substitute);
                sut.LastCallShouldReturn(valueToReturn);
            }

            public override void Context()
            {
                substitute = mock<ISubstitute>();
                valueToReturn = 42;
            }

            public override SubstitutionContext CreateSubjectUnderTest()
            {
                return new SubstitutionContext();
            }
        }
    }
}