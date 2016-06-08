using System;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using NSubstitute.Core;

namespace NSubstitute.Specs.Routing.AutoValues
{
    public class AutoQueryableProviderSpecs : ConcernFor<AutoQueryableProvider>
    {
        [Test]
        public void Substitute_should_automock_queryable()
        {
            var foo2 = Substitute.For<IFoo2>();

            var queryable = foo2.GetObject();

            Assert.That(queryable, Is.Not.Null);
            Assert.That(queryable, Is.Empty);
            Assert.DoesNotThrow(() => queryable.Any());
        }

        [Test]
        public void Provides_no_value_for_non_queryable()
        {
            Assert.That(sut.GetValue(typeof(IEnumerable<>)), Is.EqualTo(Maybe.Nothing<object>()));
            Assert.That(sut.GetValue(typeof(IList<>)), Is.EqualTo(Maybe.Nothing<object>()));
        }

        [Test]
        public void Provides_value_for_queryables()
        {
            Provides_value_for_queryables<int>();
            Provides_value_for_queryables<string>();
            Provides_value_for_queryables<List<int>>();
        }

        private void Provides_value_for_queryables<T>()
        {
            var queryable = sut.GetValue(typeof(IQueryable<T>)).ValueOrDefault();
            Assert.IsInstanceOf<IQueryable<T>>(queryable);
            CollectionAssert.AreEquivalent(new T[0], (IQueryable<T>)queryable);
        }

        public override AutoQueryableProvider CreateSubjectUnderTest()
        {
            return new AutoQueryableProvider();
        }

        public interface IFoo2
        {
            IQueryable<object> GetObject();
        }
    }
}
