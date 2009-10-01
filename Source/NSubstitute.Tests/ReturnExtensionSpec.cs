using NSubstitute.Tests.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Tests
{
    public class ReturnExtensionSpecs
    {
        public class When_setting_a_return_value_for_the_previous_invocation : StaticConcern
        {
            private object value;
            private ISubstitutionContext substitutionContext;

            [Test]
            public void Should_tell_the_substitution_context_to_return_the_value_from_the_last_invocation()
            {
                substitutionContext.received(x => x.LastInvocationShouldReturn(value));
            }

            public override void Because()
            {
                new object().Return(value);
            }

            public override void Context()
            {
                value = new object();
                substitutionContext = mock<ISubstitutionContext>();
                temporarilyChange(SubstitutionContext.Current)
                    .to(substitutionContext)
                    .via(x => SubstitutionContext.Current = x);
            }
        }
        
    }
}