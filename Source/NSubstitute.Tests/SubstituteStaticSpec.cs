using NSubstitute.Tests.TestInfrastructure;
using NSubstitute.Tests.TestStructures;
using NUnit.Framework;

namespace NSubstitute.Tests
{
    public class SubstituteStaticSpec
    {
        public class When_substituting_for_a_type : StaticConcern
        {
            Foo result;
            Foo substitute;
            ISubstitutionFactory substitutionFactory;

            [Test]
            public void Should_return_substitute_from_factory()
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
                substitutionFactory = mock<ISubstitutionFactory>();
                substitutionFactory.stub(x => x.Create<Foo>()).Return(substitute);
                temporarilyChange(SubstitutionFactory.Current)
                    .to(substitutionFactory)
                    .via(x => SubstitutionFactory.Current = x);
            }
        }
    }
}