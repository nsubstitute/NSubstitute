using System;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class SubstituteSpecs
    {
        public class When_substituting_for_a_type : StaticConcern
        {
            Foo _result;
            Foo _substitute;
            ISubstituteFactory _substituteFactory;
            ISubstitutionContext _context;

            [Test]
            public void Should_return_a_substitute_from_factory()
            {
                Assert.That(_result, Is.SameAs(_substitute));
            }

            public override void Because()
            {
                _result = Substitute.For<Foo>();
            }

            public override void Context()
            {
                _substitute = mock<Foo>();
                _substituteFactory = mock<ISubstituteFactory>();
                _substituteFactory.stub(x => x.Create<Foo>(new Type[0], new object[0])).Return(_substitute);
                _context = mock<ISubstitutionContext>();
                _context.stub(x => x.GetSubstituteFactory()).Return(_substituteFactory);
                temporarilyChange(() => SubstitutionContext.Current).to(_context);
            }
        }
    }
}