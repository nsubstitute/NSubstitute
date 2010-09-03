using System;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class NonParamsArgumentSpecificationFactorySpecs
    {
        public abstract class BaseConcern : ConcernFor<NonParamsArgumentSpecificationFactory>
        {
            protected IArgumentSpecification _result;
            protected IDefaultChecker _defaultChecker;
            protected object _argument;
            protected IParameterInfo _parameterInfo;
            protected ISuppliedArgumentSpecifications _suppliedArgumentSpecifications;
            protected IArgumentEqualsSpecificationFactory _argumentEqualsSpecificationFactory;

            public override void Context()
            {
                base.Context();
                _parameterInfo = mock<IParameterInfo>();
                _argumentEqualsSpecificationFactory = mock<IArgumentEqualsSpecificationFactory>();
                _defaultChecker = mock<IDefaultChecker>();
                _suppliedArgumentSpecifications = mock<ISuppliedArgumentSpecifications>();
            }

            public override NonParamsArgumentSpecificationFactory CreateSubjectUnderTest()
            {
                return new NonParamsArgumentSpecificationFactory(_defaultChecker, _argumentEqualsSpecificationFactory);
            }

            public override void Because()
            {
                _result = sut.Create(_argument, _parameterInfo, _suppliedArgumentSpecifications);
            }
        }

        public class When_argument_is_not_default_value : BaseConcern
        {
            private IArgumentSpecification _argumentEqualsSpecification;
            public override void Context()
            {
                base.Context();
                _argumentEqualsSpecification = mock<IArgumentSpecification>();
                _defaultChecker.stub(x => x.IsDefault(_argument, _parameterInfo.ParameterType)).Return(false);
                _argumentEqualsSpecificationFactory.stub(x => x.Create(_argument, _parameterInfo.ParameterType)).Return(_argumentEqualsSpecification);
            }

            [Test]
            public void Should_return_equals_specification()
            {
                Assert.That(_result, Is.EqualTo(_argumentEqualsSpecification));
            }
        }
        
        public abstract class When_argument_is_default_value : BaseConcern
        {
            public override void Context()
            {
                base.Context();
                _defaultChecker.stub(x => x.IsDefault(_argument, _parameterInfo.ParameterType)).Return(true);
            }

            public class Given_no_specifications_supplied_for_parameter_type: When_argument_is_default_value
            {
                private IArgumentSpecification _argumentEqualsSpecification;
                public override void Context()
                {
                    base.Context();
                    _argumentEqualsSpecification = mock<IArgumentSpecification>();
                    _suppliedArgumentSpecifications.stub(x => x.AnyFor(_parameterInfo.ParameterType)).Return(false);
                    _argumentEqualsSpecificationFactory.stub(x => x.Create(_argument, _parameterInfo.ParameterType)).Return(_argumentEqualsSpecification);
                }

                [Test]
                public void Should_return_equals_specification()
                {
                    Assert.That(_result, Is.EqualTo(_argumentEqualsSpecification));
                }
            }
            
            public class Given_next_specification_supplied_is_for_parameter_type: When_argument_is_default_value
            {
                private IArgumentSpecification _suppliedSpecification;
                public override void Context()
                {
                    base.Context();
                    _suppliedSpecification = mock<IArgumentSpecification>();
                    _suppliedArgumentSpecifications.stub(x => x.AnyFor(_parameterInfo.ParameterType)).Return(true);
                    _suppliedArgumentSpecifications.stub(x => x.NextFor(_parameterInfo.ParameterType)).Return(true);
                    _suppliedArgumentSpecifications.stub(x => x.Dequeue()).Return(_suppliedSpecification);
                }

                [Test]
                public void Should_return_supplied_specification()
                {
                    Assert.That(_result, Is.EqualTo(_suppliedSpecification));
                }
            }

            public class Given_next_supplied_specification_is_not_for_parameter_type_and_specifications_for_type_are_available: When_argument_is_default_value
            {
                private Exception _capturedException;

                public override void Context()
                {
                    base.Context();
                    _suppliedArgumentSpecifications.stub(x => x.AnyFor(_parameterInfo.ParameterType)).Return(true);
                    _suppliedArgumentSpecifications.stub(x => x.NextFor(_parameterInfo.ParameterType)).Return(false);
                }

                public override void Because()
                {
                    try
                    {
                        base.Because();
                    }
                    catch (Exception e)
                    {
                        _capturedException = e;
                    }
                }
                [Test]
                public void Should_throw_ambiguous_arguments_exception()
                {
                    Assert.That(_capturedException, Is.TypeOf(typeof(AmbiguousArgumentsException)));
                }
            }
        }
    }
}