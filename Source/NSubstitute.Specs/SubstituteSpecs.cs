using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class SubstituteSpecs
    {
        public class When_substituting_for_a_type : StaticConcern
        {
            Foo result;
            Foo substitute;
            ISubstituteFactory substituteFactory;
            ISubstitutionContext context;

            [Test]
            public void Should_return_a_substitute_from_factory()
            {
                Assert.That(result, Is.SameAs(substitute));
            }

            public override void Because()
            {
                result = Substitute.For<Foo>();
            }

            public override void Context()
            {
                substitute = mock<Foo>();
                substituteFactory = mock<ISubstituteFactory>();
                substituteFactory.stub(x => x.Create<Foo>()).Return(substitute);
                context = mock<ISubstitutionContext>();
                context.stub(x => x.GetSubstituteFactory()).Return(substituteFactory);
                temporarilyChange(() => SubstitutionContext.Current).to(context);
            }
        }
    }
}