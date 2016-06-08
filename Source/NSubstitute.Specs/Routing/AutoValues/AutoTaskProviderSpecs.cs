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
            Assert.That(sut.GetValue(type), Is.EqualTo(Maybe.Nothing<object>()));
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
            var value = sut.GetValue(typeof(Task<T>)).ValueOrDefault();
            Assert.IsInstanceOf<Task<T>>(value);
            Assert.That(((Task<T>) value).Result, Is.EqualTo(default(T)));
        }

        [Test]
        public void Provides_value_from_other_auto_value_providers()
        {
            const string autoValue = "test";
            _testValuesProvider.stub(x => x.GetValue(typeof(string))).Return(Maybe.Just<object>(autoValue));

            var type = typeof(Task<string>);
            Assert.That(((Task<string>)sut.GetValue(type).ValueOrDefault()).Result, Is.EqualTo(autoValue));
        }

        [Test]
        public void Provides_value_from_other_auto_value_providers_of_value_types()
        {
            const int autoValue = 10;
            _testValuesProvider.stub(x => x.GetValue(typeof(int))).Return(Maybe.Just<object>(autoValue));

            var type = typeof(Task<int>);
            Assert.That(((Task<int>)sut.GetValue(type).ValueOrDefault()).Result, Is.EqualTo(autoValue));
        }

        public override void Context()
        {
            _testValuesProvider = mock<IAutoValueProvider>();
        }

        public override AutoTaskProvider CreateSubjectUnderTest()
        {
            return new AutoTaskProvider(() => new[] { _testValuesProvider });
        }

        public interface IFoo2
        {
            Task<object> GetObjectAsync();
        }
    }
}
#endif