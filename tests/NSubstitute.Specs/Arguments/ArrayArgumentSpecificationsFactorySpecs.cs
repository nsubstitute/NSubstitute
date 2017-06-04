using System;
using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class ArrayArgumentSpecificationsFactorySpecs
    {
        public class When_creating : ConcernFor<ArrayArgumentSpecificationsFactory>
        {
            private IEnumerable<IArgumentSpecification> _result;
            private object[] _arrayArgument;
            private IParameterInfo[] _parameterInfos;
            private ISuppliedArgumentSpecifications _suppliedArgumentSpecifications;
            private IArgumentSpecification[] _expectedResult;
            private INonParamsArgumentSpecificationFactory _nonParamsArgumentSpecificationFactory;

            [Test]
            public void Should_return_argument_specifications_for_each_element_of_array()
            {
                Assert.That(_result, Is.EquivalentTo(_expectedResult));
            }

            public override void Context()
            {
                _expectedResult = new[] { mock<IArgumentSpecification>(), mock<IArgumentSpecification>() };

                _arrayArgument = new[] { "one", "two" };
                _parameterInfos = new[] { mock<IParameterInfo>(), mock<IParameterInfo>() };
                _suppliedArgumentSpecifications = mock<ISuppliedArgumentSpecifications>();

                _nonParamsArgumentSpecificationFactory = mock<INonParamsArgumentSpecificationFactory>();
                _nonParamsArgumentSpecificationFactory.stub(x => x.Create(_arrayArgument[0], _parameterInfos[0], _suppliedArgumentSpecifications)).Return(_expectedResult[0]);
                _nonParamsArgumentSpecificationFactory.stub(x => x.Create(_arrayArgument[1], _parameterInfos[1], _suppliedArgumentSpecifications)).Return(_expectedResult[1]);
            }

            public override void Because()
            {
                _result = sut.Create(_arrayArgument, _parameterInfos, _suppliedArgumentSpecifications);
            }

            public override ArrayArgumentSpecificationsFactory CreateSubjectUnderTest()
            {
                return new ArrayArgumentSpecificationsFactory(_nonParamsArgumentSpecificationFactory);
            }
        }
    }
}