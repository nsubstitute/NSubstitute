using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class ArgumentFormatInfoFactorySpecs : ConcernFor<ArgumentFormatInfoFactory>
    {
        private MethodInfo _callWithoutParams;
        private MethodInfo _callWithParams;
        private IEnumerable<object> _objectArgumentsWithoutAnArray;
        private IEnumerable<int> _argumentsToHighlight;
        private IEnumerable<object> _emptyArguments;
        private IEnumerable<object> _objectArgumentsWithAnArray;
        private IEnumerable<object> _argumentSpecification;

        public override ArgumentFormatInfoFactory CreateSubjectUnderTest()
        {
            return new ArgumentFormatInfoFactory();
        }

        public override void Context()
        {
            var sample = Substitute.For<ISample>();
            sample.MethodWithParams("foo");
            sample.MethodWithoutParams(new[] {"bar"});

            _callWithParams = sample.ReceivedCalls().ToList()[0].GetMethodInfo();
            _callWithoutParams = sample.ReceivedCalls().ToList()[1].GetMethodInfo();

            _emptyArguments = new object[0];
            _argumentsToHighlight = new int[0];
            _objectArgumentsWithoutAnArray = new object[] { "foo" };
            _objectArgumentsWithAnArray = new object[] { new[] { "foo", "bar"}};
            _argumentSpecification = CreateArgumentSpecification();
        }


        private IEnumerable<object> CreateArgumentSpecification()
        {
            IArgumentSpecification argumentSpecification = mock<IArgumentSpecification>();

            return new object[] {argumentSpecification};
        }

        public interface ISample
        {
            void MethodWithoutParams(string[] arguments);
            void MethodWithParams(params string[] arguments);
        }

        [Test]
        public void Should_not_return_argument_format_infos_from_empty_arguments()
        {
            var argumentFormatInfos = sut.CreateArgumentFormatInfos(_callWithoutParams, _emptyArguments, _argumentsToHighlight);

            Assert.That(argumentFormatInfos.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Should_not_return_params_argument_format_infos_from_empty_arguments()
        {
            var argumentFormatInfos = sut.CreateArgumentFormatInfos(_callWithParams, _emptyArguments, _argumentsToHighlight);

            Assert.That(argumentFormatInfos.Count(), Is.EqualTo(0));
        }
        
        [Test]
        public void Should_create_object_argument_format_info_from_object_argument()
        {
            var argumentFormatInfos = sut.CreateArgumentFormatInfos(_callWithoutParams, _objectArgumentsWithoutAnArray, _argumentsToHighlight);

            Assert.That(argumentFormatInfos.Count(), Is.EqualTo(1));
            Assert.That(argumentFormatInfos.Single(), Is.InstanceOf<ObjectArgumentFormatInfo>());
        }

        [Test]
        public void Should_create_params_object_argument_format_info_from_object_array_argument()
        {
            var argumentFormatInfos = sut.CreateArgumentFormatInfos(_callWithParams, _objectArgumentsWithAnArray, _argumentsToHighlight);

            Assert.That(argumentFormatInfos.Count(), Is.EqualTo(1));
            Assert.That(argumentFormatInfos.Single(), Is.InstanceOf<ObjectParamsArrayArgumentFormatInfo>());
        }

        [Test]
        public void Should_throw_an_exception_for_object_argument_without_array_contents()
        {
            Assert.Throws<ArgumentException>(() => sut.CreateArgumentFormatInfos(_callWithParams, _objectArgumentsWithoutAnArray, _argumentsToHighlight));
        }

        [Test]
        public void Should_create_argument_specification_format_info_from_argument_specification()
        {
            var argumentFormatInfos = sut.CreateArgumentFormatInfos(_callWithoutParams, _argumentSpecification, _argumentsToHighlight);

            Assert.That(argumentFormatInfos.Count(), Is.EqualTo(1));
            Assert.That(argumentFormatInfos.Single(), Is.InstanceOf<ArgumentSpecificationFormatInfo>());
        }

        [Test]
        public void Should_create_params_argument_specification_format_info_from_argument_specification()
        {
            var argumentFormatInfos = sut.CreateArgumentFormatInfos(_callWithParams, _argumentSpecification, _argumentsToHighlight);

            Assert.That(argumentFormatInfos.Count(), Is.EqualTo(1));
            Assert.That(argumentFormatInfos.Single(), Is.InstanceOf<ArgumentSpecificationParamsArrayFormatInfo>());
        }
    }
}
