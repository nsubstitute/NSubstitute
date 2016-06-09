using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.AutoValues
{
    public class CompositeProviderSpecs : ConcernFor<CompositeProvider>
    {
        private IAutoValueProvider _provider1;
        private IAutoValueProvider _provider2;
        private IAutoValueProvider _provider3;
        private List<IAutoValueProvider> _providers = new List<IAutoValueProvider>();

        [Test]
        public void Provides_no_value_when_no_provider_provides()
        {
            Assert.That(sut.GetValue(typeof(object)), Is.EqualTo(Maybe.Nothing<object>()));
        }

        [Test]
        public void Provides_value_that_first_provider_provides()
        {
            var type = typeof(object);
            var value2 = new object();
            var value3 = new object();
            _provider1.stub(x => x.GetValue(type)).Return(Maybe.Nothing<object>());
            _provider2.stub(x => x.GetValue(type)).Return(Maybe.Just(value2));
            _provider3.stub(x => x.GetValue(type)).Return(Maybe.Just(value3));

            Assert.That(sut.GetValue(typeof(object)), Is.EqualTo(Maybe.Just(value2)));
        }

        [Test]
        public void Provides_value_that_new_provider_provides()
        {
            var newProvider = mock<IAutoValueProvider>();
            _providers.Insert(0, newProvider);
            var type = typeof(object);
            var newValue = new object();
            var value2 = new object();
            var value3 = new object();
            newProvider.stub(x => x.GetValue(type)).Return(Maybe.Just(newValue));
            _provider1.stub(x => x.GetValue(type)).Return(Maybe.Nothing<object>());
            _provider2.stub(x => x.GetValue(type)).Return(Maybe.Just(value2));
            _provider3.stub(x => x.GetValue(type)).Return(Maybe.Just(value3));

            Assert.That(sut.GetValue(typeof(object)), Is.EqualTo(Maybe.Just(newValue)));
        }

        public override void Context()
        {
            base.Context();
            _provider1 = mock<IAutoValueProvider>();
            _provider2 = mock<IAutoValueProvider>();
            _provider3 = mock<IAutoValueProvider>();
            _providers.AddRange(new [] { _provider1, _provider2, _provider3 });
        }

        public override CompositeProvider CreateSubjectUnderTest()
        {
            return new CompositeProvider(_providers);
        }
    }
}
