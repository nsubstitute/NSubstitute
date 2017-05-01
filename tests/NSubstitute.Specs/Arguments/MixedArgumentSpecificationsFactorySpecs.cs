using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class MixedArgumentSpecificationsFactorySpecs
    {
        public class When_creating_argument_specifications : ConcernFor<MixedArgumentSpecificationsFactory>
        {
            protected IList<IArgumentSpecification> _argumentSpecifications;
            protected object[] _arguments;
            private IArgumentSpecificationFactory _argumentSpecificationFactory;
            private ISuppliedArgumentSpecificationsFactory _suppliedArgumentSpecificationsFactory;
            protected IParameterInfo[] _parameterInfos;
            protected IArgumentSpecification[] _expectedResults;
            protected IEnumerable<IArgumentSpecification> _result;

            public override void Context()
            {
                _expectedResults = new[] { mock<IArgumentSpecification>(), mock<IArgumentSpecification>() };
                _arguments = new object[] { 1, "fred" };
                _parameterInfos = new[] { mock<IParameterInfo>(), mock<IParameterInfo>() };
                _argumentSpecifications = new List<IArgumentSpecification>
                                              {
                                                  mock<IArgumentSpecification>(),
                                                  mock<IArgumentSpecification>()
                                              };
                _argumentSpecificationFactory = mock<IArgumentSpecificationFactory>();
                var suppliedArgumentSpecifications = mock<ISuppliedArgumentSpecifications>();
                _suppliedArgumentSpecificationsFactory = mock<ISuppliedArgumentSpecificationsFactory>();
                _suppliedArgumentSpecificationsFactory.stub(x => x.Create(_argumentSpecifications)).Return(suppliedArgumentSpecifications);
                _argumentSpecificationFactory.stub(x => x.Create(_arguments[0], _parameterInfos[0], suppliedArgumentSpecifications)).Return(_expectedResults[0]);
                _argumentSpecificationFactory.stub(x => x.Create(_arguments[1], _parameterInfos[1], suppliedArgumentSpecifications)).Return(_expectedResults[1]);
            }

            public override MixedArgumentSpecificationsFactory CreateSubjectUnderTest()
            {
                return new MixedArgumentSpecificationsFactory(_argumentSpecificationFactory, _suppliedArgumentSpecificationsFactory);
            }

            public override void Because()
            {
                _result = sut.Create(_argumentSpecifications, _arguments, _parameterInfos);
            }

            [Test]
            public void Should_have_specifications_for_all_arguments()
            {
                Assert.That(_result, Is.EquivalentTo(_expectedResults));
            }
        }
    }
}