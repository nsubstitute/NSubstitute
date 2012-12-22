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

            Assert.That(((Task<string>)sut.GetValue(typeof(Task<string>))).Result, Is.SameAs(autoValue));
        }

        [Test]
        public void Should_create_substitute_for_task_of_string()
        {
            Assert.That(((Task<string>)sut.GetValue(typeof(Task<string>))).Result, Is.Null);
        }

        [Test]
        public void Should_create_substitute_for_task_of_value_type()
        {
            Assert.That(((Task<int>)sut.GetValue(typeof(Task<int>))).Result, Is.EqualTo(default(int)));
        }

        [Test]
        public void Should_create_substitute_for_task_of_complex_type()
        {
            Assert.That(((Task<List<string>>)sut.GetValue(typeof(Task<List<string>>))).Result, Is.Null);
        }

        [Test]
        public void Should_create_substitute_for_task()
        {
            Assert.True(((Task)sut.GetValue(typeof(Task))).IsCompleted);
        }

        public override void Context()
        {
            _testValuesProvider = mock<IAutoValueProvider>();
        }

        public override AutoTaskProvider CreateSubjectUnderTest()
        {
            return new AutoTaskProvider(new[] { _testValuesProvider });
        }
    }
}
#endif