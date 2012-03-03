using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public class PassthroughArgumentParamsExtractor : IArgumentParamsExtractor
    {
        public IEnumerable<object> GetWithExtractedArguments(IEnumerable<object> arguments)
        {
            return arguments;
        }

        public IEnumerable<int> GetWithExtractedArgumentsToHighlight(IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            return argumentsToHighlight;
        }
    }
}