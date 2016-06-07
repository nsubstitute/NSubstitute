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
        public void Can_provide_value_for_queryables()
        {
            Assert.That(sut.CanProvideValueFor(typeof(IQueryable<int>)));
            Assert.That(sut.CanProvideValueFor(typeof(IQueryable<List<int>>)));
        }

        [Test]
        public void Can_not_provide_value_for_non_queryable_enumerables()
        {
            Assert.False(sut.CanProvideValueFor(typeof(IEnumerable<>)));
            Assert.False(sut.CanProvideValueFor(typeof(IList<>)));
        }

        [Test]
        public void Can_not_provide_value_for_value_types()
        {
            Assert.False(sut.CanProvideValueFor(typeof(int)));
            Assert.False(sut.CanProvideValueFor(typeof(string)));
        }

        [Test]
        public void Provides_empty_queryable_value_type()
        {
            var queryable = (IQueryable<int>)sut.GetValue(typeof(IQueryable<int>));
            Assert.That(queryable, Is.Not.Null);
            Assert.That(queryable, Is.Empty);
            Assert.DoesNotThrow(() => queryable.Any());
        }

        [Test]
        public void Should_create_substitute_for_queryable_of_string()
        {
            var type = typeof(IQueryable<string>);
            Assert.True(sut.CanProvideValueFor(type));
            Assert.That(sut.GetValue(type), Is.InstanceOf(typeof(IQueryable<string>)));
        }

        [Test]
        public void Should_create_substitute_for_queryable_of_value_type()
        {
            var type = typeof(IQueryable<int>);
            Assert.True(sut.CanProvideValueFor(type));
            Assert.That(sut.GetValue(type), Is.InstanceOf(typeof(IQueryable<int>)));
        }

        [Test]
        public void Should_create_substitute_for_queryable_of_complex_type()
        {
            var type = typeof(IQueryable<List<string>>);
            Assert.True(sut.CanProvideValueFor(type));
            Assert.That(sut.GetValue(type), Is.InstanceOf(typeof(IQueryable<List<string>>)));
        }

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
            Assert.That(((IMaybeAutoValueProvider)sut).GetValue(typeof(IEnumerable<>)), Is.EqualTo(Maybe.Nothing<object>()));
            Assert.That(((IMaybeAutoValueProvider)sut).GetValue(typeof(IList<>)), Is.EqualTo(Maybe.Nothing<object>()));
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
            var queryable = ((IMaybeAutoValueProvider)sut).GetValue(typeof(IQueryable<T>)).ValueOrDefault();
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
