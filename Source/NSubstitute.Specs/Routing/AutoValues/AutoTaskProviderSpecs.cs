#if NET4
using System.Threading.Tasks;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using System.Collections.Generic;

namespace NSubstitute.Specs.Routing.AutoValues
{
    public class AutoTaskProviderSpecs : ConcernFor<AutoTaskProvider>
    {
        private IAutoValueProvider _testValuesProvider;

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