#if NET45
using System;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using System.Collections.Generic;
using NSubstitute.Core;

namespace NSubstitute.Specs.Routing.AutoValues
{
    public class AutoObservableProviderSpecs : ConcernFor<AutoObservableProvider>
    {
        private IAutoValueProvider _testValuesProvider;

        [Test]
        public void Substitute_should_automock_observable()
        {
            var foo2 = Substitute.For<IFoo2>();
            var observer = mock<IObserver<object>>();

            var observable = foo2.GetObject();

            Assert.NotNull(observable);
            observable.Subscribe(observer);
            observer.received(x => x.OnNext(It.Is<object>(null)));
            observer.received(x => x.OnCompleted());
        }

        [Test]
        [TestCase(typeof(string))]
        [TestCase(typeof(int))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(List<int>))]
        public void Provides_no_value_for_non_observables(Type type)
        {
            Assert.That(sut.GetValue(type), Is.EqualTo(Maybe.Nothing<object>()));
        }

        [Test]
        public void Provides_value_for_observables()
        {
            Provides_value_for_observables<string>();
            Provides_value_for_observables<int>();
            Provides_value_for_observables<int[]>();
            Provides_value_for_observables<List<int>>();
        }

        public void Provides_value_for_observables<T>()
        {
            var observer = mock<IObserver<T>>();
            var value = sut.GetValue(typeof(IObservable<T>)).ValueOrDefault();
            Assert.IsInstanceOf<IObservable<T>>(value);
            ((IObservable<T>) value).Subscribe(observer);
            observer.received(x => x.OnNext(default(T)));
            observer.received(x => x.OnCompleted());
        }

        [Test]
        public void Provides_value_from_other_auto_value_providers()
        {
            const string autoValue = "test";
            var observer = mock<IObserver<string>>();
            _testValuesProvider.stub(x => x.GetValue(typeof(string))).Return(Maybe.Just<object>(autoValue));

            var type = typeof(IObservable<string>);
            var value = (IObservable<string>)sut.GetValue(type).ValueOrDefault();
            value.Subscribe(observer);
            observer.received(x => x.OnNext(It.Is(autoValue)));
            observer.received(x => x.OnCompleted());
        }

        [Test]
        public void Provides_value_from_other_auto_value_providers_of_value_types()
        {
            const int autoValue = 10;
            var observer = mock<IObserver<int>>();
            _testValuesProvider.stub(x => x.GetValue(typeof(int))).Return(Maybe.Just<object>(autoValue));

            var type = typeof(IObservable<int>);
            var value = (IObservable<int>)sut.GetValue(type).ValueOrDefault();
            value.Subscribe(observer);
            observer.received(x => x.OnNext(It.Is(autoValue)));
            observer.received(x => x.OnCompleted());
        }

        public override void Context()
        {
            _testValuesProvider = mock<IAutoValueProvider>();
        }

        public override AutoObservableProvider CreateSubjectUnderTest()
        {
            return new AutoObservableProvider(() => _testValuesProvider);
        }

        public interface IFoo2
        {
            IObservable<object> GetObject();
        }
    }
}
#endif