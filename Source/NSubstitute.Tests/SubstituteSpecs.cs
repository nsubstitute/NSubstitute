using NSubstitute.Tests.TestInfrastructure;
using NSubstitute.Tests.TestStructures;
using NUnit.Framework;

namespace NSubstitute.Tests
{
    public class SubstituteSpecs
    {
        public class When_substituting_for_a_type : StaticConcern
        {
            Foo result;
            Foo substitute;
            ISubstituteFactory _substituteFactory;

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
                _substituteFactory = mock<ISubstituteFactory>();
                _substituteFactory.stub(x => x.Create<Foo>()).Return(substitute);
                temporarilyChange(SubstitutionFactory.Current)
                    .to(_substituteFactory)
                    .via(x => SubstitutionFactory.Current = x);
            }
        }
    }
}