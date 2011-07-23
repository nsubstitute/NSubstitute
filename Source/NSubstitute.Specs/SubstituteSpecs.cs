using System;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class SubstituteSpecs
    {
        public class When_substituting_for_a_types : StaticConcern
        {
            object _result;
            Foo _substitute;
            ISubstituteFactory _substituteFactory;
            ISubstitutionContext _context;
            private Type[] _types;
            private object[] _constructorArgs;

            [Test]
            public void Should_return_a_substitute_from_factory()
            {
                Assert.That(_result, Is.SameAs(_substitute));
            }

            public override void Because()
            {
                _result = Substitute.For(_types, _constructorArgs);
            }

            public override void Context()
            {
                _types = new[] { typeof(Foo) };
                _constructorArgs = new[] { new object() };
                _substitute = mock<Foo>();
                _substituteFactory = mock<ISubstituteFactory>();
                _substituteFactory.stub(x => x.Create(_types, _constructorArgs)).Return(_substitute);
                _context = mock<ISubstitutionContext>();
                _context.stub(x => x.SubstituteFactory).Return(_substituteFactory);
                temporarilyChange(() => SubstitutionContext.Current).to(_context);
            }
        }
    }
}