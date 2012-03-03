using NSubstitute.Core;

namespace NSubstitute.Specs
{
    public class ObjectArgumentParamsExtractorSpecs : ConcernForArgumentParamsExtractor
    {
        public override ArgumentParamsExtractor CreateSubjectUnderTest()
        {
            return new ObjectArgumentParamsExtractor();
        }

        public override void Context()
        {
            _argumentsWithAnArray = new object[] {new[] {"foo", "bar"}};
            _argumentsWithObjectAndArray = new object[] { "blah", new[] { "foo", "bar" } };
            _argumentsWithoutAnArray = new object[] {"foobar"};
            _emptyArguments = new object[0];

            _noArgumentsHighlighted = new int[0];
            _firstArgumentHighlighted = new[] { 0 };
            _secondArgumentHighlighted = new[] {1};
        }

        
    }
}
