using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class ArgumentParamsExtractorFactorySpecs : ConcernFor<ArgumentParamsExtractorFactory>
    {
        private MethodInfo _methodInfoWithParams;
        private MethodInfo _methodInfoWithoutParams;
        private IEnumerable<object> _objectArguments;
        private IEnumerable<object> _argumentSpecifications; 

        public override ArgumentParamsExtractorFactory CreateSubjectUnderTest()
        {
            return new ArgumentParamsExtractorFactory();
        }

        public override void Context()
        {
            var sample = Substitute.For<ISample>();
            sample.MethodWithParams("foo");
            sample.MethodWithoutParams(new[] { "bar" });

            _methodInfoWithParams = sample.ReceivedCalls().ToList()[0].GetMethodInfo();
            _methodInfoWithoutParams = sample.ReceivedCalls().ToList()[1].GetMethodInfo();

            _objectArguments = new object[] { "foo", "bar" };
            _argumentSpecifications = new object[] { mock<IArgumentSpecification>() };
        }

        [Test]
        public void Should_always_create_a_PassthroughExtractor_for_empty_arguments()
        {
            Assert.That(sut.Create(_methodInfoWithParams, new object[0], new int[0]), Is.InstanceOf<PassthroughArgumentParamsExtractor>());
            Assert.That(sut.Create(_methodInfoWithoutParams, new object[0], new int[0]), Is.InstanceOf<PassthroughArgumentParamsExtractor>());
        }

        [Test]
        public void Should_create_a_PassthroughExtractor_for_method_without_params()
        {
            Assert.That(sut.Create(_methodInfoWithoutParams, _objectArguments, new int[0]), Is.InstanceOf<PassthroughArgumentParamsExtractor>());
            Assert.That(sut.Create(_methodInfoWithoutParams, _argumentSpecifications, new int[0]), Is.InstanceOf<PassthroughArgumentParamsExtractor>());
        }

        [Test]
        public void Should_create_a_ObjectArgumentParamsExtractor_for_object_arguments()
        {
            Assert.That(sut.Create(_methodInfoWithParams, _objectArguments, new int[0]), Is.InstanceOf<ObjectArgumentParamsExtractor>());
        }

        [Test]
        public void Should_create_a_ArgumentSpecificationParamsExtractor_for_argument_specifications()
        {
            Assert.That(sut.Create(_methodInfoWithParams, _argumentSpecifications, new int[0]), Is.InstanceOf<ArgumentSpecificationParamsExtractor>());
        }

        public interface ISample
        {
            void MethodWithParams(params string[] arguments);
            void MethodWithoutParams(string[] arguments);
        }
    }
}
