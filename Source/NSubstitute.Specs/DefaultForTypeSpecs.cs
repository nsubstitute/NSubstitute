using System.Threading.Tasks;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class DefaultForTypeSpecs : ConcernFor<DefaultForType>
    {
        [Test]
        public void Should_return_null_for_reference_type()
        {
            Assert.That(sut.GetDefaultFor(typeof(object)), Is.Null);
        }

        [Test]
        public void Should_return_default_for_value_type()
        {
            Assert.That(sut.GetDefaultFor(typeof(int)), Is.EqualTo(default(int)));
        }

        [Test]
        public void Should_return_null_for_void()
        {
            Assert.That(sut.GetDefaultFor(typeof(void)), Is.Null);
        }

        [Test]
        public void Should_never_return_null_for_task()
        {
            Assert.That(sut.GetDefaultFor(typeof(Task)), Is.Not.Null);
        }

        [Test]
        public void Should_never_return_null_for_generic_task()
        {
            Assert.That(sut.GetDefaultFor(typeof(Task<string>)), Is.Not.Null);
        }

        [Test]
        public void Should_return_null_task_result_for_generic_reference_task()
        {
            Task<string> output = (Task<string>)sut.GetDefaultFor(typeof(Task<string>));
            Assert.That(output.Result, Is.Null);
        }

        [Test]
        public void Should_return_default_task_for_generic_value_task()
        {
            Task<int> output = (Task<int>)sut.GetDefaultFor(typeof(Task<int>));
            Assert.That(output.Result, Is.EqualTo(default(int)));
        }

        public override DefaultForType CreateSubjectUnderTest()
        {
            return new DefaultForType();
        }
    }
}