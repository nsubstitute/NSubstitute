using NSubstitute.Tests.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Tests
{
    public class ReturnExtensionSpecs
    {
        public class When_setting_a_return_value_for_the_previous_call_to_a_substitute : StaticConcern
        {
            private object value;
            private ISubstitutionContext substitutionContext;

            [Test]
            public void Should_tell_the_substitution_context_to_return_the_value_from_the_last_call()
            {
                substitutionContext.received(x => x.LastCallShouldReturn(value));
            }

            public override void Because()
            {
                new object().Return(value);
            }

            public override void Context()
            {
                value = new object();
                substitutionContext = mock<ISubstitutionContext>();
                SubstitutionContext.SetCurrent(substitutionContext);
            }
        }
        
    }
}