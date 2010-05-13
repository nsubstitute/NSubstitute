using NSubstitute.Core;
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
            private IReturn _returnValueSet;

            [Test]
            public void Should_tell_the_substitution_context_to_return_the_value_from_the_last_call()
            {
                Assert.That(_returnValueSet, Is.TypeOf<ReturnValue>());
                Assert.That(_returnValueSet.ReturnFor(null), Is.SameAs(_value));
            }

            public override void Because()
            {
                new object().Returns(_value);
            }

            public override void Context()
            {
                _value = new object();
                _substitutionContext = mock<ISubstitutionContext>();
                _substitutionContext
                    .stub(x => x.LastCallShouldReturn(Arg.Any<IReturn>()))
                    .IgnoreArguments()
                    .WhenCalled(x => _returnValueSet = (IReturn) x.Arguments[0]);
                temporarilyChange(() => SubstitutionContext.Current).to(_substitutionContext);
            }
        }
    }
}