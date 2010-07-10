using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface IArgumentsFormatter
    {
        string Format(IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight);
    }
}