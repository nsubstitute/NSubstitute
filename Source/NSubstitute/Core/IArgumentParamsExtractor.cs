using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface IArgumentParamsExtractor
    {
        IEnumerable<object> GetWithExtractedArguments(IEnumerable<object> arguments);
        IEnumerable<int> GetWithExtractedArgumentsToHighlight(IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight);
    }
}
