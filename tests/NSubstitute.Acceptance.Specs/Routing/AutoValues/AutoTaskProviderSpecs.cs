#if (NET4 || NET45)
using System;
using System.Threading.Tasks;
using NSubstitute.Routing.AutoValues;
using NUnit.Framework;
using System.Collections.Generic;

namespace NSubstitute.Specs.Routing.AutoValues
{
    public class AutoTaskProviderSpecs
    {
        private class TestAutoValueProvider : IAutoValueProvider {
            private IDictionary<Type, object> stubs = new Dictionary<Type, object>();
            public void Stub<T>(T value) => stubs[typeof(T)] = value;
            public bool CanProvideValueFor(Type type) => stubs.ContainsKey(type);
            public object GetValue(Type type) => stubs[type];
        }

        private TestAutoValueProvider _testValuesProvider;
        private AutoTaskProvider sut;

        public AutoTaskProviderSpecs()
        {
            _testValuesProvider = new TestAutoValueProvider();
            sut = new AutoTaskProvider(
                new Lazy<IReadOnlyCollection<IAutoValueProvider>>( () => new [] { _testValuesProvider })
            );
        }

        [Test]
        public void Should_create_substitute_with_another_auto_value_provider_if_available()
        {
            const string autoValue = "test";
            _testValuesProvider.Stub(autoValue);

            var type = typeof (Task<string>);
            Assert.True(sut.CanProvideValueFor(type));
            Assert.That(((Task<string>)sut.GetValue(type)).Result, Is.SameAs(autoValue));
        }

        [Test]
        public void Should_create_substitute_for_task_of_string()
        {
            var type = typeof (Task<string>);
            Assert.True(sut.CanProvideValueFor(type));
            Assert.That(((Task<string>)sut.GetValue(type)).Result, Is.Null);
        }

        [Test]
        public void Should_create_substitute_for_task_of_value_type()
        {
            var type = typeof (Task<int>);
            Assert.True(sut.CanProvideValueFor(type));
            Assert.That(((Task<int>)sut.GetValue(type)).Result, Is.EqualTo(default(int)));
        }

        [Test]
        public void Should_create_substitute_for_task_of_complex_type()
        {
            var type = typeof (Task<List<string>>);
            Assert.True(sut.CanProvideValueFor(type));
            Assert.That(((Task<List<string>>)sut.GetValue(type)).Result, Is.Null);
        }

        [Test]
        public void Should_create_substitute_for_task()
        {
            var type = typeof (Task);
            Assert.True(sut.CanProvideValueFor(type));
            Assert.True(((Task)sut.GetValue(type)).IsCompleted);
        }

        [Test]
        public void Substitute_should_automock_task()
        {
            var foo2 = Substitute.For<IFoo2>();

            var task = foo2.GetObjectAsync();

            Assert.NotNull(task);
            Assert.Null(task.Result);
        }

        public interface IFoo2
        {
            Task<object> GetObjectAsync();
        }
    }
}
#endif