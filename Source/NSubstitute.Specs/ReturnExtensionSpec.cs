using System.Collections.Generic;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ReturnExtensionSpecs
    {
        public class When_setting_a_return_value_for_the_previous_call : StaticConcern
        {
            private object _value;
            private ISubstitutionContext _substitutionContext;
            List<IArgumentMatcher> _argumentMatchers;

            [Test]
            public void Should_tell_the_substitution_context_to_return_the_value_from_the_last_call()
            {
                _substitutionContext.received(x => x.LastCallShouldReturn(_value, _argumentMatchers));
            }

            public override void Because()
            {
                new object().Return(_value);
            }

            public override void Context()
            {
                _value = new object();
                _argumentMatchers = new List<IArgumentMatcher>();
                _substitutionContext = mock<ISubstitutionContext>();
                _substitutionContext.stub(x => x.RetrieveArgumentMatchers()).Return(_argumentMatchers);
                temporarilyChange(() => SubstitutionContext.Current).to(_substitutionContext);
            }
        }
        
    }
}