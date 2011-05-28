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
            protected object _argument;
            protected IParameterInfo _parameterInfo;
            protected ISuppliedArgumentSpecifications _suppliedArgumentSpecifications;
            protected IArgumentEqualsSpecificationFactory _argumentEqualsSpecificationFactory;

            public override void Context()
            {
                base.Context();
                _parameterInfo = mock<IParameterInfo>();
                _argumentEqualsSpecificationFactory = mock<IArgumentEqualsSpecificationFactory>();
                _suppliedArgumentSpecifications = mock<ISuppliedArgumentSpecifications>();
            }

            public override NonParamsArgumentSpecificationFactory CreateSubjectUnderTest()
            {
                return new NonParamsArgumentSpecificationFactory(_argumentEqualsSpecificationFactory);
            }

            public override void Because()
            {
                _result = sut.Create(_argument, _parameterInfo, _suppliedArgumentSpecifications);
            }
        }

        public class When_next_supplied_arg_spec_works_for_this_argument : BaseConcern
        {
            private IArgumentSpecification _suppliedSpecification;
            public override void Context()
            {
                base.Context();
                _suppliedSpecification = mock<IArgumentSpecification>();
                _suppliedArgumentSpecifications.stub(x => x.IsNextFor(_argument, _parameterInfo.ParameterType)).Return(true);
                _suppliedArgumentSpecifications.stub(x => x.Dequeue()).Return(_suppliedSpecification);
            }

            [Test]
            public void Should_return_next_arg_spec()
            {
                Assert.That(_result, Is.EqualTo(_suppliedArgumentSpecifications.Dequeue()));
            }
        }

        public class When_no_supplied_argument_specs_work_for_this_argument : BaseConcern
        {
            private IArgumentSpecification _argumentEqualsSpecification;
            public override void Context()
            {
                base.Context();
                _argumentEqualsSpecification = mock<IArgumentSpecification>();
                _argumentEqualsSpecificationFactory.stub(x => x.Create(_argument, _parameterInfo.ParameterType)).Return(_argumentEqualsSpecification);
                _suppliedArgumentSpecifications.stub(x => x.IsNextFor(_argument, _parameterInfo.ParameterType)).Return(false);
                _suppliedArgumentSpecifications.stub(x => x.AnyFor(_argument, _parameterInfo.ParameterType)).Return(false);
            }

            [Test]
            public void Should_return_equals_specification()
            {
                Assert.That(_result, Is.EqualTo(_argumentEqualsSpecification));
            }
        }

        public abstract class When_next_arg_spec_does_not_work_for_this_arg_but_other_supplied_specs_do : BaseConcern
        {
            private Exception _capturedException;

            public override void Context()
            {
                base.Context();
                _suppliedArgumentSpecifications.stub(x => x.IsNextFor(_argument, _parameterInfo.ParameterType)).Return(false);
                _suppliedArgumentSpecifications.stub(x => x.AnyFor(_argument, _parameterInfo.ParameterType)).Return(true);
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