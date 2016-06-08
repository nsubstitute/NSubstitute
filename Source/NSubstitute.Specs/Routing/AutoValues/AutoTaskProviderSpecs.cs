#if (NET4 || NET45)
using System;
using System.Threading.Tasks;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using System.Collections.Generic;
using NSubstitute.Core;

namespace NSubstitute.Specs.Routing.AutoValues
{
    public class AutoTaskProviderSpecs : ConcernFor<AutoTaskProvider>
    {
        private IAutoValueProvider _testValuesProvider;
        private IMaybeAutoValueProvider _testMaybeValuesProvider;

        [Test]
        public void Should_create_substitute_with_another_auto_value_provider_if_available()
        {
            const string autoValue = "test";
            _testValuesProvider.stub(x => x.CanProvideValueFor(typeof(string))).Return(true);
            _testValuesProvider.stub(x => x.GetValue(typeof(string))).Return(autoValue);

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

        [Test]
        [TestCase(typeof(string))]
        [TestCase(typeof(int))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(List<int>))]
        public void Provides_no_value_for_non_tasks(Type type)
        {
            Assert.That(((IMaybeAutoValueProvider)sut).GetValue(type), Is.EqualTo(Maybe.Nothing<object>()));
        }

        [Test]
        public void Provides_value_for_tasks()
        {
            Provides_value_for_tasks<string>();
            Provides_value_for_tasks<int>();
            Provides_value_for_tasks<int[]>();
            Provides_value_for_tasks<List<int>>();
        }

        public void Provides_value_for_tasks<T>()
        {
            var value = ((IMaybeAutoValueProvider)sut).GetValue(typeof(Task<T>)).ValueOrDefault();
            Assert.IsInstanceOf<Task<T>>(value);
            Assert.That(((Task<T>) value).Result, Is.EqualTo(default(T)));
        }

        [Test]
        public void Provides_value_from_other_auto_value_providers()
        {
            const string autoValue = "test";
            _testMaybeValuesProvider.stub(x => x.GetValue(typeof(string))).Return(Maybe.Just<object>(autoValue));

            var type = typeof(Task<string>);
            Assert.That(((Task<string>)((IMaybeAutoValueProvider)sut).GetValue(type).ValueOrDefault()).Result, Is.EqualTo(autoValue));
        }

        [Test]
        public void Provides_value_from_other_auto_value_providers_of_value_types()
        {
            const int autoValue = 10;
            _testMaybeValuesProvider.stub(x => x.GetValue(typeof(int))).Return(Maybe.Just<object>(autoValue));

            var type = typeof(Task<int>);
            Assert.That(((Task<int>)((IMaybeAutoValueProvider)sut).GetValue(type).ValueOrDefault()).Result, Is.EqualTo(autoValue));
        }

        public override void Context()
        {
            _testValuesProvider = mock<IAutoValueProvider>();
            _testMaybeValuesProvider = mock<IMaybeAutoValueProvider>();
        }

        public override AutoTaskProvider CreateSubjectUnderTest()
        {
            return new AutoTaskProvider(() => new[] { _testValuesProvider }, () => new[] { _testMaybeValuesProvider });
        }

        public interface IFoo2
        {
            Task<object> GetObjectAsync();
        }
    }
}
#endif