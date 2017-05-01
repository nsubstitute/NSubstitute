using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class ArgumentSpecificationFactorySpecs
    {
        public abstract class BaseConcern : ConcernFor<ArgumentSpecificationFactory>
        {
            protected IArgumentSpecification _result;
            protected object _argument;
            protected IParameterInfo _parameterInfo;
            protected ISuppliedArgumentSpecifications _suppliedArgumentSpecifications;
            protected IParamsArgumentSpecificationFactory _paramsArgumentSpecificationFactory;
            protected INonParamsArgumentSpecificationFactory _nonParamsArgumentSpecificationFactory;

            public override void Context()
            {
                base.Context();
                _parameterInfo = mock<IParameterInfo>();
                _paramsArgumentSpecificationFactory = mock<IParamsArgumentSpecificationFactory>();
                _nonParamsArgumentSpecificationFactory = mock<INonParamsArgumentSpecificationFactory>();
                _suppliedArgumentSpecifications = mock<ISuppliedArgumentSpecifications>();
            }

            public override ArgumentSpecificationFactory CreateSubjectUnderTest()
            {
                return new ArgumentSpecificationFactory(_paramsArgumentSpecificationFactory, _nonParamsArgumentSpecificationFactory);
            }

            public override void Because()
            {
                _result = sut.Create(_argument, _parameterInfo, _suppliedArgumentSpecifications);
            }

        }

        public class When_argument_is_not_params : BaseConcern
        {
            private IArgumentSpecification _nonParamsArgumentSpecification;
            public override void Context()
            {
                base.Context();
                _nonParamsArgumentSpecification = mock<IArgumentSpecification>();
                _parameterInfo.stub(x => x.IsParams).Return(false);
                _nonParamsArgumentSpecificationFactory.stub(x => x.Create(_argument, _parameterInfo, _suppliedArgumentSpecifications)).Return(_nonParamsArgumentSpecification);
            }

            [Test]
            public void Should_return_nonparams_specification()
            {
                Assert.That(_result, Is.EqualTo(_nonParamsArgumentSpecification));
            }
        }
        
        public class When_argument_is_params : BaseConcern
        {
            private IArgumentSpecification _paramsArgumentSpecification;

            public override void Context()
            {
                base.Context();
                _paramsArgumentSpecification = mock<IArgumentSpecification>();
                _parameterInfo.stub(x => x.IsParams).Return(true);
                _paramsArgumentSpecificationFactory
                    .stub(x => x.Create(_argument, _parameterInfo, _suppliedArgumentSpecifications))
                    .Return(_paramsArgumentSpecification);
            }

            [Test]
            public void Should_return_params_specification()
            {
                Assert.That(_result, Is.EqualTo(_paramsArgumentSpecification));
            }
        }
    }
}