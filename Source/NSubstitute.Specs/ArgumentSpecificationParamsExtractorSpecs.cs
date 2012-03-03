using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;

namespace NSubstitute.Specs
{
    public class ArgumentSpecificationParamsExtractorSpecs : ConcernForArgumentParamsExtractor
    {
        public override void Context()
        {
            _argumentsWithAnArray = new object[] { CreateArrayArgumentSpecification() };
            _argumentsWithObjectAndArray = new object[] { CreateArgumentSpecification(), CreateArrayArgumentSpecification() };
            _argumentsWithoutAnArray = new object[] { CreateArgumentSpecification() };
            _emptyArguments = new object[0];

            _noArgumentsHighlighted = new int[0];
            _firstArgumentHighlighted = new[] { 0 };
            _secondArgumentHighlighted = new[] { 1 };
        }

        private IArgumentSpecification CreateArrayArgumentSpecification()
        {
            IArgumentSpecification argumentSpecification = mock<IArgumentSpecification>();
            ArrayContentsArgumentMatcher arrayContentsArgumentMatcher = new ArrayContentsArgumentMatcher(new[] { CreateArgumentSpecification(), CreateArgumentSpecification() });

            argumentSpecification.stub(a => a.ArgumentMatcher).Return(arrayContentsArgumentMatcher);

            return argumentSpecification;
        }

        private IArgumentSpecification CreateArgumentSpecification()
        {
            IArgumentMatcher argumentMatcher = mock<IArgumentMatcher>();
            IArgumentSpecification argumentSpecification = mock<IArgumentSpecification>();
            argumentSpecification.stub(a => a.ArgumentMatcher).Return(argumentMatcher);

            return argumentSpecification;
        }

        public override ArgumentParamsExtractor CreateSubjectUnderTest()
        {
            return new ArgumentSpecificationParamsExtractor();
        }
    }
}