using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class ParameterInfosFromParamsArrayFactorySpecs
    {
        public class When_creating : ConcernFor<ParameterInfosFromParamsArrayFactory>
        {
            private IEnumerable<IParameterInfo> _result;
            private string[] _paramsArrayArgument;
            private Type _paramsArrayType;

            [Test]
            public void Should_return_parameterInfo_for_each_element_of_array()
            {
                Assert.That(_result.Count(), Is.EqualTo(_paramsArrayArgument.Length));
            }

            [Test]
            public void ParameterInfos_should_have_correct_parameter_type()
            {
                Assert.That(_result.All(x => x.ParameterType == typeof (string)), Is.True);
            }

            [Test]
            public void ParameterInfos_should_have_correct_is_params()
            {
                Assert.That(_result.All(x => x.IsParams == false), Is.True);
            }

            public override void Context()
            {
                _paramsArrayArgument = new[] { "one", "two" };
                _paramsArrayType = _paramsArrayArgument.GetType();
            }

            public override void Because()
            {
                _result = sut.Create(_paramsArrayArgument, _paramsArrayType);
            }

            public override ParameterInfosFromParamsArrayFactory CreateSubjectUnderTest()
            {
                return new ParameterInfosFromParamsArrayFactory();
            }
        }
    }
}