using System;
using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class ParamsArgumentSpecificationFactorySpecs
    {
        //If default value 
        //If no argument specs and none for that type then just match literal value
        //If no argument specs and has some for that type throw ambiguous
        //If one argument spec that matches array type of parameter then return argument spec
        //If one argument spec that doesn't match array type of parameter then throw ambiguous
        //If more than one argument spec then throw ambiguous
        //else
        //If argument specs not all for array element type then throw ambiguous
        //Call NonParamsArgSpecFactory with each child element then create ArrayContentsArgSpec
        //If no argument specs then match literal values in array

        public abstract class BaseConcern : ConcernFor<ParamsArgumentSpecificationFactory>
        {
            protected IArgumentSpecification _result;
            protected IDefaultChecker _defaultChecker;
            protected object _argument;
            protected IParameterInfo _parameterInfo;
            protected ISuppliedArgumentSpecifications _suppliedArgumentSpecifications;
            protected Queue<IArgumentSpecification> _argumentSpecificationsToSupply;
            protected IArgumentEqualsSpecificationFactory _argumentEqualsSpecificationFactory;
            protected IArrayArgumentSpecificationsFactory _arrayArgumentSpecificationsFactory;
            protected IParameterInfosFromParamsArrayFactory _parameterInfosFromParamsArrayFactory;
            protected ISuppliedArgumentSpecificationsFactory SuppliedArgumentSpecificationsFactory;
            protected IArrayContentsArgumentSpecificationFactory _arrayContentsArgumentSpecificationFactory;

            public override void Context()
            {
                base.Context();
                _argument = new[] { "one", "two", "three" };
                _parameterInfo = mock<IParameterInfo>();
                _parameterInfo.stub(x => x.ParameterType).Return(typeof(string[]));
                _argumentEqualsSpecificationFactory = mock<IArgumentEqualsSpecificationFactory>();
                _arrayArgumentSpecificationsFactory = mock<IArrayArgumentSpecificationsFactory>();
                _parameterInfosFromParamsArrayFactory = mock<IParameterInfosFromParamsArrayFactory>();
                SuppliedArgumentSpecificationsFactory = mock<ISuppliedArgumentSpecificationsFactory>();
                _arrayContentsArgumentSpecificationFactory = mock<IArrayContentsArgumentSpecificationFactory>();
                _defaultChecker = mock<IDefaultChecker>();
                _argumentSpecificationsToSupply = new Queue<IArgumentSpecification>();
                _suppliedArgumentSpecifications = mock<ISuppliedArgumentSpecifications>();
                _suppliedArgumentSpecifications.stub(x => x.Dequeue()).Return(null).WhenCalled(x => x.ReturnValue = _argumentSpecificationsToSupply.Dequeue());
                _suppliedArgumentSpecifications.stub(x => x.DequeueRemaining()).Return(null).WhenCalled(x => x.ReturnValue = _argumentSpecificationsToSupply);
            }


            public override ParamsArgumentSpecificationFactory CreateSubjectUnderTest()
            {
                return new ParamsArgumentSpecificationFactory(_defaultChecker, 
                                                            _argumentEqualsSpecificationFactory, 
                                                            _arrayArgumentSpecificationsFactory,
                                                            _parameterInfosFromParamsArrayFactory,
                                                            SuppliedArgumentSpecificationsFactory,
                                                            _arrayContentsArgumentSpecificationFactory);
            }

            public override void Because()
            {
                _result = sut.Create(_argument, _parameterInfo, _suppliedArgumentSpecifications);
            }

        }

        public abstract class When_argument_is_default_value : BaseConcern
        {
            public override void Context()
            {
                base.Context();
                _defaultChecker.stub(x => x.IsDefault(_argument, _parameterInfo.ParameterType)).Return(true);
            }

            //If no argument specs and none for that type then just match literal value
            public class Given_no_specifications_and_none_for_type : When_argument_is_default_value
            {
                private IArgumentSpecification _argumentEqualsSpecification;
                public override void Context()
                {
                    base.Context();
                    _argumentEqualsSpecification = mock<IArgumentSpecification>();
                    _suppliedArgumentSpecifications.stub(x => x.AnyFor(_argument, _parameterInfo.ParameterType)).Return(false);
                    _argumentEqualsSpecificationFactory.stub(x => x.Create(_argument, _parameterInfo.ParameterType)).Return(_argumentEqualsSpecification);
                }

                [Test]
                public void Should_return_equals_specification()
                {
                    Assert.That(_result, Is.EqualTo(_argumentEqualsSpecification));
                }
            }

            //If one argument spec that matches array type of parameter then return argument spec
            public class Given_next_specification_supplied_is_for_parameter_type : When_argument_is_default_value
            {
                private IArgumentSpecification _suppliedSpecification;
                public override void Context()
                {
                    base.Context();
                    _suppliedSpecification = mock<IArgumentSpecification>();
                    _suppliedArgumentSpecifications.stub(x => x.AnyFor(_argument, _parameterInfo.ParameterType)).Return(true);
                    _suppliedArgumentSpecifications.stub(x => x.IsNextFor(_argument, _parameterInfo.ParameterType)).Return(true);
                    _argumentSpecificationsToSupply.Enqueue(_suppliedSpecification);
                }

                [Test]
                public void Should_return_supplied_specification()
                {
                    Assert.That(_result, Is.EqualTo(_suppliedSpecification));
                }
            }

            //If no argument specs and has some for that type throw ambiguous
            public abstract class Given_will_throw_ambiguous_exception : When_argument_is_default_value
            {
                private Exception _capturedException;

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

            //If no argument specs and has some for that type throw ambiguous
            [TestFixture]
            public class Given_next_supplied_specification_is_not_for_parameter_type_and_specifications_for_type_are_available : Given_will_throw_ambiguous_exception
            {
                public override void Context()
                {
                    base.Context();
                    _suppliedArgumentSpecifications.stub(x => x.AnyFor(_argument, _parameterInfo.ParameterType)).Return(true);
                    _suppliedArgumentSpecifications.stub(x => x.IsNextFor(_argument, _parameterInfo.ParameterType)).Return(false);
                }
            }

            //If one argument spec that doesn't match array type of parameter then throw ambiguous
            [TestFixture]
            public class Given_next_supplied_specification_is_not_for_parameter_type_and_one_specification : Given_will_throw_ambiguous_exception
            {
                public override void Context()
                {
                    base.Context();
                    _suppliedArgumentSpecifications.stub(x => x.AnyFor(_argument, _parameterInfo.ParameterType)).Return(false);
                    _suppliedArgumentSpecifications.stub(x => x.IsNextFor(_argument, _parameterInfo.ParameterType)).Return(false);
                    _argumentSpecificationsToSupply.Enqueue(mock<IArgumentSpecification>());
                }
            }

            //If more than one argument spec then throw ambiguous
            [TestFixture]
            public class Given_next_supplied_specification_is_not_for_parameter_type_and_more_than_one_specification : Given_will_throw_ambiguous_exception
            {
                public override void Context()
                {
                    base.Context();
                    _suppliedArgumentSpecifications.stub(x => x.AnyFor(_argument, _parameterInfo.ParameterType)).Return(true);
                    _suppliedArgumentSpecifications.stub(x => x.IsNextFor(_argument, _parameterInfo.ParameterType)).Return(true);
                    _argumentSpecificationsToSupply.Enqueue(mock<IArgumentSpecification>());
                    _argumentSpecificationsToSupply.Enqueue(mock<IArgumentSpecification>());
                }
            }

        }

        //If argument specs not all for array element type then throw ambiguous
        //Call NonParamsArgSpecFactory with each child element then create ArrayContentsArgSpec
        //If no argument specs then match literal values in array
        public class When_argument_is_not_default_value : BaseConcern
        {
            private IArgumentSpecification _arrayContentsArgumentSpecification;

            public override void Context()
            {
                base.Context();
                _arrayContentsArgumentSpecification = mock<IArgumentSpecification>();
                _defaultChecker.stub(x => x.IsDefault(_argument, _parameterInfo.ParameterType)).Return(false);
                var paramterInfosFromParamsArray = mock<IEnumerable<IParameterInfo>>();
                _parameterInfosFromParamsArrayFactory.stub(x => x.Create(_argument, _parameterInfo.ParameterType)).Return(paramterInfosFromParamsArray);
                var suppliedArgumentSpecificationsFromParamsArray = mock<ISuppliedArgumentSpecifications>();
                SuppliedArgumentSpecificationsFactory.stub(x => x.Create(_suppliedArgumentSpecifications.DequeueRemaining())).Return(suppliedArgumentSpecificationsFromParamsArray);
                var arrayArgumentSpecifications = mock<IEnumerable<IArgumentSpecification>>();
                _arrayArgumentSpecificationsFactory.stub(x => x.Create(_argument, paramterInfosFromParamsArray, suppliedArgumentSpecificationsFromParamsArray)).Return(arrayArgumentSpecifications);
                _arrayContentsArgumentSpecificationFactory.stub(x => x.Create(arrayArgumentSpecifications, _parameterInfo.ParameterType)).Return(_arrayContentsArgumentSpecification);
            }

            [Test]
            public void Should_return_array_contents_argument_specification()
            {
                Assert.That(_result, Is.EqualTo(_arrayContentsArgumentSpecification));
            }
        }
    }
}