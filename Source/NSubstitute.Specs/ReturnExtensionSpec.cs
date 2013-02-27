using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ReturnExtensionSpecs
    {
        public abstract class When_setting_return_value : StaticConcern
        {
            protected object _value;
            protected ISubstitutionContext _substitutionContext;
            protected IReturn _returnValueSet;

            [Test]
            public void Should_tell_the_substitution_context_to_return_the_value_from_the_last_call()
            {
                Assert.That(_returnValueSet, Is.TypeOf<ReturnValue>());
                Assert.That(_returnValueSet.ReturnFor(null), Is.SameAs(_value));
            }

            public override void Context()
            {
                _value = new object();
                _substitutionContext = mock<ISubstitutionContext>();
                _substitutionContext
                    .stub(x => x.LastCallShouldReturn(It.IsAny<IReturn>(), It.IsAny<MatchArgs>()))
                    .IgnoreArguments()
                    .Return(new ConfiguredCall(x => { }))
                    .WhenCalled(x => _returnValueSet = (IReturn) x.Arguments[0]);
                temporarilyChange(() => SubstitutionContext.Current).to(_substitutionContext);
            }
        }

        public class When_setting_a_return_value_for_the_previous_call : When_setting_return_value
        {
            [Test]
            public void Should_match_last_calls_arguments()
            {
                _substitutionContext.received(x => x.LastCallShouldReturn(_returnValueSet, MatchArgs.AsSpecifiedInCall)); 
            }

            public override void Because()
            {
                new object().Returns(_value);
            }
        }

        public class When_setting_a_return_value_for_the_previous_call_with_any_args : When_setting_return_value
        {
            [Test]
            public void Should_not_match_last_calls_arguments()
            {
                _substitutionContext.received(x => x.LastCallShouldReturn(_returnValueSet, MatchArgs.Any)); 
            }

            public override void Because()
            {
                new object().ReturnsForAnyArgs(_value);
            }
        }
    }
}